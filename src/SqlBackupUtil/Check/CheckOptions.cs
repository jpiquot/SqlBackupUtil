using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CA1812 // Internal class that is apparently never instantiated.

namespace SqlBackupUtil
{
    internal class CheckOptions : CommandOptions
    {
        public CheckOptions()
        {
            Command = "CHECK";
        }

        public int DiffFrequency { get; set; }
        public int FullFrequency { get; set; }
        public int LogFrequency { get; set; }
    }
}