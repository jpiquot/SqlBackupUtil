using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

using Microsoft.SqlServer.Management.Smo;

namespace SqlBackup.Database
{
    public class DatabaseRestore
    {
        private readonly string _destinationDatabaseName;
        private readonly IEnumerable<BackupHeader> _backups;
        private readonly Server _server;
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
        }

        private Restore Restore => _restore ??= InitRestore();
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
                restore.Devices.Add(new BackupDeviceItem(fileName, DeviceType.File));
            }
            return restore;
        }
        /// <summary>
        /// Execute the restore operation on the database
        /// </summary>
        public void Execute()
        {
            foreach (DataRow file in Restore.ReadFileList(_server).Rows)
            {
                Restore.RelocateFiles.Add(Relocate(file));
            }
            //Gets the exclusive access to database
            _server.KillAllProcesses(_destinationDatabaseName);
            Restore.Wait();

            Restore.SqlRestore(_server);
        }
        private RelocateFile Relocate (DataRow row)
        {
            RelocateFile file = new ();
            if ((string)row[1] == "D")
            {
                file.LogicalFileName = _destinationDatabaseName;
                file.PhysicalFileName = Path.Combine(_server.Settings.DefaultFile, _destinationDatabaseName+".mdf");
            }
            else
            {
                file.LogicalFileName = _destinationDatabaseName+"_log";
                file.PhysicalFileName = Path.Combine(_server.Settings.DefaultFile, _destinationDatabaseName+"_log.ldf");
            }
            return file;
        }

    }
}