using System.CommandLine;

using Microsoft.Extensions.Options;

namespace SqlBackupUtil
{
    /// <summary>
    /// Password Option. Implements the <see cref="Option{Int32}"/>
    /// </summary>
    /// <seealso cref="Option{Int32}"/>
    public class PasswordOption : Option<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordOption"/> class.
        /// </summary>
        /// <param name="defaultValues">The default values.</param>
        public PasswordOption(IOptions<SqlBackupSettings> defaultValues)
            : base(
                  new[] { "--password", "-p" },
                  () => defaultValues.Value.Password ?? string.Empty,
                  "The SQL server Password name.")
        {
        }
    }
}