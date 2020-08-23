﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

using Microsoft.SqlServer.Management.Smo;

namespace SqlBackup
{
    public class DatabaseRestore
    {
        private readonly IEnumerable<BackupHeader> _backups;
        private readonly string _destinationDatabaseName;
        private readonly Server _server;
        private IEnumerable<DatabaseFileInfo>? _relocatedFiles;
        private Restore? _restore;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="destinationDatabaseName"></param>
        /// <param name="backups"></param>
        public DatabaseRestore(string serverName, string destinationDatabaseName, IEnumerable<BackupHeader> backups)
        {
            if (string.IsNullOrWhiteSpace(serverName))
            {
                throw new ArgumentException(Properties.Resources.ArgumentIsNullOrWhitespace, nameof(serverName));
            }
            if (string.IsNullOrWhiteSpace(destinationDatabaseName))
            {
                throw new ArgumentException(Properties.Resources.ArgumentIsNullOrWhitespace, nameof(destinationDatabaseName));
            }
            _server = new Server(serverName);
            _destinationDatabaseName = destinationDatabaseName;
            _backups = backups ?? throw new ArgumentNullException(nameof(backups));
            if (!_backups.Any())
            {
                throw new InvalidOperationException(Properties.Resources.NoBackupFileToRestore);
            }
        }

        public IEnumerable<DatabaseFileInfo> RelocatedFiles => _relocatedFiles ??= InitRelocatedFiles();
        private Restore Restore => _restore ??= InitRestore();

        /// <summary>
        /// Execute the restore operation on the database
        /// </summary>
        public void Execute()
        {
            //Gets the exclusive access to database
            _server.KillAllProcesses(_destinationDatabaseName);
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
                Action = RestoreActionType.Database,
                Database = _destinationDatabaseName,
                ReplaceDatabase = true
            };
            foreach (string fileName in _backups.Select(p => p.FileName).Distinct())
            {
                restore.Devices.Add(new BackupDeviceItem(fileName, Microsoft.SqlServer.Management.Smo.DeviceType.File));
            }
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
                    throw new NotSupportedException(string.Format(Properties.Resources.FileTypeNotSupported, fileInfo.FileType));
            }
            return file;
        }
    }
}