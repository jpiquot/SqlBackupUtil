using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;

using Microsoft.SqlServer.Management.Smo;

namespace SqlBackup
{
    public class DatabaseRestoreItem
    {
        private readonly BackupHeader _backup;
        private readonly string _destinationDatabaseName;
        private readonly bool _last;
        private readonly Server _server;
        private IEnumerable<DatabaseFileInfo>? _relocatedFiles;
        private Restore? _restore;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="destinationDatabaseName"></param>
        /// <param name="backup"></param>
        public DatabaseRestoreItem(Server server, string destinationDatabaseName, BackupHeader backup, bool last)
        {
            if (string.IsNullOrWhiteSpace(destinationDatabaseName))
            {
                throw new ArgumentException(Properties.Resources.ArgumentIsNullOrWhitespace, nameof(destinationDatabaseName));
            }
            _server = server ?? throw new ArgumentNullException(nameof(server));
            _destinationDatabaseName = destinationDatabaseName;
            _backup = backup ?? throw new ArgumentNullException(nameof(backup));
            _last = last;
        }

        public IEnumerable<DatabaseFileInfo> RelocatedFiles => _relocatedFiles ??= InitRelocatedFiles();
        private Restore Restore => _restore ??= InitRestore();

        /// <summary>
        /// Execute the restore operation on the database
        /// </summary>
        public void Execute()
        {
            Restore.Wait();

            Restore.SqlRestore(_server);
        }

        private IEnumerable<DatabaseFileInfo> InitRelocatedFiles()
        {
            DataRowCollection? rows = Restore.ReadFileList(_server).Rows;
            _ = rows ?? throw new FailedOperationException(Properties.Resources.CorruptDatabaseFileInfo);
            List<DatabaseFileInfo> files = new(rows.Count);
            if (!_server.Databases.Contains(_destinationDatabaseName))
            {
                foreach (DataRow? file in rows)
                {
                    _ = file ?? throw new FailedOperationException(Properties.Resources.CorruptDatabaseFileInfo);
                    var info = new DatabaseFileInfo(file.ToDictionary());
                    Restore.RelocateFiles.Add(Relocate(info));
                    files.Add(info);
                }
            }
            return files;
        }

        private Restore InitRestore()
        {
            var restore = new Restore()
            {
                Action = (_backup.BackupType == BackupType.Log) ? RestoreActionType.Log : RestoreActionType.Database,
                Database = _destinationDatabaseName,
                ReplaceDatabase = _backup.BackupType == BackupType.Full,
                NoRecovery = !_last,
                FileNumber = _backup.Position
            };
            restore.Devices.Add(new BackupDeviceItem(_backup.FileName, Microsoft.SqlServer.Management.Smo.DeviceType.File));
            // restore.BackupSet.BackupMediaSet;
            return restore;
        }

        private RelocateFile Relocate(DatabaseFileInfo fileInfo)
        {
            RelocateFile file = new();

            switch (fileInfo.FileType)
            {
                case FileType.Data:
                    file.LogicalFileName = _destinationDatabaseName;
                    file.PhysicalFileName = Path.Combine(_server.Settings.DefaultFile, _destinationDatabaseName + ".mdf");
                    break;

                case FileType.Log:
                    file.LogicalFileName = _destinationDatabaseName + "_log";
                    file.PhysicalFileName = Path.Combine(_server.Settings.DefaultFile, _destinationDatabaseName + "_log.ldf");
                    break;

                default:
                    throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, Properties.Resources.FileTypeNotSupported, fileInfo.FileType));
            }
            return file;
        }
    }
}