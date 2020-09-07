using System;
using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.CommandLine.Rendering.Views;
using System.Threading.Tasks;

using SqlBackup;

#pragma warning disable CA1812 // Internal class that is apparently never instantiated.

namespace SqlBackupUtil
{
    internal class CheckCommandHandler : CommandHandler<CheckCommandHandler, CheckOptions>
    {
        public CheckCommandHandler(IServiceProvider serviceProvider, CheckOptions options) : base(serviceProvider, options)
        {
        }

        public override Task<int> InvokeAsync(InvocationContext context)
        {
            var store = new BackupStore(Options.Server, Microsoft.Extensions.Options.Options.Create(Options.GetBackupStoreSettings()));

            IEnumerable<BackupHeader>? backups = store.GetBackups
                (
                Options.SourceServer,
                Options.SourceDatabase,
                Options.BackupType switch
                {
                    BackupRestoreType.Full => BackupType.Full,
                    BackupRestoreType.Diff => BackupType.Differential,
                    BackupRestoreType.Log => BackupType.Log,
                    _ => null
                });

            var check = new CheckView(backups, Options);
            check.Initialize();
            if (!Options.Silent)
            {
                _ = context.Console ?? throw new ArgumentException("Console property is null", nameof(context));
                using var screen = new ScreenView(ConsoleRenderer, context.Console) { Child = check };
                screen.Render();
            }
            return Task.FromResult((check.HasErrors) ? -1 : 0);
        }
    }
}