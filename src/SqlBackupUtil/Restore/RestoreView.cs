using System;
using System.Collections.Generic;
using System.CommandLine.Rendering;
using System.CommandLine.Rendering.Views;
using System.Linq;

using SqlBackup.Database;

namespace SqlBackupUtil
{
    /// <summary>
    /// Database backup restore view
    /// </summary>
    internal class RestoreView : CommandView<BackupHeader, RestoreOptions>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="backups">List of backup file headers</param>
        /// <param name="options"></param>
        public RestoreView(IEnumerable<BackupHeader> backups, RestoreOptions options) : base(backups, options)
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
                cellValue: f => Span(f.StartDate),
                header: new ContentView("Start date".Underline()));

            _tableView.AddColumn(
                cellValue: f => Span(f.FileName),
                header: new ContentView("Backup file".Underline()));
        }
        protected override void AddSummaryInformation()
        {
            Add(new ContentView(Span($"Database:            {(_options.Database ?? "All").DarkGrey()}")));
            Add(new ContentView(Span($"Source server:       {(_options.SourceServer ?? "All").DarkGrey()}")));
            Add(new ContentView(Span($"Source database:     {(_options.SourceDatabase ?? "All").DarkGrey()}")));
        }
    }
}

