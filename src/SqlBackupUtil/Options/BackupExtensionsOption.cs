using System.CommandLine;

using Microsoft.Extensions.Options;

namespace SqlBackupUtil
{
    /// <summary>
    /// Backup extensions Option. Implements the <see cref="Option{Int32}"/>
    /// </summary>
    /// <seealso cref="Option{Int32}"/>
    internal class BackupExtensionsOption : Option<string[]>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BackupExtensionsOption"/> class.
        /// </summary>
        /// <param name="defaultValues">The default values.</param>
        public BackupExtensionsOption(IOptions<SqlBackupSettings> defaultValues)
            : base(
                  new[] { "--backup-extensions", "-be" },
                  () => defaultValues.Value.BackupExtensions?.ToArray() ?? new[] { "BAK", "TRN" },
                  "The backup files extensions.")
        {
        }
    }
}