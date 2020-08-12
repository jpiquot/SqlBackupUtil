using System.CommandLine;

using Microsoft.Extensions.Options;

namespace SqlBackupUtil
{
    /// <summary>
    /// Source Server Option. Implements the <see cref="Option{Int32}"/>
    /// </summary>
    /// <seealso cref="Option{Int32}"/>
    public class SourceServerOption : Option<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SourceServerOption"/> class.
        /// </summary>
        /// <param name="defaultValues">The default values.</param>
        public SourceServerOption(IOptions<SqlBackupSettings> defaultValues)
            : base(
                  new[] { "--source-server", "-ss" },
                  () => defaultValues.Value.SourceServer ?? string.Empty,
                  "The source SQL Server name.")
        {
        }
    }
}