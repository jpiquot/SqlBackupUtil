using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Management.Smo;

namespace SqlBackup
{
    public class DatabaseRestore
    {
        private readonly IEnumerable<BackupHeader> _backups;
        private readonly string _destinationDatabaseName;
        private readonly ILogger<DatabaseRestore> _logger;
        private readonly DateTime? _pointInTime;
        private readonly Server _server;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serverName">The name of the Sql Server where the database has to be restored.</param>
        /// <param name="destinationDatabaseName">The name of the database to restore.</param>
        /// <param name="backups">List of the backups needed for the restore operation.</param>
        /// <param name="pointInTime">Restore to a point of time.</param>
        /// <param name="serviceProvider">The service provider instance.</param>
        public DatabaseRestore(
            string serverName,
            string destinationDatabaseName,
            IEnumerable<BackupHeader> backups,
            DateTime? pointInTime,
            ILogger<DatabaseRestore> logger)
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
            _server.ConnectionContext.ConnectTimeout = 5000;
            _server.ConnectionContext.StatementTimeout = 3600;
            _destinationDatabaseName = destinationDatabaseName;
            _backups = backups ?? throw new ArgumentNullException(nameof(backups));
            _pointInTime = pointInTime;
            _logger = logger;
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
            _logger.LogInformation(Properties.Resources.KillAllDatabaseProcesses.Format(_destinationDatabaseName));
            //Gets the exclusive access to database
            _server.KillAllProcesses(_destinationDatabaseName);
            DateTime start = DateTime.Now;
            _logger.LogInformation(Properties.Resources.DatabaseRestoreStart.Format(_destinationDatabaseName, start));
            BackupHeader[]? backups = _backups.OrderBy(p => p.LastLSN).ToArray();
            int count = _backups.Count();
            for (int i = 0; i < count; i++)
            {
                new DatabaseRestoreItem(_server, _destinationDatabaseName, backups[i], _pointInTime, i == count - 1).Execute();
            }
            DateTime end = DateTime.Now;
            var elapsed = new TimeSpan((end - start).Ticks);
            _logger.LogInformation(Properties.Resources.DatabaseRestoreEnd.Format(_destinationDatabaseName, end, (int)elapsed.TotalHours, elapsed.Minutes, elapsed.Seconds));
        }
    }
}