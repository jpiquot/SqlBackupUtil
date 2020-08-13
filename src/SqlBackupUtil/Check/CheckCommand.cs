using System.CommandLine;

using Microsoft.Extensions.Options;

namespace SqlBackupUtil
{
    /// <summary>
    /// Check command. Implements the <see cref="System.CommandLine.Command"/>
    /// </summary>
    /// <seealso cref="System.CommandLine.Command"/>
    internal class CheckCommand : Command
    {
        public CheckCommand(IOptions<SqlBackupSettings> defaultValues)
            : base("check", "Check if a database backup file exists.")
        {
            AddAlias("c");
            AddOption(new DatabaseOption(defaultValues));
        }
    }
}