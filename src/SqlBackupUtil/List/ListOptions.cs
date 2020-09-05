#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CA1812 // Internal class that is apparently never instantiated.

namespace SqlBackupUtil
{
    internal class ListOptions : CommandOptions
    {
        public ListOptions()
        {
            Command = "LIST";
        }

        public bool LatestOnly { get; set; }
    }
}