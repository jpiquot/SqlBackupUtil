using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SqlBackup
{
    public class BackupHeader
    {
        private readonly Dictionary<string, object> _values;

        public BackupHeader(Dictionary<string, object> values, string fileName)
        {
            _values = values ?? throw new ArgumentNullException(nameof(values));
            FileName = fileName;
        }

        public string BackupDescription => (string)_values[nameof(BackupDescription)];
        public DateTime BackupFinishDate => (DateTime)_values[nameof(BackupFinishDate)];
        public string BackupName => (string)_values[nameof(BackupName)];
        public Guid? BackupSetGUID => (Guid?)_values[nameof(BackupSetGUID)];
        public decimal BackupSize => (decimal)_values[nameof(BackupSize)];
        public DateTime BackupStartDate => (DateTime)_values[nameof(BackupStartDate)];
        public BackupType BackupType => (BackupType)(byte)_values[nameof(BackupType)];
        public string BackupTypeDescription => (string)_values[nameof(BackupTypeDescription)];
        public bool BeginsLogChain => (bool)_values[nameof(BeginsLogChain)];
        public Guid BindingId => (Guid)_values[nameof(BindingId)];
        public decimal CheckpointLSN => (decimal)_values[nameof(CheckpointLSN)];
        public short CodePage => (short)_values[nameof(CodePage)];
        public string Collation => (string)_values[nameof(Collation)];
        public short CompatibilityLevel => (short)_values[nameof(CompatibilityLevel)];
        public bool Compressed => (bool)_values[nameof(Compressed)];
        public long CompressedBackupSize => (long)_values[nameof(CompressedBackupSize)];
        public short Containement => (short)_values[nameof(Containement)];
        public decimal DatabaseBackupLSN => (decimal)_values[nameof(DatabaseBackupLSN)];
        public DateTime DatabaseCreationDate => (DateTime)_values[nameof(DatabaseCreationDate)];
        public string DatabaseName => (string)_values[nameof(DatabaseName)];
        public int DatabaseVersion => (int)_values[nameof(DatabaseVersion)];
        public DeviceType DeviceType => (DeviceType)(byte)_values[nameof(DeviceType)];
        public Guid? DifferentialBaseGUID => (Guid?)_values[nameof(DifferentialBaseGUID)];
        public decimal? DifferentialBaseLSN => (decimal?)_values[nameof(DifferentialBaseLSN)];
        public ReadOnlyCollection<byte>? EncryptorThumbprint => new ReadOnlyCollection<byte>((byte[])(_values[nameof(EncryptorThumbprint)] ?? Array.Empty<byte>()));
        public string EncryptorType => (string)_values[nameof(EncryptorType)];
        public DateTime? ExpirationDate => (DateTime?)_values[nameof(ExpirationDate)];
        public Guid FamilyGUID => (Guid)_values[nameof(FamilyGUID)];
        public string FileName { get; }
        public decimal FirstLSN => (decimal)_values[nameof(FirstLSN)];
        public Guid FirstRecoveryForkID => (Guid)_values[nameof(FirstRecoveryForkID)];
        public decimal? ForkPointLSN => (decimal?)_values[nameof(ForkPointLSN)];
        public bool HasBackupChecksums => (bool)_values[nameof(HasBackupChecksums)];
        public bool HasBulkLoggedData => (bool)_values[nameof(HasBulkLoggedData)];
        public bool HasIncompleteMetaData => (bool)_values[nameof(HasIncompleteMetaData)];
        public bool IsCopyOnly => (bool)_values[nameof(IsCopyOnly)];
        public bool IsDamaged => (bool)_values[nameof(IsDamaged)];
        public bool IsForceOffline => (bool)_values[nameof(IsForceOffline)];
        public bool IsReadOnly => (bool)_values[nameof(IsReadOnly)];
        public bool IsSingleUser => (bool)_values[nameof(IsSingleUser)];
        public bool IsSnapshot => (bool)_values[nameof(IsSnapshot)];
        public string KeyAlgorithm => (string)_values[nameof(KeyAlgorithm)];
        public decimal LastLSN => (decimal)_values[nameof(LastLSN)];
        public string MachineName => (string)_values[nameof(MachineName)];
        public short Position => (short)_values[nameof(Position)];
        public Guid RecoveryForkId => (Guid)_values[nameof(RecoveryForkId)];
        public string RecoveryModel => (string)_values[nameof(RecoveryModel)];
        public string ServerName => (string)_values[nameof(ServerName)];
        public int SoftwareVendorBuild => (int)_values[nameof(SoftwareVendorBuild)];
        public int SoftwareVendorId => (int)_values[nameof(SoftwareVendorId)];
        public int SoftwareVendorMajor => (int)_values[nameof(SoftwareVendorMajor)];
        public int SoftwareVendorMinor => (int)_values[nameof(SoftwareVendorMinor)];
        public int SoftwareVersionMajor => (int)_values[nameof(SoftwareVersionMajor)];
        public short SortOrder => (short)_values[nameof(SortOrder)];
        public DateTime StartDate => (DateTime)_values[nameof(BackupStartDate)];
        public int UnicodeComparaisonStyle => (int)_values[nameof(UnicodeComparaisonStyle)];
        public int UnicodeLocaleId => (int)_values[nameof(UnicodeLocaleId)];
        public string UserName => (string)_values[nameof(UserName)];
        public Dictionary<string, object> Values => new Dictionary<string, object>(_values);
    }
}