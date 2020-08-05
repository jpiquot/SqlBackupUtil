using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.SqlServer.Management.Smo;

namespace SqlBackup.Database
{
    public class BackupHeader
    {
        private readonly Dictionary<string, object> _values;

        public BackupHeader(Dictionary<string, object> values, string fileName)
        {
            _values = values ?? throw new ArgumentNullException(nameof(values));
            FileName = fileName;
        }

        public string BackupName => (string)_values["BackupName"];
        public BackupType BackupType => GetType((byte)_values["BackupType"]);
        public string DatabaseName => (string)_values["DatabaseName"];
        public string ServerName => (string)_values["ServerName"];
        public DateTime FinishDate => (DateTime)_values["BackupFinishDate"];
        public decimal FirstLSN => (decimal)_values["FirstLSN"];
        public decimal LastLSN => (decimal)_values["LastLSN"];
        public decimal DatabaseBackupLSN => (decimal)_values["DatabaseBackupLSN"];
        public decimal CheckpointLSN => (decimal)_values["CheckpointLSN"];
        public int Position => (short)_values["Position"];
        public int SoftwareVersionMajor => (int)_values["SoftwareVersionMajor"];
        public DateTime StartDate => (DateTime)_values["BackupStartDate"];

        public Dictionary<string, object> Values => new Dictionary<string, object>(_values);

        public string FileName { get; }

        private static BackupType GetType(short type) => type switch
        {
            1 => BackupType.Full,
            5 => BackupType.Differential,
            3 => BackupType.Log,
            _ => BackupType.NotSupported
        };
    }
}