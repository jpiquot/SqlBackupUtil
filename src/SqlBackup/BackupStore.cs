using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Microsoft.Extensions.Options;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace SqlBackup
{
    public class BackupStore
    {
        private readonly IDirectoryService _directory;
        private readonly IOptions<BackupStoreSettings> _options;
        private readonly Server _server;
        private List<BackupDatabaseFile>? _backupDatabaseFiles;
        private IEnumerable<string>? _backupFiles;
        private List<BackupHeader>? _backupHeaders;
        private List<BackupMediaHeader>? _backupMediaHeaders;

        public BackupStore(Server server, IOptions<BackupStoreSettings> options, IDirectoryService? directory = null)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _server = server ?? throw new ArgumentNullException(nameof(server));
            _directory = directory ?? new SqlDirectoryService(server.InstanceName, options.Value.Login, options.Value.Password);
        }

        public BackupStore(string serverName, IOptions<BackupStoreSettings> options, IDirectoryService? directory = null)
        {
            if (string.IsNullOrWhiteSpace(serverName))
            {
                throw new ArgumentException(Properties.Resources.ArgumentIsNullOrWhitespace, nameof(serverName));
            }
            string? login = options.Value.Login;
            var con = new ServerConnection
            {
                ServerInstance = serverName
            };
            if (!string.IsNullOrWhiteSpace(login))
            {
                con.LoginSecure = false;
                con.Authentication = SqlConnectionInfo.AuthenticationMethod.SqlPassword;
                con.Login = login;
                con.Password = options.Value.Password;
            }
            _server = new Server(con);
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _directory = directory ?? new SqlDirectoryService(serverName, options.Value.Login, options.Value.Password);
        }

        public List<BackupDatabaseFile> BackupDatabaseFiles => _backupDatabaseFiles ??= InitBackupDatabaseFiles();

        public IEnumerable<string> BackupFiles
            => _backupFiles ??= _directory.GetFiles(_options.Value.BackupPaths, _options.Value.BackupFileExtensions, _options.Value.IncludeSubDirectories);

        public List<BackupHeader> BackupHeaders => _backupHeaders ??= InitBackupHeaders();
        public List<BackupMediaHeader> BackupMediaHeaders => _backupMediaHeaders ??= InitBackupMediaHeaders();

        public int FullBackupCount(string serverName, string databaseName) => GetFullBackupHeaders(serverName, databaseName).Count();

        public IEnumerable<BackupHeader> GetBackupHeaders(string? serverName, string? databaseName, BackupType? type = null, DateTime? before = null)
            => BackupHeaders.Where(p =>
                (type == null || p.BackupType == type) &&
                (string.IsNullOrEmpty(databaseName) || p.DatabaseName == databaseName) &&
                (string.IsNullOrEmpty(serverName) || p.ServerName == serverName) &&
                (before == null || p.StartDate <= before)
            )
            .OrderBy(p => p.ServerName)
                .ThenBy(p => p.DatabaseName)
                    .ThenByDescending(p => p.LastLSN)
            .ToList();

        public IEnumerable<BackupHeader> GetDiffBackupHeaders(string? serverName, string? databaseName)
            => GetBackupHeaders(serverName, databaseName, BackupType.Differential);

        public IEnumerable<BackupHeader> GetFullBackupHeaders(string? serverName, string? databaseName, DateTime? before = null)
            => GetBackupHeaders(serverName, databaseName, BackupType.Full, before);

        public BackupHeader GetFullForDiff(BackupHeader diff)
        {
            BackupHeader? full = GetFullBackupHeaders(diff.ServerName, diff.DatabaseName, diff.StartDate)
                .Where(p => p.CheckpointLSN == diff.DatabaseBackupLSN)
                .FirstOrDefault();
            if (full == null)
            {
                throw new ApplicationException(string.Format(Properties.Resources.FullNotFoundForDiffBackup, diff.FileName));
            }
            return full;
        }

        public BackupHeader GetLatestBackup(string serverName, string databaseName, bool includeDifferentials = true, bool includeLogs = false, DateTime? before = null)
            => GetBackupHeaders(serverName, databaseName, null, before)
                .Where(p => p.BackupType == BackupType.Full ||
                                (includeDifferentials && p.BackupType == BackupType.Differential) ||
                                (includeLogs && p.BackupType == BackupType.Log))
                .FirstOrDefault()
            ?? throw new ApplicationException(string.Format(Properties.Resources.FullBackupNotFound, serverName, databaseName, before ?? DateTime.Now));

        public IEnumerable<BackupHeader> GetLatestBackupSet(string serverName, string databaseName, bool includeDifferentials = true, bool includeLogs = false, DateTime? before = null)
        {
            BackupHeader latestBackup = GetLatestBackup(serverName, databaseName, includeDifferentials, includeLogs, before);

            return latestBackup.BackupType switch
            {
                BackupType.Full => new List<BackupHeader> { latestBackup },
                BackupType.Differential => new List<BackupHeader> { GetFullForDiff(latestBackup), latestBackup },
                BackupType.Log => GetAllForLog(latestBackup),
                _ => Array.Empty<BackupHeader>()
            };
        }

        public BackupHeader GetLatestFull(string serverName, string databaseName, DateTime? before = null)
            => GetFullBackupHeaders(serverName, databaseName, before)
                .OrderByDescending(p => p.CheckpointLSN)
                .FirstOrDefault()
            ?? throw new ApplicationException(string.Format(Properties.Resources.FullBackupNotFound, serverName, databaseName, before ?? DateTime.Now));

        public BackupHeader GetLatestFullOrDiff(string serverName, string databaseName, DateTime? before = null)
        {
            BackupHeader? full = GetLatestFull(serverName, databaseName, before);
            BackupHeader? diff = GetBackupHeaders(serverName, databaseName, null, before)
                .Where(p => p.BackupType == BackupType.Differential && p.StartDate > full.StartDate)
                .FirstOrDefault();
            return diff ?? full ?? throw new ApplicationException(string.Format(Properties.Resources.FullBackupNotFound, serverName, databaseName, before ?? DateTime.Now));
        }

        public IEnumerable<BackupHeader> GetLogBackupHeaders(string? serverName, string? databaseName)
            => GetBackupHeaders(serverName, databaseName, BackupType.Log);

        private IEnumerable<BackupHeader> GetAllForLog(BackupHeader latestBackup)
        {
            var backups = new List<BackupHeader>();
            var latestDiffOrFull = GetLatestFullOrDiff(latestBackup.ServerName, latestBackup.DatabaseName, latestBackup.StartDate);
            IEnumerable<BackupHeader>? logs = GetBackupHeaders(latestBackup.ServerName, latestBackup.DatabaseName, BackupType.Log, latestBackup.StartDate)
                .Where(p => p.StartDate > latestDiffOrFull.StartDate);
            if (latestDiffOrFull.BackupType == BackupType.Differential)
            {
                backups.Add(GetFullForDiff(latestDiffOrFull));
            }
            backups.Add(latestDiffOrFull);
            backups.AddRange(logs);
            return backups;
        }

        private List<BackupDatabaseFile> InitBackupDatabaseFiles()
        {
            var list = new List<BackupDatabaseFile>(BackupFiles.Count());
            foreach (string file in BackupFiles)
            {
                var media = new BackupFile(_server, file);
                list.AddRange(media.BackupDatabaseFiles);
            }
            return list;
        }

        private List<BackupHeader> InitBackupHeaders()
        {
            var list = new List<BackupHeader>(BackupFiles.Count());
            foreach (string file in BackupFiles)
            {
                var media = new BackupFile(_server, file);
                list.AddRange(media.BackupHeaders);
            }
            return list;
        }

        private List<BackupMediaHeader> InitBackupMediaHeaders()
        {
            var list = new List<BackupMediaHeader>(BackupFiles.Count());
            foreach (string file in BackupFiles)
            {
                var media = new BackupFile(_server, file);
                list.AddRange(media.BackupMediaHeaders);
            }
            return list;
        }
    }
}