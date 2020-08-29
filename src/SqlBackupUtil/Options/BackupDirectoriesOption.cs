using System.CommandLine;

using Microsoft.Extensions.Options;

namespace SqlBackupUtil
{
    /// <summary>
    /// Backup directories Option.
    /// </summary>
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