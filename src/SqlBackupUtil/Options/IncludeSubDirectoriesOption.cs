using System.CommandLine;

using Microsoft.Extensions.Options;

namespace SqlBackupUtil
{
    /// <summary>
    /// Include the sub directories Option. Implements the <see cref="Option{Int32}"/>
    /// </summary>
    /// <seealso cref="Option{Int32}"/>
    internal class IncludeSubDirectoriesOption : Option<bool>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IncludeSubDirectoriesOption"/> class.
        /// </summary>
        /// <param name="defaultValues">The default values.</param>
        public IncludeSubDirectoriesOption(IOptions<SqlBackupSettings> defaultValues)
            : base(
                  new[] { "--include-sub-directories", "-is" },
                  () => defaultValues.Value.IncludeSubDirectories,
                  "Include the sub directories in the backup file search.")
        {
        }
    }
}