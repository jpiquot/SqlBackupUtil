using System;
using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.CommandLine.Rendering;
using System.CommandLine.Rendering.Views;

using Microsoft.Extensions.Options;

using SqlBackup;

namespace SqlBackupUtil
{
    /// <summary>
    /// Handle the list command
    /// </summary>
    internal class ListCommandHandler
    {
        private readonly ConsoleRenderer _consoleRenderer;
        private readonly InvocationContext _invocationContext;
        private readonly ListOptions _options;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="invocationContext">The invocation context</param>
        /// <param name="consoleRenderer"></param>
        /// <param name="options">Command options</param>
        public ListCommandHandler(InvocationContext invocationContext, ConsoleRenderer consoleRenderer, ListOptions options)
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
            var settings = new BackupStoreSettings
            (
                _options.BackupExtensions,
                _options.BackupDirectories,
                _options.Login,
                _options.Password,
                _options.IncludeSubDirectories
            );
            var store = new BackupStore(_options.Server, Options.Create(settings));
            IEnumerable<BackupHeader>? backups;
            if (_options.LatestOnly)
            {
                if (string.IsNullOrWhiteSpace(_options.SourceServer) || string.IsNullOrWhiteSpace(_options.SourceDatabase))
                {
                    throw new InvalidOperationException(SqlBackup.Properties.Resources.SourceRequiredWithLastestOption);
                }
                backups = store.GetLatestBackupSet(
                    _options.SourceServer,
                    _options.SourceDatabase,
                    _options.BackupType == BackupRestoreType.Diff || _options.BackupType == BackupRestoreType.All,
                    _options.BackupType == BackupRestoreType.Log || _options.BackupType == BackupRestoreType.All,
                    _options.Before);
            }
            else
            {
                backups = store.GetBackups
                    (
                    _options.SourceServer,
                    _options.SourceDatabase,
                    _options.BackupType switch
                    {
                        BackupRestoreType.Full => BackupType.Full,
                        BackupRestoreType.Diff => BackupType.Differential,
                        BackupRestoreType.Log => BackupType.Log,
                        _ => null
                    },
                    _options.Before);
            }
            var list = new ListView(backups, _options);
            list.Initialize();

            if (!_options.Silent)
            {
                using var screen = new ScreenView(_consoleRenderer, _invocationContext.Console) { Child = list };
                screen.Render();
            }
        }
    }
}