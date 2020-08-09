using System;
using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.CommandLine.Rendering;
using System.CommandLine.Rendering.Views;
using System.IO.Abstractions;
using System.Linq;

using Microsoft.Extensions.Options;

using SqlBackup.Database;

namespace SqlBackupUtil
{
    /// <summary>
    /// Handle the list command
    /// </summary>
    internal class ListCommand
    {
        private readonly InvocationContext _invocationContext;
        private readonly ConsoleRenderer _consoleRenderer;
        private readonly ListOptions _options;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="invocationContext">The invocation context</param>
        /// <param name="consoleRenderer"></param>
        /// <param name="options">Command options</param>
        public ListCommand(InvocationContext invocationContext, ConsoleRenderer consoleRenderer, ListOptions options)
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

            IEnumerable<BackupHeader>? backups = store.GetBackupHeaders
                (
                _options.SourceServer, 
                _options.SourceDatabase, 
                _options.BackupType switch
                {
                    BackupTypeOption.Full => BackupType.Full,
                    BackupTypeOption.Diff => BackupType.Differential,
                    BackupTypeOption.Log => BackupType.Log,
                    _ => null
                });
 
            var list = new ListView(backups, settings.BackupPaths, settings.BackupFileExtensions);
            var console = _invocationContext.Console;
            if (console is ITerminal terminal)
            {
                terminal.Clear();
            }

            var consoleRenderer = new ConsoleRenderer(
                console,
                mode: OutputMode.Auto,
                resetAfterRender: true);

            var screen = new ScreenView(consoleRenderer, _invocationContext.Console) { Child = list };
            screen.Render();
        }
    }
}
