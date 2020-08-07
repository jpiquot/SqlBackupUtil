using System;
using System.Collections.Generic;
using System.CommandLine.Rendering;
using System.CommandLine.Rendering.Views;
using System.Linq;

using SqlBackup.Database;

namespace SqlBackupUtil
{
    /// <summary>
    /// Database backup list view
    /// </summary>
    public class ListView : StackLayoutView
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="backups">List of backup file headers</param>
        /// <param name="directories">Directories where the files where searched.</param>
        /// <param name="extensions">File extension filter</param>
        public ListView(IEnumerable<BackupHeader> backups, IEnumerable<string> directories, IEnumerable<string> extensions)
        {

            Add(new ContentView("\n"));
            Add(new ContentView(Span($"Backup directories: {string.Join("; ", directories).Rgb(235, 30, 180)}")));
            Add(new ContentView(Span($"Backup extensions: {string.Join("; ", extensions).Rgb(235, 30, 180)}")));
            Add(new ContentView("\n"));

            var tableView = new TableView<BackupHeader>();

            tableView.Items = (from s in backups orderby s.ServerName, s.DatabaseName, s.StartDate select s).ToList();

            tableView.AddColumn(
                cellValue: f => Span(f.ServerName),
                header: new ContentView("Server".Underline()));

            tableView.AddColumn(
                cellValue: f => Span(f.DatabaseName),
                header: new ContentView("Database".Underline()));

            tableView.AddColumn(
               cellValue: f => f.BackupType == BackupType.Full
                                   ? f.BackupType.ToString().LightGreen()
                                   : f.BackupType == BackupType.Differential
                                       ? f.BackupType.ToString().LightBlue()
                                       : f.BackupType.ToString().White(),
               header: new ContentView("Type".Underline()));

            tableView.AddColumn(
                cellValue: f => Span(f.Position),
                header: new ContentView("Position".Underline()));

            tableView.AddColumn(
                cellValue: f => Span(f.StartDate),
                header: new ContentView("StartDate".Underline()));

            tableView.AddColumn(
                cellValue: f => Span(f.DatabaseBackupLSN),
                header: new ContentView("Backup LSN".Underline()));

            tableView.AddColumn(
                cellValue: f => Span(f.CheckpointLSN),
                header: new ContentView("Checkpoint LSN".Underline()));

            tableView.AddColumn(
                cellValue: f => Span(f.FileName),
                header: new ContentView("Backup file".Underline()));

            Add(tableView);

            Formatter.AddFormatter<DateTime>(d => $"{d:d} {ForegroundColorSpan.DarkGray()}{d:t}");
        }

        TextSpan Span(FormattableString formattableString)
        {
            return Formatter.ParseToSpan(formattableString);
        }

        TextSpan Span(object obj)
        {
            return Formatter.Format(obj);
        }

        protected TextSpanFormatter Formatter { get; } = new TextSpanFormatter();
    }
}

