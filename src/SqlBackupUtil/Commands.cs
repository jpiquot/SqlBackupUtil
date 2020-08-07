using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

using SqlBackup.Database;

namespace SqlBackupUtil
{
    internal static class Commands
    {
        private static Command AddFrequencyOptions (this Command command)
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
        private static Command AddSourceOptions(this Command command)
        {
            var option = new Option<string>(
                    "--source-server",
                    "The source SQL Server name.");
            option.AddAlias("-ss");
            command.Add(option);

            option = new Option<string>(
                    "--source-database",
                    "The source database name.");
            option.AddAlias("-sd");
            command.Add(option);
            return command;

        }
        private static Command AddServerOption(this Command command)
        {
            var option = new Option<string>(
                    "--server",
                    () => "localhost",
                    "The name of the SQL Server used for the operation.");
            option.AddAlias("-s");
            command.Add(option);
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
        private static Command AddBackupFilesOptions(this Command command)
        {
            var option = new Option<List<string>>(
                    "--backup-extensions",
                    () => new List<string> { "BAK", "TRN" },
                    "The backup files extensions.");
            option.AddAlias("-be");
            command.Add(option);

            option = new Option<List<string>>(
                    "--backup-directories",
                    () => new List<string> { "." },
                    "The backup directories.");
            option.AddAlias("-bd");
            command.Add(option);
            return command;
        }
        private static Command AddBackupTypeOption(this Command command)
        {
            var option = new Option<BackupTypeOption>(
                    "--backup-type",
                    () => BackupTypeOption.All,
                    "The backup type.");
            option.AddAlias("-bt");
            command.Add(option);
            return command;
        }
        internal static CommandLineBuilder CreateBuilder(InvocationContext invocationContext)
        {
            // Create a root command with some options
            var rootCommand = new RootCommand("Sql Server Backup Utility");
            rootCommand.AddCommands(invocationContext);
               
            // Parse the incoming args and invoke the handler
            return new CommandLineBuilder(rootCommand);
        }
 
        internal static RootCommand AddCommands(this RootCommand rootCommand, InvocationContext invocationContext)
        {
            var command = new Command(
                "check",
                description: "Check if a database backup file exists.");
            rootCommand.Add(command);
            command = new Command(
                "restore",
                description: "Restore the database.");
            rootCommand.Add(command);
            command = new Command(
                "list",
                description: "List backup files.");
            command
                .AddServerOption()
                .AddBackupFilesOptions()
                .AddBackupTypeOption();
            command.Handler = CommandHandler.Create<ListOptions>((options) => (new ListCommand(invocationContext,options)).Execute());
            rootCommand.Add(command);
            return rootCommand;
        }
    }
}
