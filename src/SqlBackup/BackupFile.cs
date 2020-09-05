using System;
using System.Collections.Generic;
using System.Data;

using Microsoft.SqlServer.Management.Smo;

namespace SqlBackup
{
    public class BackupFile
    {
        private readonly string _backupFile;
        private readonly Server _server;
        private List<BackupDatabaseFile>? _backupDatabaseFiles;
        private List<BackupHeader>? _backupHeaders;
        private List<BackupMediaHeader>? _backupMediaHeaders;
        private Restore? _restore;

        public BackupFile(Server server, string backupFile)
        {
            _server = server ?? throw new ArgumentNullException(nameof(server));
            _backupFile = backupFile ?? throw new ArgumentNullException(nameof(backupFile));
        }

        public List<BackupDatabaseFile> BackupDatabaseFiles => _backupDatabaseFiles ??= InitBackupDatabaseFiles();
        public List<BackupHeader> BackupHeaders => _backupHeaders ??= InitBackupHeaders();
        public List<BackupMediaHeader> BackupMediaHeaders => _backupMediaHeaders ??= InitBackupMediaHeaders();
        public Restore Restore => _restore ??= InitRestore();

        private static List<T> CreateList<T>(DataTable table, Func<Dictionary<string, object>, T> create)
        {
            var list = new List<T>(table.Rows.Count);
            foreach (DataRow? row in table.Rows)
            {
                if (row == null)
                {
                    continue;
                }
                list.Add(create(row.ToDictionary()));
            }
            return list;
        }

        private List<BackupDatabaseFile> InitBackupDatabaseFiles()
                    => CreateList(Restore.ReadFileList(_server), (dict) => new BackupDatabaseFile(dict, _backupFile));

        private List<BackupHeader> InitBackupHeaders()
            => (Restore.Devices.Count > 0) ? CreateList(Restore.ReadBackupHeader(_server), (dict) => new BackupHeader(dict, _backupFile)) : new List<BackupHeader>();

        private List<BackupMediaHeader> InitBackupMediaHeaders()
            => (Restore.Devices.Count > 0) ? CreateList(Restore.ReadMediaHeader(_server), (dict) => new BackupMediaHeader(dict, _backupFile)) : new List<BackupMediaHeader>();

        private Restore InitRestore()
        {
            var restore = new Restore();
            restore.Devices.AddDevice(_backupFile, Microsoft.SqlServer.Management.Smo.DeviceType.File);
            return restore;
        }
    }
}