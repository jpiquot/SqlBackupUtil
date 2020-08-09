using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace SqlBackupUtil
{

    internal class ListOptions
    {
        public string Server { get; set; } = "localhost";

        public string? SourceServer { get; set; }
        public string? SourceDatabase { get; set; }
        public List<string> BackupExtensions { get; set; } = new List<string> { "BAK", "TRN" };
        public List<string> BackupDirectories { get; set; } = new List<string> { @"." };
        public BackupTypeOption BackupType { get; set; } = BackupTypeOption.All;

    }
}
