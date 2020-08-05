using System;
using System.Collections.Generic;
using System.Data;
using System.IO.Abstractions;
using System.Linq;

using Microsoft.Extensions.Options;
using Microsoft.SqlServer.Management.Smo;

namespace SqlBackup.Database
{
    public class BackupStore
    {
        private readonly IFileSystem _fileSystem;
        private readonly IOptions<BackupStoreSettings> _options;
        private readonly Server _server;
        private List<BackupDatabaseFile>? _backupDatabaseFiles;
        private List<BackupHeader>? _backupHeaders;
        private List<BackupMediaHeader>? _backupMediaHeaders;
        private List<string>? _backupFiles;

        public BackupStore(Server server, IFileSystem fileSystem, IOptions<BackupStoreSettings> options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _server = server ?? throw new ArgumentNullException(nameof(server));
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(_fileSystem));
        }

        public List<BackupHeader> BackupHeaders => _backupHeaders ??= InitBackupHeaders();
        public List<BackupMediaHeader> BackupMediaHeaders => _backupMediaHeaders ??= InitBackupMediaHeaders();
        public List<BackupDatabaseFile> BackupDatabaseFiles => _backupDatabaseFiles ??= InitBackupDatabaseFiles();
        public List<string> BackupFiles => _backupFiles ??= InitBackupFileNames();
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

        private List<string> InitBackupFileNames()
        {
            string[] extensions = _options.Value.BackupFileExtensions.Select(p => p.ToUpperInvariant()).ToArray();
            var list = new List<string>();
            foreach (string path in _options.Value.BackupPaths)
            {
                string[]? files = _fileSystem.Directory.GetFiles(path);
                list.AddRange(files.Where(
                    p => extensions
                        .Contains(
                            _fileSystem
                                .Path
                                .GetExtension(p)
                                .ToUpperInvariant()
                                .TrimStart('.')
                                )
                        )
                    );
            }
            return list;
        }

        private IEnumerable<BackupHeader> GetBackupHeaders(BackupType type, string serverName, string databaseName)
            => BackupHeaders.Where(p => p.BackupType == type && p.DatabaseName == databaseName && p.ServerName == serverName).ToList();
        private IEnumerable<BackupHeader> GetFullBackupHeaders(string serverName, string databaseName)
            => GetBackupHeaders(BackupType.Full, serverName, databaseName);
        private IEnumerable<BackupHeader> GetDiffBackupHeaders(string serverName, string databaseName)
            => GetBackupHeaders(BackupType.Differential, serverName, databaseName);
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
            throw new ApplicationException(string.Format(Properties.Resources.FullBackupNotFound, serverName, databaseName, before??DateTime.Now));
        }
        public int FullBackupCount(string serverName, string databaseName) => GetFullBackupHeaders(serverName, databaseName).Count();
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

                var diff = headers.Where(p => p.LastLSN == lsn).First();
                var full = GetFullBackupHeaders(serverName, databaseName).Where(p => p.CheckpointLSN == diff.DatabaseBackupLSN).FirstOrDefault();
                if (full == null)
                {
                    throw new ApplicationException(string.Format(Properties.Resources.FullNotFoundForDiffBackup, diff.FileName));
                }
                list.Add(full);
                list.Add(diff);
                return list;
            }
            throw new ApplicationException(string.Format(Properties.Resources.DiffBackupNotFound, serverName, databaseName, before ?? DateTime.Now));
        }
    }
}