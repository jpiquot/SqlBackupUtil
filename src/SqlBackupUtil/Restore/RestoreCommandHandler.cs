using System;
using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.CommandLine.Rendering;
using System.CommandLine.Rendering.Views;
using System.Linq;

using Microsoft.Extensions.Options;

using SqlBackup;

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
            var store = new BackupStore(_options.Server, Options.Create(settings));
            _ = _options.SourceServer ?? throw new NotSupportedException("The source server must be defined.");
            _ = _options.SourceDatabase ?? throw new NotSupportedException("The source database must be defined.");
            IEnumerable<BackupHeader> backups = store.GetLatestBackupSet
                (
                _options.SourceServer,
                _options.SourceDatabase,
                _options.BackupType == BackupTypes.Diff || _options.BackupType == BackupTypes.All,
                _options.BackupType == BackupTypes.Log || _options.BackupType == BackupTypes.All,
                _options.Before
                );

            var restore = backups.Any() ? new DatabaseRestore(_options.Server, _options.Database, backups) : null;
            var view = new RestoreView(backups, _options);
            view.Initialize();
            if (!_options.Silent)
            {
                var screen = new ScreenView(_consoleRenderer, _invocationContext.Console);
                screen.Child = view;
                screen.Render();
            }
            if (restore != null)
            {
                restore.Execute();
                return 0;
            }
            else
            {
                return -1;
            }
        }
    }
}