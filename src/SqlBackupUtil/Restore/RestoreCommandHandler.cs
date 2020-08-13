using System;
using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.CommandLine.Rendering;
using System.CommandLine.Rendering.Views;
using System.IO.Abstractions;

using Microsoft.Extensions.Options;

using SqlBackup.Database;

namespace SqlBackupUtil
{
    /// <summary>
    /// Handle the restore command
    /// </summary>
    internal class RestoreCommandHandler
    {
        private readonly ConsoleRenderer _consoleRenderer;
        private readonly InvocationContext _invocationContext;
        private readonly RestoreOptions _options;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="invocationContext">The invocation context</param>
        /// <param name="consoleRenderer"></param>
        /// <param name="options">Command options</param>
        public RestoreCommandHandler(InvocationContext invocationContext, ConsoleRenderer consoleRenderer, RestoreOptions options)
        {
            _invocationContext = invocationContext ?? throw new ArgumentNullException(nameof(invocationContext));
            _consoleRenderer = consoleRenderer ?? throw new ArgumentNullException(nameof(consoleRenderer));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary>
        /// Execute the list command
        /// </summary>
        public void Execute()
        {
            var settings = new BackupStoreSettings();
            settings.BackupFileExtensions = _options.BackupExtensions;
            settings.BackupPaths = _options.BackupDirectories;
            var store = new BackupStore(_options.Server, new FileSystem(), Options.Create(settings));
            _ = _options.SourceServer ?? throw new NotSupportedException("The source server must be defined.");
            _ = _options.SourceDatabase ?? throw new NotSupportedException("The source database must be defined.");
            IEnumerable<BackupHeader> backups = store.GetLatestBackup
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

            var check = new RestoreView(backups, _options);

            var screen = new ScreenView(_consoleRenderer, _invocationContext.Console) { Child = check };
            screen.Render();

            var restore = new DatabaseRestore(_options.Server, _options.Database, backups);
            restore.Execute();
        }
    }
}