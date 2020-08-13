using System.CommandLine;

using Microsoft.Extensions.Options;

namespace SqlBackupUtil
{
    /// <summary>
    /// Full Frequency Option. Implements the <see cref="Option{Int32}"/>
    /// </summary>
    /// <seealso cref="Option{Int32}"/>
    internal class FullFrequencyOption : Option<int>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FullFrequencyOption"/> class.
        /// </summary>
        /// <param name="defaultValues">The default values.</param>
        public FullFrequencyOption(IOptions<SqlBackupSettings> defaultValues)
            : base(
                  new[] { "--full-frequency", "-ff" },
                  () => defaultValues.Value.FullFrequency ?? 7 * 24 * 60,
                  "The full backup frequency in minutes.")
        {
        }
    }
}