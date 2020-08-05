using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.SqlServer.Management.Smo;

namespace SqlBackup.Database
{
    public class DatabaseRestore
    {
        private readonly string destinationDatabaseName;
        private readonly IEnumerable<string> fileNames;
        private readonly Server server;
        private Restore? _restore;

        /// <summary>
        /// Restore constructor
        /// </summary>
        /// <param name="destinationDatabaseName">The name of the database to restore</param>
        /// <param name="fileNames">              The backup files names</param>
        public DatabaseRestore(Server server, string destinationDatabaseName, IEnumerable<string> fileNames)
        {
            this.fileNames = fileNames ?? throw new ArgumentNullException(nameof(fileNames));
            this.destinationDatabaseName = destinationDatabaseName ?? throw new ArgumentNullException(nameof(destinationDatabaseName));
            this.server = server ?? throw new ArgumentNullException(nameof(server)); ;
        }

        //public BackupSet BackupSet => new BackupSet();
        public Restore Restore => _restore ??= new Restore();
    }
}