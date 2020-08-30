#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Collections.Generic;

namespace SqlBackupUtil
{
    internal abstract class CommandOptions
    {
        public List<string> BackupDirectories { get; set; } = new List<string> { @"." };
        public List<string> BackupExtensions { get; set; } = new List<string> { "BAK", "TRN" };
        public BackupTypes BackupType { get; set; } = BackupTypes.All;
        public DateTime? Before { get; set; }
        public string Command { get; set; } = string.Empty;
        public bool IncludeSubDirectories { get; set; }
        public string? Login { get; set; }
        public string? Password { get; set; }
        public string Server { get; set; } = "localhost";
        public bool Silent { get; set; }
        public string? SourceDatabase { get; set; }
        public string? SourceServer { get; set; }
    }
}