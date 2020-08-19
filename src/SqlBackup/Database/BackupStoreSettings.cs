using System.Collections.Generic;

namespace SqlBackup.Database
{
    public class BackupStoreSettings
    {
        public List<string> BackupFileExtensions { get; set; } = new List<string>();
        public List<string> BackupPaths { get; set; } = new List<string>();
        public string? Login { get; set; }
        public string? Password { get; set; }
    }
}