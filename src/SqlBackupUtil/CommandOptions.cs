#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Collections.Generic;

namespace SqlBackupUtil
{
    internal abstract class CommandOptions
    {
        public List<string> BackupDirectories { get; set; } = new List<string> { @"." };
        public List<string> BackupExtensions { get; set; } = new List<string> { "BAK", "TRN" };
        public BackupTypeOption BackupType { get; set; } = BackupTypeOption.All;
        public string Command { get; set; } = string.Empty;
        public string? Login { get; set; }
        public string? Password { get; set; }
        public string Server { get; set; } = "localhost";
    }
}