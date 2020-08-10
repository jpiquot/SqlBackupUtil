using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace SqlBackupUtil
{

    internal class CheckOptions : CommandOptions
    {
        public CheckOptions()
        {
            Command = "CHECK";
        }
        public string SourceServer { get; set; } = string.Empty;
        public string SourceDatabase { get; set; } = string.Empty;

        public int FullFrequency { get; set; }
        public int DiffFrequency { get; set; }
        public int LogFrequency { get; set; }
    }
}
