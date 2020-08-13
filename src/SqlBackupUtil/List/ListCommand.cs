using System.CommandLine;

using Microsoft.Extensions.Options;

namespace SqlBackupUtil
{
    /// <summary>
    /// List command. Implements the <see cref="System.CommandLine.Command"/>
    /// </summary>
    /// <seealso cref="System.CommandLine.Command"/>
    internal class ListCommand : Command
    {
        public ListCommand(IOptions<SqlBackupSettings> defaultValues)
            : base("list", "List of the existing database backup files.")
        {
            AddAlias("ls");
            AddOption(new DatabaseOption(defaultValues));
        }
    }
}