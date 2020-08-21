using System;
using System.Collections.Generic;
using System.CommandLine.Rendering;
using System.CommandLine.Rendering.Views;
using System.Linq;
using System.Reflection;

namespace SqlBackupUtil
{
    /// <summary>
    /// Database backup list view
    /// </summary>
    internal abstract class CommandView<TItems, TOptions> : StackLayoutView
        where TOptions : CommandOptions
    {
        protected readonly IEnumerable<TItems> _backups;
        protected readonly TOptions _options;
        protected readonly TableView<TItems> _tableView;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="backups">List of backup file headers</param>
        /// <param name="options"></param>
        public CommandView(IEnumerable<TItems> backups, TOptions options)
        {
            _backups = backups ?? throw new ArgumentNullException(nameof(backups));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _tableView = new TableView<TItems>();
        }

        protected TextSpanFormatter Formatter { get; } = new TextSpanFormatter();

        public virtual void Initialize()
        {
            Add(new ContentView("\n"));
            Add(new ContentView(Span($"Sql backup utility V{Assembly.GetExecutingAssembly().GetName().Version}".Orange())));
            Add(new ContentView(Span($"Jérôme Piquot".DarkOrange())));
            Add(new ContentView("\n"));
            Add(new ContentView(Span($"Command:             {string.Join("; ", _options.Command).White()}")));
            Add(new ContentView(Span($"Sql server:          {string.Join("; ", _options.Server).DarkGrey()}")));
            Add(new ContentView("\n"));
            Add(new ContentView(Span($"Backup directories:  {string.Join("; ", _options.BackupDirectories).DarkGrey()}")));
            Add(new ContentView(Span($"Backup extensions:   {string.Join("; ", _options.BackupExtensions).DarkGrey()}")));
            Add(new ContentView(Span($"Backup type:         {_options.BackupType.ToString().DarkGrey()}")));
            AddSummaryInformation();
            Add(new ContentView("\n"));
            if (_backups.Any())
            {
                Add(_tableView);
                AddTableInformation();
            }
            Formatter.AddFormatter<DateTime>(d => $"{d:d} {ForegroundColorSpan.DarkGray()}{d:t}");
        }

        protected abstract void AddSummaryInformation();

        protected abstract void AddTableInformation();

        protected TextSpan Span(FormattableString formattableString) => Formatter.ParseToSpan(formattableString);

        protected TextSpan Span(object obj) => Formatter.Format(obj);
    }
}