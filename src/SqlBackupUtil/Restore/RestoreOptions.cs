using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace SqlBackupUtil
{
    internal class RestoreOptions : CommandOptions
    {
        public RestoreOptions()
        {
            Command = "RESTORE";
        }

        public string Database { get; set; } = string.Empty;
    }
}