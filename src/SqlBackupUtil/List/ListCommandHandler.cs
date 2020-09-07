using System;
using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.CommandLine.Rendering.Views;
using System.Threading.Tasks;

using SqlBackup;

#pragma warning disable CA1812 // Internal class that is apparently never instantiated.

namespace SqlBackupUtil
{
    /// <summary>
    /// Handle the list command
    /// </summary>
    internal class ListCommandHandler : CommandHandler<ListCommandHandler, ListOptions>
    {
        public ListCommandHandler(IServiceProvider serviceProvider, ListOptions options) : base(serviceProvider, options)
        {
        }

        public override Task<int> InvokeAsync(InvocationContext context)
        {
            var store = new BackupStore(Options.Server, Microsoft.Extensions.Options.Options.Create(Options.GetBackupStoreSettings()));
            IEnumerable<BackupHeader>? backups;
            if (Options.LatestOnly)
            {
                if (string.IsNullOrWhiteSpace(Options.SourceServer) || string.IsNullOrWhiteSpace(Options.SourceDatabase))
                {
                    throw new InvalidOperationException(SqlBackup.Properties.Resources.SourceRequiredWithLastestOption);
                }
                backups = store.GetLatestBackupSet(
                    Options.SourceServer,
                    Options.SourceDatabase,
                    Options.BackupType == BackupRestoreType.Diff || Options.BackupType == BackupRestoreType.All,
                    Options.BackupType == BackupRestoreType.Log || Options.BackupType == BackupRestoreType.All,
                    Options.Before);
            }
            else
            {
                backups = store.GetBackups
                    (
                    Options.SourceServer,
                    Options.SourceDatabase,
                    Options.BackupType switch
                    {
                        BackupRestoreType.Full => BackupType.Full,
                        BackupRestoreType.Diff => BackupType.Differential,
                        BackupRestoreType.Log => BackupType.Log,
                        _ => null
                    },
                    Options.Before);
            }
            var list = new ListView(backups, Options);
            list.Initialize();

            if (!Options.Silent)
            {
                _ = context.Console ?? throw new ArgumentException("Console property is null", nameof(context));
                using var screen = new ScreenView(ConsoleRenderer, context.Console) { Child = list };
                screen.Render();
            }
            return Task.FromResult(0);
        }
    }
}