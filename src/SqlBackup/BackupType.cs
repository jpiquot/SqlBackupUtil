using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlBackup.Database
{
    public enum BackupType
    {
        NotSupported,
        Full,
        Differential,
        Log
    }
}