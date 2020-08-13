using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Rendering;

using Microsoft.Extensions.Options;

namespace SqlBackupUtil
{
    internal static class Commands
    {
        internal static RootCommand AddCommands(this RootCommand rootCommand, InvocationContext invocationContext, ConsoleRenderer consoleRenderer, IOptions<SqlBackupSettings> defaultValues)
        {
            var command = new Command(
                "check",
                description: "Check if a database backup file exists.");
            command.AddAlias("c");
            command.AddOption(new DatabaseOption(defaultValues));
            command.Handler = CommandHandler.Create<CheckOptions>((options) => (new CheckCommand(invocationContext, consoleRenderer, options)).Execute());
            rootCommand.Add(command);
            command = new Command(
                "restore",
                description: "Restore the database.");
            command.AddAlias("r");
            command
                .AddDatabaseOption()
                .AddSourceOptions();
            command.Handler = CommandHandler.Create<RestoreOptions>((options) => (new RestoreCommand(invocationContext, consoleRenderer, options)).Execute());
            rootCommand.Add(command);
            command = new Command(
                "list",
                description: "List backup files.");
            command.AddAlias("l");
            command
                .AddSourceOptions();
            command.Handler = CommandHandler.Create<ListOptions>((options) => (new ListCommand(invocationContext, consoleRenderer, options)).Execute());
            rootCommand.Add(command);
            return rootCommand;
        }

        internal static CommandLineBuilder CreateBuilder(InvocationContext invocationContext, ConsoleRenderer consoleRenderer)
        {
            // Create a root command with some options
            var rootCommand = new RootCommand("Sql Server Backup Utility");
            rootCommand.AddCommands(invocationContext, consoleRenderer);

            // Parse the incoming args and invoke the handler
            return new CommandLineBuilder(rootCommand);
        }

        private static void Arity<T>(this Option<T> option, int minOccurences, int maxOccurences)
            => option.Argument = new Argument<T>() { Arity = new ArgumentArity(minOccurences, maxOccurences) };

        private static void Required<T>(this Option<T> option)
                                                    => option.Argument = new Argument<T>()
                                                    {
                                                        Arity = ArgumentArity.ExactlyOne
                                                    };
    }
}