﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Dapper;

using Microsoft.Data.SqlClient;

namespace SqlBackup
{
    public class SqlDirectoryService : IDirectoryService, IDisposable
    {
        private const string _query = "EXEC master.sys.xp_dirtree '{0}',1,1;";
        private readonly SqlConnection _sqlConnection;

        private bool isDisposed;

        public SqlDirectoryService(SqlConnection sqlConnection)
        {
            _sqlConnection = sqlConnection ?? throw new ArgumentNullException(nameof(sqlConnection));
        }

        public SqlDirectoryService(string serverName, string? login, string? password)
        {
            SqlConnectionStringBuilder builder = new();
            builder.DataSource = serverName;
            if (string.IsNullOrWhiteSpace(login))
            {
                builder.IntegratedSecurity = true;
            }
            else
            {
                builder.IntegratedSecurity = false;
                builder.Authentication = SqlAuthenticationMethod.SqlPassword;
                builder.UserID = login;
                builder.Password = password;
            }
            _sqlConnection = new SqlConnection(builder.ConnectionString);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public Task<IEnumerable<SqlDirTree>> GetDirectoryContentAsync(string path)
                    => _sqlConnection
                .QueryAsync<SqlDirTree>(string.Format(CultureInfo.InvariantCulture, _query, path));

        public IEnumerable<string> GetFiles(IEnumerable<string> paths, IEnumerable<string> extensions, bool recursive = false)
            => GetFilesAsync(paths, extensions, recursive).GetAwaiter().GetResult();

        public async Task<IEnumerable<string>> GetFilesAsync(IEnumerable<string> paths, IEnumerable<string> extensions, bool recursive = false)
        {
            List<string> list = new();
            if (paths?.Any() == true && extensions?.Any() == true)
            {
                foreach (string path in paths)
                {
                    IEnumerable<SqlDirTree>? content = await GetDirectoryContentAsync(path);
                    foreach (string extension in extensions)
                    {
                        list.AddRange(content
                            .Where(p => p.File == true && p.SubDirectory.ToUpperInvariant().EndsWith("." + extension, StringComparison.InvariantCultureIgnoreCase))
                            .Select(p => Path.Combine(path, p.SubDirectory)));
                    }
                    if (recursive)
                    {
                        list.AddRange(await GetFilesAsync(content
                                    .Where(p => p.File == false)
                                    .Select(p => Path.Combine(path, p.SubDirectory)),
                                    extensions, recursive)
                                    );
                    }
                }
            }
            return list;
        }

        // The bulk of the clean-up code is implemented in Dispose(bool)
        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed)
            {
                return;
            }

            if (disposing)
            {
                _sqlConnection.Dispose();
            }

            isDisposed = true;
        }
    }
}