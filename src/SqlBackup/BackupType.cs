namespace SqlBackup
{
    public enum BackupType
    {
        Full = 1,
        Log = 2,
        File = 4,
        Differential = 5,
        DifferentialFile = 6,
        Partial = 7,
        DifferentialPartial = 8
    }
}