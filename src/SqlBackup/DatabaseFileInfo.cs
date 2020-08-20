using System;
using System.Collections.Generic;

namespace SqlBackup
{
    public class DatabaseFileInfo
    {
        private readonly Dictionary<string, object> _values;

        public DatabaseFileInfo(Dictionary<string, object> values)
        {
            _values = values ?? throw new ArgumentNullException(nameof(values));
        }

        public FileType FileType => GetType((string)_values["Type"]);
        public string LogicalName => (string)_values[nameof(LogicalName)];
        public string PhysicalName => (string)_values[nameof(PhysicalName)];

        public Dictionary<string, object> Values => new Dictionary<string, object>(_values);

        private static FileType GetType(string type) => type switch
        {
            "D" => FileType.Data,
            "L" => FileType.Log,
            _ => throw new NotSupportedException(string.Format(Properties.Resources.BackupFileTypeUnsupported, type))
        };
    }
}