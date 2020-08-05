using System;
using System.Collections.Generic;
using System.Data;


using Microsoft.SqlServer.Management.Smo;

namespace SqlBackup.Database
{
    public class BackupFile
    {
        private readonly Server _server;
        private List<BackupDatabaseFile>? _backupDatabaseFiles;
        private readonly string _backupFile;
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

        private List<BackupDatabaseFile> InitBackupDatabaseFiles()
            => CreateList(Restore.ReadFileList(_server), (dict) => new BackupDatabaseFile(dict, _backupFile));
 

        private List<BackupHeader> InitBackupHeaders() 
            => (Restore.Devices.Count > 0)? CreateList(Restore.ReadBackupHeader(_server), (dict) => new BackupHeader(dict, _backupFile)) : new List<BackupHeader>();

        private List<BackupMediaHeader> InitBackupMediaHeaders() 
            => (Restore.Devices.Count > 0) ? CreateList(Restore.ReadMediaHeader(_server), (dict) => new BackupMediaHeader(dict, _backupFile)) : new List<BackupMediaHeader>();

        private List<T> CreateList<T>(DataTable table, Func<Dictionary<string, object>, T> create)
        {
            var list = new List<T>(table.Rows.Count);
            foreach (DataRow? row in table.Rows)
            {
                if (row == null)
                {
                    continue;
                }
                var values = new Dictionary<string, object>();
                foreach (DataColumn? column in table.Columns)
                {
                    if (column == null)
                    {
                        continue;
                    }
                    values.Add(column.ColumnName, row[column.Ordinal]);
                }
                list.Add(create(values));
            }
            return list;
        }

        private Restore InitRestore()
        {
            var restore = new Restore();
            restore.Devices.AddDevice(_backupFile, DeviceType.File);
            return restore;
        }
    }
}