using System;
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
        private readonly ConsoleRenderer _consoleRenderer;
        private readonly InvocationContext _invocationContext;

        public MainCommand(InvocationContext invocationContext, ConsoleRenderer consoleRenderer, IOptions<SqlBackupSettings> defaultValues)
            : base("Sql Server Backup Utility")
        {
            _invocationContext = invocationContext ?? throw new ArgumentNullException(nameof(invocationContext));
            _consoleRenderer = consoleRenderer ?? throw new ArgumentNullException(nameof(consoleRenderer));
            AddGlobalOption(new ServerOption(defaultValues));
            AddGlobalOption(new BackupExtensionsOption(defaultValues));
            AddGlobalOption(new BackupDirectoriesOption(defaultValues));
            AddGlobalOption(new BackupTypeOption(defaultValues));
            AddGlobalOption(new LoginOption(defaultValues));
            AddGlobalOption(new PasswordOption(defaultValues));
            AddGlobalOption(new SourceDatabaseOption(defaultValues));
            AddGlobalOption(new SourceServerOption(defaultValues));

            Command command = new CheckCommand(defaultValues)
            {
                Handler =
                CommandHandler.Create<CheckOptions>
                (
                    (options)
                        => new CheckCommandHandler(_invocationContext, _consoleRenderer, options).Execute()
                )
            };

            AddCommand(command);

            command = new ListCommand(defaultValues)
            {
                Handler =
                CommandHandler.Create<ListOptions>
                (
                    (options)
                        => new ListCommandHandler(_invocationContext, _consoleRenderer, options).Execute()
                )
            };

            AddCommand(command);
            command = new RestoreCommand(defaultValues)
            {
                Handler =
                CommandHandler.Create<RestoreOptions>
                (
                    (options)
                        => new RestoreCommandHandler(_invocationContext, _consoleRenderer, options).Execute()
                )
            };

            AddCommand(command);
        }
    }
}