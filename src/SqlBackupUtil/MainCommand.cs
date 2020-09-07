using System.CommandLine;
using System.CommandLine.Invocation;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
            AddGlobalOption(new IncludeSubDirectoriesOption(defaultValues));
            AddGlobalOption(new BackupDirectoriesOption(defaultValues));
            AddGlobalOption(new BackupTypeOption(defaultValues));
            AddGlobalOption(new LoginOption(defaultValues));
            AddGlobalOption(new PasswordOption(defaultValues));
            AddGlobalOption(new SourceDatabaseOption(defaultValues));
            AddGlobalOption(new SourceServerOption(defaultValues));
            AddGlobalOption(new BeforeOption(defaultValues));
            AddGlobalOption(new SilentOption(defaultValues));

            Command command = new CheckCommand(defaultValues)
            {
                Handler = CommandHandler.Create<IHost, CheckOptions>((host, options) =>
                {
                    new CheckCommandHandler(
                            host.Services,
                            options
                        )
                        .InvokeAsync(host.Services.GetRequiredService<InvocationContext>())
                        .GetAwaiter()
                        .GetResult();
                })
            };
            AddCommand(command);

            command = new ListCommand(defaultValues)
            {
                Handler = CommandHandler.Create<IHost, ListOptions>((host, options) =>
                {
                    new ListCommandHandler(
                            host.Services,
                            options
                        )
                        .InvokeAsync(host.Services.GetRequiredService<InvocationContext>())
                        .GetAwaiter()
                        .GetResult();
                })
            };
            AddCommand(command);

            command = new RestoreCommand(defaultValues)
            {
                Handler = CommandHandler.Create<IHost, RestoreOptions>((host, options) =>
                {
                    new RestoreCommandHandler(
                            host.Services,
                            options
                        )
                        .InvokeAsync(host.Services.GetRequiredService<InvocationContext>())
                        .GetAwaiter()
                        .GetResult();
                })
            };
            AddCommand(command);
        }
    }
}