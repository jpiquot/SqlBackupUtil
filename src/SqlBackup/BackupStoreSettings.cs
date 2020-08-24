using System.Collections.Generic;

namespace SqlBackup
{
    public class BackupStoreSettings
    {
        public List<string> BackupFileExtensions { get; set; } = new List<string>();
        public List<string> BackupPaths { get; set; } = new List<string>();
        public string? Login { get; set; }
        public string? Password { get; set; }
        public bool IncludeSubDirectories { get; set; }
    }
}