using System.CommandLine;

using Microsoft.Extensions.Options;

namespace SqlBackupUtil
{
    /// <summary>
    /// Restore command. Implements the <see cref="System.CommandLine.Command"/>
    /// </summary>
    /// <seealso cref="System.CommandLine.Command"/>
    internal class RestoreCommand : Command
    {
        public RestoreCommand(IOptions<SqlBackupSettings> defaultValues)
            : base("restore", "Restore the database.")
        {
            AddAlias("r");
            AddOption(new DatabaseOption(defaultValues));
        }
    }
}