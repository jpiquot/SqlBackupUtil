using System;
using System.Collections.Generic;

namespace SqlBackup
{
    public class DatabaseFileInfo
    {
        private readonly Dictionary<string, object> _values;

        public DatabaseFileInfo(Dictionary<string, object> values)
        {
            _values = values ?? throw new ArgumentNullException(nameof(values));
        }

        public long BackupSizeInBytes => (long)_values[nameof(BackupSizeInBytes)];
        public decimal CreateLSN => (decimal)_values[nameof(CreateLSN)];
        public Guid? DifferentialBaseGUID => (Guid?)_values[nameof(DifferentialBaseGUID)];
        public decimal? DifferentialBaseLSN => (decimal?)_values[nameof(DifferentialBaseLSN)];
        public decimal DropLSN => (decimal)_values[nameof(DropLSN)];
        public int FileGroupId => (int)_values[nameof(FileGroupId)];
        public string FileGroupName => (string)_values[nameof(FileGroupName)];
        public long FileId => (long)_values[nameof(FileId)];
        public FileType FileType => GetType((string)_values["Type"]);
        public bool IsPresent => (bool)_values[nameof(IsPresent)];
        public bool IsReadOnly => (bool)_values[nameof(IsReadOnly)];
        public Guid? LogGroupGUID => (Guid?)_values[nameof(LogGroupGUID)];
        public string LogicalName => (string)_values[nameof(LogicalName)];
        public long MaxSize => (long)_values[nameof(MaxSize)];
        public string PhysicalName => (string)_values[nameof(PhysicalName)];
        public decimal? ReadOnlyLSN => (decimal?)_values[nameof(ReadOnlyLSN)];
        public decimal? ReadWriteLSN => (decimal?)_values[nameof(ReadWriteLSN)];
        public long Size => (long)_values[nameof(Size)];
        public string? SnapshotURL => (string?)_values[nameof(SnapshotURL)];
        public int SourceBlockSize => (int)_values[nameof(SourceBlockSize)];
        public byte[]? TDEThumbprint => (byte[]?)_values[nameof(TDEThumbprint)];
        public Guid UniqueId => (Guid)_values[nameof(UniqueId)];
        public Dictionary<string, object> Values => new Dictionary<string, object>(_values);

        private static FileType GetType(string type) => type switch
        {
            "D" => FileType.Data,
            "L" => FileType.Log,
            "F" => FileType.FullText,
            "S" => FileType.Stream,
            _ => throw new NotSupportedException(string.Format(Properties.Resources.BackupFileTypeUnsupported, type))
        };
    }
}