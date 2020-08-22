using System;
using System.Collections.Generic;
using System.CommandLine.Rendering.Views;
using System.Linq;

using SqlBackup;

namespace SqlBackupUtil
{
    /// <summary>
    /// Database backup restore view
    /// </summary>
    internal class RestoreView : CommandView<BackupHeader, RestoreOptions>
    {
        private readonly IEnumerable<DatabaseFileInfo> _relocatedFiles;
        private TableView<DatabaseFileInfo>? _tableView2;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="relocatedFiles"></param>
        /// <param name="backups">List of backup file headers</param>
        /// <param name="options"></param>
        public RestoreView(IEnumerable<DatabaseFileInfo> relocatedFiles, IEnumerable<BackupHeader> backups, RestoreOptions options)
            : base(backups, options)
            => _relocatedFiles = relocatedFiles ?? throw new ArgumentNullException(nameof(relocatedFiles));

        public override void Initialize()
        {
            base.Initialize();
            AddRelocatedFilesTable();
        }

        protected override void AddSummaryInformation()
        {
            Add(new ContentView(Span($"Database:            {(_options.Database ?? "All").DarkGrey()}")));
            Add(new ContentView(Span($"Source server:       {(_options.SourceServer ?? "All").DarkGrey()}")));
            Add(new ContentView(Span($"Source database:     {(_options.SourceDatabase ?? "All").DarkGrey()}")));
            Add(new ContentView(Span($"\nBackup Media".Orange())));
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

        private void AddRelocatedFilesTable()
        {
            Add(new ContentView(Span($"\nDatbase Files".Orange())));
            _tableView2 = new TableView<DatabaseFileInfo>();
            Add(_tableView2);
            if (_relocatedFiles.Any())
            {
                _tableView2.Items = (from s in _relocatedFiles orderby s.FileId select s).ToList();

                _tableView2.AddColumn(
                    cellValue: f => Span(f.FileId),
                    header: new ContentView("Id".Underline()));

                _tableView2.AddColumn(
                   cellValue: f => f.FileType == FileType.Data
                                       ? f.FileType.ToString().LightGreen()
                                       : f.FileType == FileType.Log
                                           ? f.FileType.ToString().LightBlue()
                                           : f.FileType.ToString().White(),
                   header: new ContentView("Type".Underline()));

                _tableView2.AddColumn(
                    cellValue: f => Span(f.LogicalName),
                    header: new ContentView("Name".Underline()));

                _tableView2.AddColumn(
                    cellValue: f => Span(f.PhysicalName),
                    header: new ContentView("File".Underline()));
            }
        }
    }
}