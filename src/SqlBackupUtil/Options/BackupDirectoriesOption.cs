using System.CommandLine;

using Microsoft.Extensions.Options;

namespace SqlBackupUtil
{
    /// <summary>
    /// Backup directories Option. Implements the <see cref="Option{Int32}"/>
    /// </summary>
    /// <seealso cref="Option{Int32}"/>
    internal class BackupDirectoriesOption : Option<string[]>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BackupDirectoriesOption"/> class.
        /// </summary>
        /// <param name="defaultValues">The default values.</param>
        public BackupDirectoriesOption(IOptions<SqlBackupSettings> defaultValues)
            : base(
                  new[] { "--backup-directories", "-bd" },
                  () => defaultValues.Value.BackupDirectories?.ToArray() ?? new[] { "." },
                  "The backup files directories.")
        {
        }
    }
}