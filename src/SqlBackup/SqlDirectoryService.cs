using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Dapper;

using Microsoft.Data.SqlClient;

namespace SqlBackup
{
    public class SqlDirectoryService : IDirectoryService
    {
        private const string _connectionString = "Initial Catalog = master; Server= %1; Integrated Security = %2;";
        private const string _query = "EXEC master.sys.xp_dirtree '%1',1,1;";
        private readonly SqlConnection _sqlConnection;

        public SqlDirectoryService(SqlConnection sqlConnection)
        {
            _sqlConnection = sqlConnection ?? throw new ArgumentNullException(nameof(sqlConnection));
        }

        public SqlDirectoryService(string serverName, string? login, string? password)
        {
            SqlConnectionStringBuilder builder = new();
            builder.DataSource = serverName;
            if (login == null)
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

        public Task<IEnumerable<SqlDirTree>> GetDirectoryContentAsync(string path)
            => _sqlConnection
                .QueryAsync<SqlDirTree>(string.Format(_query, path));

        public IEnumerable<string> GetFiles(IEnumerable<string> paths, IEnumerable<string> extensions, bool recursive = false)
            => GetFilesAsync(paths, extensions, recursive).GetAwaiter().GetResult();

        public async Task<IEnumerable<string>> GetFilesAsync(IEnumerable<string> paths, IEnumerable<string> extensions, bool recursive = false)
        {
            List<string> list = new();
            foreach (string path in paths)
            {
                IEnumerable<SqlDirTree>? content = await GetDirectoryContentAsync(path);
                foreach (string extension in extensions)
                {
                    list.AddRange(content
                        .Where(p => p.File == true && p.SubDirectory.ToUpperInvariant().EndsWith("." + extension))
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
            return list;
        }
    }
}