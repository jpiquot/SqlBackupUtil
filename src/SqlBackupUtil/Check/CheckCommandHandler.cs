using System;
using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.CommandLine.Rendering;
using System.CommandLine.Rendering.Views;

using SqlBackup;

namespace SqlBackupUtil
{
    internal class CheckCommandHandler
    {
        private readonly ConsoleRenderer _consoleRenderer;
        private readonly InvocationContext _invocationContext;
        private readonly CheckOptions _options;

        public CheckCommandHandler(InvocationContext invocationContext, ConsoleRenderer consoleRenderer, CheckOptions options)
        {
            _invocationContext = invocationContext ?? throw new ArgumentNullException(nameof(invocationContext));
            _consoleRenderer = consoleRenderer ?? throw new ArgumentNullException(nameof(consoleRenderer));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public int Execute()
        {
            var settings = new BackupStoreSettings
            (
                _options.BackupExtensions,
                _options.BackupDirectories,
                _options.Login,
                _options.Password,
                _options.IncludeSubDirectories
            );
            var store = new BackupStore(_options.Server, Microsoft.Extensions.Options.Options.Create(settings));

            IEnumerable<BackupHeader>? backups = store.GetBackups
                (
                _options.SourceServer,
                _options.SourceDatabase,
                _options.BackupType switch
                {
                    BackupRestoreType.Full => BackupType.Full,
                    BackupRestoreType.Diff => BackupType.Differential,
                    BackupRestoreType.Log => BackupType.Log,
                    _ => null
                });

            var check = new CheckView(backups, _options);
            check.Initialize();
            if (!_options.Silent)
            {
                using var screen = new ScreenView(_consoleRenderer, _invocationContext.Console) { Child = check };
                screen.Render();
            }
            return (check.HasErrors) ? -1 : 0;
        }
    }
}