using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace SqlBackupUtil
{

    internal class UtilityOptions
    {
        public int FullFrequency { get; set; }
        public int DiffFrequency { get; set; }
        public int LogFrequency { get; set; }
        public string? SourceServer { get; set; }
        public string? SourceDatabase { get; set; }
        public string? DestinationServer { get; set; }
        public string? DestinationDatabase { get; set; }

        public string? BackupExtensions { get; set; }
        public string? BackupDirectories { get; set; }
        public BackupTypeOption BackupType { get; set; }

    }
}
