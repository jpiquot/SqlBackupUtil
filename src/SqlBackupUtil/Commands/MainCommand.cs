using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Rendering;

using Microsoft.Extensions.Options;

namespace SqlBackupUtil
{
    /// <summary>
    /// Main command. Implements the <see cref="System.CommandLine.RootCommand"/>
    /// </summary>
    /// <seealso cref="System.CommandLine.RootCommand"/>
    internal class MainCommand : RootCommand
    {
        public MainCommand(IOptions<SqlBackupSettings> defaultValues)
            : base("Sql Server Backup Utility")
        {
            AddGlobalOption(new ServerOption(defaultValues));
            AddGlobalOption(new BackupExtensionsOption(defaultValues));
            AddGlobalOption(new BackupDirectoriesOption(defaultValues));
            AddGlobalOption(new BackupTypeOption(defaultValues));
            AddGlobalOption(new LoginOption(defaultValues));
            AddGlobalOption(new PasswordOption(defaultValues));
            AddGlobalOption(new SourceDatabaseOption(defaultValues));
            AddGlobalOption(new SourceServerOption(defaultValues));

            var command = new CheckCommand(defaultValues)
            {
                Handler =
                CommandHandler.Create<InvocationContext, ConsoleRenderer, CheckOptions>
                (
                    (invocationContext, consoleRenderer, options)
                        => new CheckCommandHandler(invocationContext, consoleRenderer, options).Execute()
                )
            };

            AddCommand(command);
        }
    }
}