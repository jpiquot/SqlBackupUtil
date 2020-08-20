using System;
using System.Collections.Generic;

namespace SqlBackup.Database
{
    public class BackupDatabaseFile
    {
        private Dictionary<string, object> _values;

        public BackupDatabaseFile(Dictionary<string, object> values, string fileName)
        {
            _values = values ?? throw new ArgumentNullException(nameof(values));
            FileName = fileName;
        }

        public string LogicalName => (string)_values["LogicalName"];
        public string PhysicalName => (string)_values["PhysicalName"];

        public string FileName { get; }
    }
}