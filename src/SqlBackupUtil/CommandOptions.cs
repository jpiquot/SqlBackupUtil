using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace SqlBackupUtil
{

    internal abstract class CommandOptions
    {
        public string Command { get; set; } = string.Empty;
        public string Server { get; set; } = "localhost";

        public List<string> BackupExtensions { get; set; } = new List<string> { "BAK", "TRN" };
        public List<string> BackupDirectories { get; set; } = new List<string> { @"." };
        public BackupTypeOption BackupType { get; set; } = BackupTypeOption.All;

    }
}
