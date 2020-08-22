namespace SqlBackup
{
    public class SqlDirTree
    {
        public int Depth { get; set; }
        public bool File { get; set; }
        public string SubDirectory { get; set; } = string.Empty;
    }
}