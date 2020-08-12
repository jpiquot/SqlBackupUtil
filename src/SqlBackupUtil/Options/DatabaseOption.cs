using System.CommandLine;

using Microsoft.Extensions.Options;

namespace SqlBackupUtil
{
    /// <summary>
    /// Database Option. Implements the <see cref="Option{Int32}"/>
    /// </summary>
    /// <seealso cref="Option{Int32}"/>
    public class DatabaseOption : Option<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseOption"/> class.
        /// </summary>
        /// <param name="defaultValues">The default values.</param>
        public DatabaseOption(IOptions<SqlBackupSettings> defaultValues)
            : base(
                  new[] { "--database", "-d" },
                  () => defaultValues.Value.Database ?? string.Empty,
                  "The SQL Database name.")
        {
        }
    }
}