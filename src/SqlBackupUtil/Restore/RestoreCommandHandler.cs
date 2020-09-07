using System;
using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.CommandLine.Rendering.Views;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using SqlBackup;

#pragma warning disable CA1812 // Internal class that is apparently never instantiated.

namespace SqlBackupUtil
{
    /// <summary>
    /// Handle the restore command
    /// </summary>
    internal class RestoreCommandHandler : CommandHandler<RestoreCommandHandler, RestoreOptions>
    {
        public RestoreCommandHandler(IServiceProvider serviceProvider, RestoreOptions options) : base(serviceProvider, options)
        {
        }

        /// <summary>
        /// Execute the list command
        /// </summary>
        public override Task<int> InvokeAsync(InvocationContext context)
        {
            var store = new BackupStore(Options.Server, Microsoft.Extensions.Options.Options.Create(Options.GetBackupStoreSettings()));
            _ = Options.SourceServer ?? throw new NotSupportedException("The source server must be defined.");
            _ = Options.SourceDatabase ?? throw new NotSupportedException("The source database must be defined.");
            IEnumerable<BackupHeader> backups = store.GetLatestBackupSet
                (
                Options.SourceServer,
                Options.SourceDatabase,
                Options.BackupType == BackupRestoreType.Diff || Options.BackupType == BackupRestoreType.All,
                Options.BackupType == BackupRestoreType.Log || Options.BackupType == BackupRestoreType.All,
                Options.Before
                );

            DatabaseRestore? restore = backups.Any() ? new DatabaseRestore(
                Options.Server,
                Options.Database,
                backups,
                null,
                ServiceProvider.GetRequiredService<ILogger<DatabaseRestore>>()) : null;
            var view = new RestoreView(backups, Options);
            view.Initialize();
            if (!Options.Silent)
            {
                _ = context.Console ?? throw new ArgumentException("Console property is null", nameof(context));
                using var screen = new ScreenView(ConsoleRenderer, context.Console)
                {
                    Child = view
                };
                screen.Render();
            }
            if (restore != null)
            {
                restore.Execute();
                return Task.FromResult(0);
            }
            else
            {
                return Task.FromResult(-1);
            }
        }
    }
}