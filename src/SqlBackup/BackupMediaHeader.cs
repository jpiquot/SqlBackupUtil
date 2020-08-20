using System;
using System.Collections.Generic;

namespace SqlBackup.Database
{
    public class BackupMediaHeader
    {
        private Dictionary<string, object> _values;

        public BackupMediaHeader(Dictionary<string, object> values, string fileName)
        {
            _values = values ?? throw new ArgumentNullException(nameof(values));
            FileName = fileName;
        }

        public Guid MediaSetId => (Guid)_values["MediaSetId"];
        public DateTime MediaDate => (DateTime)_values["MediaDate"];

        public string FileName { get; }
    }
}