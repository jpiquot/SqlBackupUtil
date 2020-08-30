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
            {
                Login = _options.Login,
                Password = _options.Password,
                BackupFileExtensions = _options.BackupExtensions,
                BackupPaths = _options.BackupDirectories,
                IncludeSubDirectories = _options.IncludeSubDirectories
            };
            var options = Microsoft.Extensions.Options.Options.Create(settings);
            var store = new BackupStore(_options.Server, options);

            IEnumerable<BackupHeader>? backups = store.GetBackupHeaders
                (
                _options.SourceServer,
                _options.SourceDatabase,
                _options.BackupType switch
                {
                    BackupTypes.Full => BackupType.Full,
                    BackupTypes.Diff => BackupType.Differential,
                    BackupTypes.Log => BackupType.Log,
                    _ => null
                });

            var check = new CheckView(backups, _options);
            check.Initialize();
            if (!_options.Silent)
            {
                var screen = new ScreenView(_consoleRenderer, _invocationContext.Console) { Child = check };
                screen.Render();
            }
            return (check.HasErrors) ? -1 : 0;
        }
    }
}