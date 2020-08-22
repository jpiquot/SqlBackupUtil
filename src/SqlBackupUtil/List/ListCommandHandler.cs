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
            {
                Login = _options.Login,
                Password = _options.Password,
                BackupFileExtensions = _options.BackupExtensions,
                BackupPaths = _options.BackupDirectories
            };
            var store = new BackupStore(_options.Server, Options.Create(settings));

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

            var list = new ListView(backups, _options);
            list.Initialize();

            var screen = new ScreenView(_consoleRenderer, _invocationContext.Console) { Child = list };
            screen.Render();
        }
    }
}