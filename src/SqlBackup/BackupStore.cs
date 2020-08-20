using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Abstractions;
using System.Linq;

using Microsoft.Extensions.Options;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace SqlBackup
{
    public class BackupStore
    {
        private readonly IFileSystem _fileSystem;
        private readonly IOptions<BackupStoreSettings> _options;
        private readonly Server _server;
        private List<BackupDatabaseFile>? _backupDatabaseFiles;
        private List<string>? _backupFiles;
        private List<BackupHeader>? _backupHeaders;
        private List<BackupMediaHeader>? _backupMediaHeaders;

        public BackupStore(Server server, IFileSystem fileSystem, IOptions<BackupStoreSettings> options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _server = server ?? throw new ArgumentNullException(nameof(server));
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(_fileSystem));
        }

        public BackupStore(string serverName, IFileSystem fileSystem, IOptions<BackupStoreSettings> options)
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
            if (login != null)
            {
                con.LoginSecure = false;
                con.Authentication = SqlConnectionInfo.AuthenticationMethod.SqlPassword;
                con.Login = login;
                con.Password = options.Value.Password;
            }
            _server = new Server(con);
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(_fileSystem));
        }

        public List<BackupDatabaseFile> BackupDatabaseFiles => _backupDatabaseFiles ??= InitBackupDatabaseFiles();
        public List<string> BackupFiles => _backupFiles ??= InitBackupFileNames();
        public List<BackupHeader> BackupHeaders => _backupHeaders ??= InitBackupHeaders();
        public List<BackupMediaHeader> BackupMediaHeaders => _backupMediaHeaders ??= InitBackupMediaHeaders();

        public int FullBackupCount(string serverName, string databaseName) => GetFullBackupHeaders(serverName, databaseName).Count();

        public IEnumerable<BackupHeader> GetBackupHeaders(string? serverName, string? databaseName, BackupType? type = null, DateTime? before = null)
            => BackupHeaders.Where(p =>
                (type == null || p.BackupType == type) &&
                (string.IsNullOrEmpty(databaseName) || p.DatabaseName == databaseName) &&
                (string.IsNullOrEmpty(serverName) || p.ServerName == serverName) &&
                (before == null || p.StartDate == before)
            ).ToList();

        public IEnumerable<BackupHeader> GetDiffBackupHeaders(string? serverName, string? databaseName)
            => GetBackupHeaders(serverName, databaseName, BackupType.Differential);

        public IEnumerable<BackupHeader> GetFullBackupHeaders(string? serverName, string? databaseName)
            => GetBackupHeaders(serverName, databaseName, BackupType.Full);

        public BackupHeader GetFullForDiff(BackupHeader diff)
        {
            BackupHeader? full = GetFullBackupHeaders(diff.ServerName, diff.DatabaseName).Where(p => p.CheckpointLSN == diff.DatabaseBackupLSN).FirstOrDefault();
            if (full == null)
            {
                throw new ApplicationException(string.Format(Properties.Resources.FullNotFoundForDiffBackup, diff.FileName));
            }
            return full;
        }

        public IEnumerable<BackupHeader> GetLatestBackup(string serverName, string databaseName, BackupType? backupType, DateTime? before = null)
        {
            BackupHeader? latestBackup = GetBackupHeaders(serverName, databaseName, backupType, before)
                .Where(p => p.BackupType != BackupType.NotSupported)
                .OrderByDescending(p => p.StartDate)
                .FirstOrDefault();
            if (latestBackup == null)
            {
                return Array.Empty<BackupHeader>();
            }
            return latestBackup.BackupType switch
            {
                BackupType.Full => new List<BackupHeader> { latestBackup },
                BackupType.Differential => new List<BackupHeader> { GetFullForDiff(latestBackup), latestBackup },
                BackupType.Log => GetAllForLog(latestBackup),
                _ => Array.Empty<BackupHeader>()
            };
        }

        public IEnumerable<BackupHeader> GetLatestDiffWithFull(string serverName, string databaseName, DateTime? before = null)
        {
            IEnumerable<BackupHeader>? headers = GetDiffBackupHeaders(serverName, databaseName);
            var list = new List<BackupHeader>(2);
            if (headers.Any())
            {
                decimal lsn;
                if (before == null)
                {
                    lsn = headers.Max(p => p.LastLSN);
                }
                else
                {
                    lsn = headers.Where(p => p.StartDate < before).Max(p => p.LastLSN);
                }

                BackupHeader? diff = headers.Where(p => p.LastLSN == lsn).First();
                BackupHeader full = GetFullForDiff(diff);
                list.Add(full);
                list.Add(diff);
                return list;
            }
            throw new ApplicationException(string.Format(Properties.Resources.DiffBackupNotFound, serverName, databaseName, before ?? DateTime.Now));
        }

        public BackupHeader GetLatestFull(string serverName, string databaseName, DateTime? before = null)
        {
            IEnumerable<BackupHeader>? headers = GetFullBackupHeaders(serverName, databaseName);
            if (headers.Any())
            {
                decimal lsn;
                if (before == null)
                {
                    lsn = headers.Max(p => p.CheckpointLSN);
                }
                else
                {
                    lsn = headers.Where(p => p.StartDate < before).Max(p => p.CheckpointLSN);
                }

                return headers.Where(p => p.CheckpointLSN == lsn).First();
            }
            throw new ApplicationException(string.Format(Properties.Resources.FullBackupNotFound, serverName, databaseName, before ?? DateTime.Now));
        }

        public IEnumerable<BackupHeader> GetLogBackupHeaders(string? serverName, string? databaseName)
            => GetBackupHeaders(serverName, databaseName, BackupType.Log);

        private IEnumerable<BackupHeader> GetAllForLog(BackupHeader latestBackup) => Array.Empty<BackupHeader>();

        private List<BackupDatabaseFile> InitBackupDatabaseFiles()
        {
            var list = new List<BackupDatabaseFile>(BackupFiles.Count);
            foreach (string file in BackupFiles)
            {
                var media = new BackupFile(_server, file);
                list.AddRange(media.BackupDatabaseFiles);
            }
            return list;
        }

        private List<string> InitBackupFileNames()
        {
            string[] extensions = _options.Value.BackupFileExtensions.Select(p => p.ToUpperInvariant()).ToArray();
            var list = new List<string>();
            foreach (string path in _options.Value.BackupPaths)
            {
                foreach (string extension in _options.Value.BackupFileExtensions)
                {
                    list.AddRange(_fileSystem
                        .Directory
                        .EnumerateFiles(
                                path,
                                "*." + extension,
                                new EnumerationOptions
                                {
                                    RecurseSubdirectories = true,
                                    MatchType = MatchType.Simple,
                                    MatchCasing = MatchCasing.CaseInsensitive
                                }));
                }
            }
            return list;
        }

        private List<BackupHeader> InitBackupHeaders()
        {
            var list = new List<BackupHeader>(BackupFiles.Count);
            foreach (string file in BackupFiles)
            {
                var media = new BackupFile(_server, file);
                list.AddRange(media.BackupHeaders);
            }
            return list;
        }

        private List<BackupMediaHeader> InitBackupMediaHeaders()
        {
            var list = new List<BackupMediaHeader>(BackupFiles.Count);
            foreach (string file in BackupFiles)
            {
                var media = new BackupFile(_server, file);
                list.AddRange(media.BackupMediaHeaders);
            }
            return list;
        }
    }
}