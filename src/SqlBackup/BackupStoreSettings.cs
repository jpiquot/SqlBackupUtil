using System;
using System.Collections.Generic;

namespace SqlBackup
{
    public class BackupStoreSettings
    {
        public BackupStoreSettings()
        {
            BackupFileExtensions = new List<string>();
            BackupPaths = new List<string>();
        }

        public BackupStoreSettings(List<string> backupFileExtensions, List<string> backupPaths, string? login, string? password, bool includeSubDirectories)
        {
            BackupFileExtensions = backupFileExtensions;
            BackupPaths = backupPaths;
            Login = login;
            Password = password;
            IncludeSubDirectories = includeSubDirectories;
        }

        public List<string> BackupFileExtensions { get; }
        public List<string> BackupPaths { get; }
        public bool IncludeSubDirectories { get; }
        public string? Login { get; }
        public string? Password { get; }
    }
}