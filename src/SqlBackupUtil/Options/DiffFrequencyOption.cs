using System.CommandLine;

using Microsoft.Extensions.Options;

namespace SqlBackupUtil
{
    /// <summary>
    /// Diff Frequency Option. Implements the <see cref="Option{Int32}"/>
    /// </summary>
    /// <seealso cref="Option{Int32}"/>
    internal class DiffFrequencyOption : Option<int>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiffFrequencyOption"/> class.
        /// </summary>
        /// <param name="defaultValues">The default values.</param>
        public DiffFrequencyOption(IOptions<SqlBackupSettings> defaultValues)
            : base(
                  new[] { "--diff-frequency", "-df" },
                  () => defaultValues.Value.DiffFrequency ?? 24 * 60,
                  "The diff backup frequency in minutes.")
        {
        }
    }
}