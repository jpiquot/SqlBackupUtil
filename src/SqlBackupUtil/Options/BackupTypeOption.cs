using System.CommandLine;

using Microsoft.Extensions.Options;

namespace SqlBackupUtil
{
    /// <summary>
    /// Backup type Option. Implements the <see cref="Option{Int32}"/>
    /// </summary>
    /// <seealso cref="Option{Int32}"/>
    internal class BackupTypeOption : Option<BackupTypes>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BackupTypeOption"/> class.
        /// </summary>
        /// <param name="defaultValues">The default values.</param>
        public BackupTypeOption(IOptions<SqlBackupSettings> defaultValues)
            : base(
                  new[] { "--backup-type", "-bt" },
                  () => defaultValues.Value.BackupType ?? BackupTypes.All,
                  "The backup files type.")
        {
        }
    }
}