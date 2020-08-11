using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Rendering;


namespace SqlBackupUtil
{
    internal static class Commands
    {
        private static Command AddFrequencyOptions(this Command command)
        {
            var option = new Option<int>
                (
                    "--full-frequency",
                    () => 7 * 24 * 60,
                    "The full backup frequency in minutes."
                );
            option.AddAlias("-ff");
            command.Add(option);

            option = new Option<int>(
                    "--diff-frequency",
                    () => 24 * 60,
                    "The full backup frequency in minutes.");
            option.AddAlias("-df");
            command.Add(option);

            option = new Option<int>(
                    "--log-frequency",
                    () => 10,
                    "The full backup frequency in minutes.");
            option.AddAlias("-lf");
            command.Add(option);
            return command;
        }
        private static void Required<T>(this Option<T> option)
            => option.Argument = new Argument<T>()
            {
                Arity = ArgumentArity.ExactlyOne
            };
        private static void Arity<T>(this Option<T> option, int minOccurences, int maxOccurences)
            => option.Argument = new Argument<T>() { Arity = new ArgumentArity(minOccurences, maxOccurences) };
        private static Command AddSourceOptions(this Command command, bool required = false)
        {
            var option = new Option<string>(
                    "--source-server",
                    "The source SQL Server name.");
            option.AddAlias("-ss");
            if (required)
            {
                option.Required();
            }
            command.Add(option);

            option = new Option<string>(
                    "--source-database",
                    "The source database name.");
            option.AddAlias("-sd");
            if (required)
            {
                option.Required();
            }
            command.Add(option);
            return command;

        }
        private static RootCommand AddServerOption(this RootCommand command)
        {
            var option = new Option<string>(
                    "--server",
                    () => "localhost",
                    "The name of the SQL Server used for the operation.");
            option.AddAlias("-s");
            command.AddGlobalOption(option);
            return command;
        }
        private static Command AddDatabaseOption(this Command command)
        {

            var option = new Option<string>(
                    "--database",
                    "The database name.");
            option.AddAlias("-d");
            command.Add(option);
            return command;
        }
        private static RootCommand AddBackupFilesOptions(this RootCommand command)
        {
            var option = new Option<List<string>>(
                    "--backup-extensions",
                    () => new List<string> { "BAK", "TRN" },
                    "The backup files extensions.");
            option.AddAlias("-be");
            command.AddGlobalOption(option);

            option = new Option<List<string>>(
                    "--backup-directories",
                    () => new List<string> { "." },
                    "The backup directories.");
            option.AddAlias("-bd");
            command.AddGlobalOption(option);
            return command;
        }
        private static RootCommand AddBackupTypeOption(this RootCommand command)
        {
            var option = new Option<BackupTypeOption>(
                    "--backup-type",
                    () => BackupTypeOption.All,
                    "The backup type.");
            option.AddAlias("-bt");
            command.AddGlobalOption(option);
            return command;
        }
        internal static CommandLineBuilder CreateBuilder(InvocationContext invocationContext, ConsoleRenderer consoleRenderer)
        {
            // Create a root command with some options
            var rootCommand = new RootCommand("Sql Server Backup Utility");
            rootCommand.AddCommands(invocationContext, consoleRenderer);

            // Parse the incoming args and invoke the handler
            return new CommandLineBuilder(rootCommand);
        }

        internal static RootCommand AddCommands(this RootCommand rootCommand, InvocationContext invocationContext, ConsoleRenderer consoleRenderer)
        {
            rootCommand
                .AddServerOption()
                .AddBackupFilesOptions()
                .AddBackupTypeOption();
            var command = new Command(
                "check",
                description: "Check if a database backup file exists.");
            command.AddAlias("c");
            command
                .AddSourceOptions(true)
                .AddFrequencyOptions();
            command.Handler = CommandHandler.Create<CheckOptions>((options) => (new CheckCommand(invocationContext, consoleRenderer, options)).Execute());
            rootCommand.Add(command);
            command = new Command(
                "restore",
                description: "Restore the database.");
            command.AddAlias("r");
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
    }
}
