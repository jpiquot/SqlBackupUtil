#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System;
using System.Collections.Generic;

namespace SqlBackupUtil
{
    internal class SqlBackupSettings
    {
        public List<string>? BackupDirectories { get; set; }
        public List<string>? BackupExtensions { get; set; }
        public BackupRestoreType? BackupType { get; set; }
        public DateTime? Before { get; set; }
        public string? Command { get; set; }
        public string? Database { get; set; }
        public int? DiffFrequency { get; set; }
        public int? FullFrequency { get; set; }
        public bool IncludeSubDirectories { get; set; }
        public bool LatestOnly { get; set; }
        public int? LogFrequency { get; set; }
        public string? Login { get; set; }
        public string? Password { get; set; }
        public string? Server { get; set; }
        public bool Silent { get; set; }
        public string? SourceDatabase { get; set; }
        public string? SourceServer { get; set; }
    }
}