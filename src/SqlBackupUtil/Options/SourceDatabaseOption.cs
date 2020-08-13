using System.CommandLine;

using Microsoft.Extensions.Options;

namespace SqlBackupUtil
{
    /// <summary>
    /// Source Database Option. Implements the <see cref="Option{Int32}"/>
    /// </summary>
    /// <seealso cref="Option{Int32}"/>
    internal class SourceDatabaseOption : Option<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SourceDatabaseOption"/> class.
        /// </summary>
        /// <param name="defaultValues">The default values.</param>
        public SourceDatabaseOption(IOptions<SqlBackupSettings> defaultValues)
            : base(
                  new[] { "--source-database", "-sd" },
                  () => defaultValues.Value.SourceDatabase ?? string.Empty,
                  "The source SQL Database name.")
        {
        }
    }
}