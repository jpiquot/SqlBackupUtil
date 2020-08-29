using System.CommandLine;

using Microsoft.Extensions.Options;

namespace SqlBackupUtil
{
    /// <summary>
    /// Backup directories Option. Implements the <see cref="Option{Bool}"/>
    /// </summary>
    /// <seealso cref="Option{Bool}"/>
    internal class LatestOnlyOption : Option<bool>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BeforeOption"/> class.
        /// </summary>
        /// <param name="defaultValues">The default values.</param>
        public LatestOnlyOption(IOptions<SqlBackupSettings> defaultValues)
            : base(
                  new[] { "--latest-only", "-lo" },
                  () => defaultValues.Value.LatestOnly,
                  "Only the last backup files set.")
        {
        }
    }
}