using System;
using System.Collections.Generic;
using System.CommandLine.Rendering;
using System.CommandLine.Rendering.Views;
using System.Linq;
using System.Reflection;

using SqlBackup.Database;

namespace SqlBackupUtil
{
    /// <summary>
    /// Database backup list view
    /// </summary>
    internal class CheckView : CommandView<BackupHeader, CheckOptions>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="backups">List of backup file headers</param>
        /// <param name="options"></param>
        public CheckView(IEnumerable<BackupHeader> backups, CheckOptions options) : base(backups, options)
        {


        }

        protected override void AddTableInformation()
        {
            _tableView.Items = (from s in _backups orderby s.ServerName, s.DatabaseName, s.StartDate select s).ToList();

            _tableView.AddColumn(
               cellValue: f => f.BackupType == BackupType.Full
                                   ? f.BackupType.ToString().LightGreen()
                                   : f.BackupType == BackupType.Differential
                                       ? f.BackupType.ToString().LightBlue()
                                       : f.BackupType.ToString().White(),
               header: new ContentView("Type".Underline()));

            _tableView.AddColumn(
                cellValue: f => FormatTime(f),
                header: new ContentView("Time".Underline()));

            _tableView.AddColumn(
                cellValue: f => Span(f.StartDate),
                header: new ContentView("Start date".Underline()));

            _tableView.AddColumn(
                cellValue: f => Span(f.FileName),
                header: new ContentView("Backup file".Underline()));
        }
        protected override void AddSummaryInformation()
        {
            Add(new ContentView(Span($"Source server:       {(_options.SourceServer ?? "All").DarkGrey()}")));
            Add(new ContentView(Span($"Source database:     {(_options.SourceDatabase ?? "All").DarkGrey()}")));
            Add(new ContentView(Span($"Full frequency:      {(_options.FullFrequency).ToString().DarkGrey()}")));
            Add(new ContentView(Span($"Diff frequency:      {(_options.DiffFrequency).ToString().DarkGrey()}")));
            Add(new ContentView(Span($"Log frequency:       {(_options.LogFrequency).ToString().DarkGrey()}")));

        }
        TextSpan FormatTime (BackupHeader backup)
        {
            int totalMinutes = (DateTime.Now - backup.StartDate).Minutes;
            int days = totalMinutes / (24 * 60);
            int hours = (totalMinutes % (24 * 60)) / 60;
            int minutes = (totalMinutes % (24 * 60)) % 60;
            string text = $"{minutes} minutes";
            if (days > 0 || hours > 0)
            {
                text = $"{hours} hours "+text;
            }
            if (days > 0)
            {
                text = $"{days} days "+text;
            }
            bool obsolete = backup.BackupType switch
            {
                BackupType.Full => (totalMinutes > _options.FullFrequency),
                BackupType.Differential => (totalMinutes > _options.DiffFrequency),
                BackupType.Log => (totalMinutes > _options.LogFrequency),
                _ => throw new NotSupportedException($"Backup type not supported."),
            };
            return (obsolete) ? text.LightGreen() : text.LightRed();
        }
    }
}

