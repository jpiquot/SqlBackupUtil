#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

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