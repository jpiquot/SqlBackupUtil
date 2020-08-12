using System.CommandLine;

using Microsoft.Extensions.Options;

namespace SqlBackupUtil
{
    /// <summary>
    /// Server Option. Implements the <see cref="Option{Int32}"/>
    /// </summary>
    /// <seealso cref="Option{Int32}"/>
    public class ServerOption : Option<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerOption"/> class.
        /// </summary>
        /// <param name="defaultValues">The default values.</param>
        public ServerOption(IOptions<SqlBackupSettings> defaultValues)
            : base(
                  new[] { "--server", "-s" },
                  () => defaultValues.Value.Server ?? string.Empty,
                  "The name of the SQL Server used for the operation.")
        {
        }
    }
}