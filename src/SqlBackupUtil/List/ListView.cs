using System.Collections.Generic;
using System.CommandLine.Rendering.Views;
using System.Linq;

using SqlBackup;

namespace SqlBackupUtil
{
    /// <summary>
    /// Database backup list view
    /// </summary>
    internal class ListView : CommandView<BackupHeader, ListOptions>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="backups">List of backup file headers</param>
        /// <param name="options"></param>
        public ListView(IEnumerable<BackupHeader> backups, ListOptions options) : base(backups, options)
        {
        }

        protected override void AddSummaryInformation()
        {
            Add(new ContentView(Span($"Latest only:             {_options.LatestOnly.ToString().DarkGrey()}")));
            if (!_backups.Any())
            {
                Add(new ContentView(Span($"\nNo backup files found!".LightRed())));
            }
        }

        protected override void AddTableInformation()
        {
            if (_tableView == null)
            {
                return;
            }
            _tableView.Items = (from s in _backups orderby s.ServerName, s.DatabaseName, s.StartDate select s).ToList();

            _tableView.AddColumn(
                cellValue: f => Span(f.ServerName),
                header: new ContentView("Server".Underline()));

            _tableView.AddColumn(
                cellValue: f => Span(f.DatabaseName),
                header: new ContentView("Database".Underline()));

            _tableView.AddColumn(
               cellValue: f => f.BackupType == BackupType.Full
                                   ? f.BackupType.ToString().LightGreen()
                                   : f.BackupType == BackupType.Differential
                                       ? f.BackupType.ToString().LightBlue()
                                       : f.BackupType.ToString().White(),
               header: new ContentView("Type".Underline()));

            _tableView.AddColumn(
                cellValue: f => Span(f.Position),
                header: new ContentView("Position".Underline()));

            _tableView.AddColumn(
                cellValue: f => Span(f.StartDate),
                header: new ContentView("Start date".Underline()));

            _tableView.AddColumn(
                cellValue: f => Span(f.DatabaseBackupLSN),
                header: new ContentView("Backup LSN".Underline()));

            _tableView.AddColumn(
                cellValue: f => Span(f.CheckpointLSN),
                header: new ContentView("Checkpoint LSN".Underline()));

            _tableView.AddColumn(
                 cellValue: f => Span(f.LastLSN),
                 header: new ContentView("Last LSN".Underline()));

            _tableView.AddColumn(
                cellValue: f => Span(f.FileName),
                header: new ContentView("Backup file".Underline()));
        }
    }
}