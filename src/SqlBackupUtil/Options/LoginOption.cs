using System.CommandLine;

using Microsoft.Extensions.Options;

namespace SqlBackupUtil
{
    /// <summary>
    /// Login Option. Implements the <see cref="Option{Int32}"/>
    /// </summary>
    /// <seealso cref="Option{Int32}"/>
    public class LoginOption : Option<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoginOption"/> class.
        /// </summary>
        /// <param name="defaultValues">The default values.</param>
        public LoginOption(IOptions<SqlBackupSettings> defaultValues)
            : base(
                  new[] { "--login", "-l" },
                  () => defaultValues.Value.Login ?? string.Empty,
                  "The SQL server Login name.")
        {
        }
    }
}