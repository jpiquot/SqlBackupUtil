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
    /// Handle the restore command
    /// </summary>
    internal class RestoreCommand
    {
        private readonly InvocationContext _invocationContext;
        private readonly ConsoleRenderer _consoleRenderer;
        private readonly RestoreOptions _options;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="invocationContext">The invocation context</param>
        /// <param name="consoleRenderer"></param>
        /// <param name="options">Command options</param>
        public RestoreCommand(InvocationContext invocationContext, ConsoleRenderer consoleRenderer, RestoreOptions options)
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
 
            var check = new RestoreView(backups, _options);
 
            var screen = new ScreenView(_consoleRenderer, _invocationContext.Console) { Child = check };
            screen.Render();
            return (check.HasErrors)?-1:0;
        }
    }
}
