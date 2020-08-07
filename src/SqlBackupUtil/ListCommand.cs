using System;
using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.CommandLine.Rendering;
using System.IO.Abstractions;
using System.Linq;

using Microsoft.Extensions.Options;

using SqlBackup.Database;

namespace SqlBackupUtil
{
    /// <summary>
    /// Handle the list command
    /// </summary>
    public class ListCommand
    {
        private readonly InvocationContext _invocationContext;
        private readonly ListOptions _options;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="invocationContext">The invocation context</param>
        /// <param name="options">Command options</param>
        public ListCommand(InvocationContext invocationContext,ListOptions options)
        {
            _invocationContext = invocationContext;
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
            IEnumerable<BackupHeader>? backups = _options.BackupType switch
            {
                BackupTypeOption.Full => store.BackupHeaders.Where(p => p.BackupType == BackupType.Full),
                BackupTypeOption.Diff => store.BackupHeaders.Where(p => p.BackupType == BackupType.Differential),
                BackupTypeOption.Log => store.BackupHeaders.Where(p => p.BackupType == BackupType.Log),
                _ => store.BackupHeaders
            };
            var list = new ListView(backups, settings.BackupPaths, settings.BackupFileExtensions);
            var console = _invocationContext.Console;

            var consoleRenderer = new ConsoleRenderer(
                console,
                mode: _invocationContext.BindingContext.OutputMode(),
                resetAfterRender: true);

            console.Append(list);
        }
    }
}
