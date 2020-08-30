using System.CommandLine;

using Microsoft.Extensions.Options;

namespace SqlBackupUtil
{
    /// <summary>
    /// Silent mode Option. Implements the <see cref="Option{Bool}"/>
    /// </summary>
    /// <seealso cref="Option{Bool}"/>
    internal class SilentOption : Option<bool>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BeforeOption"/> class.
        /// </summary>
        /// <param name="defaultValues">The default values.</param>
        public SilentOption(IOptions<SqlBackupSettings> defaultValues)
            : base(
                  new[] { "--silent" },
                  () => defaultValues.Value.Silent,
                  "Silent mode. Do not write on console.")
        {
        }
    }
}