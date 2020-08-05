using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlBackup.Database
{
    public class BackupStoreSettings
    {
        public List<string> BackupFileExtensions { get; set; } = new List<string>();
        public List<string> BackupPaths { get; set; } = new List<string>();
    }
}