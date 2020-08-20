using System;
using System.Collections.Generic;

namespace SqlBackup.Database
{
    public class DatabaseFileInfo
    {
        private readonly Dictionary<string, object> _values;

        public DatabaseFileInfo(Dictionary<string, object> values, string fileName)
        {
            _values = values ?? throw new ArgumentNullException(nameof(values));
            FileName = fileName;
        }

        public string BackupName => (string)_values["BackupName"];
        public BackupType BackupType => GetType((byte)_values["BackupType"]);
        public decimal CheckpointLSN => (decimal)_values["CheckpointLSN"];
        public decimal DatabaseBackupLSN => (decimal)_values["DatabaseBackupLSN"];
        public string DatabaseName => (string)_values["DatabaseName"];
        public string FileName { get; }
        public DateTime FinishDate => (DateTime)_values["BackupFinishDate"];
        public decimal FirstLSN => (decimal)_values["FirstLSN"];
        public decimal LastLSN => (decimal)_values["LastLSN"];
        public int Position => (short)_values["Position"];
        public string ServerName => (string)_values["ServerName"];
        public int SoftwareVersionMajor => (int)_values["SoftwareVersionMajor"];
        public DateTime StartDate => (DateTime)_values["BackupStartDate"];

        public Dictionary<string, object> Values => new Dictionary<string, object>(_values);

        private static BackupType GetType(short type) => type switch
        {
            1 => BackupType.Full,
            5 => BackupType.Differential,
            2 => BackupType.Log,
            _ => BackupType.NotSupported
        };
    }
}