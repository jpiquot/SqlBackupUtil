using System.CommandLine;

using Microsoft.Extensions.Options;

namespace SqlBackupUtil
{
    /// <summary>
    /// Log Frequency Option. Implements the <see cref="Option{Int32}"/>
    /// </summary>
    /// <seealso cref="Option{Int32}"/>
    public class LogFrequencyOption : Option<int>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogFrequencyOption"/> class.
        /// </summary>
        /// <param name="defaultValues">The default values.</param>
        public LogFrequencyOption(IOptions<SqlBackupSettings> defaultValues)
            : base(
                  new[] { "--log-frequency", "-lf" },
                  () => defaultValues.Value.LogFrequency ?? 10,
                  "The log backup frequency in minutes.")
        {
        }
    }
}