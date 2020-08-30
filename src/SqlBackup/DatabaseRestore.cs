using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.SqlServer.Management.Smo;

namespace SqlBackup
{
    public class DatabaseRestore
    {
        private readonly IEnumerable<BackupHeader> _backups;
        private readonly string _destinationDatabaseName;
        private readonly DateTime? _pointInTime;
        private readonly Server _server;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="destinationDatabaseName"></param>
        /// <param name="backups"></param>
        public DatabaseRestore(string serverName, string destinationDatabaseName, IEnumerable<BackupHeader> backups, DateTime? pointInTime = null)
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
            _pointInTime = pointInTime;
            if (!_backups.Any())
            {
                throw new InvalidOperationException(Properties.Resources.NoBackupFileToRestore);
            }
        }

        /// <summary>
        /// Execute the restore operation on the database
        /// </summary>
        public void Execute()
        {
            //Gets the exclusive access to database
            _server.KillAllProcesses(_destinationDatabaseName);
            BackupHeader[]? backups = _backups.OrderBy(p => p.LastLSN).ToArray();
            int count = _backups.Count();
            for (int i = 0; i < count; i++)
            {
                new DatabaseRestoreItem(_server, _destinationDatabaseName, backups[i], i == count - 1).Execute();
            }
        }
    }
}