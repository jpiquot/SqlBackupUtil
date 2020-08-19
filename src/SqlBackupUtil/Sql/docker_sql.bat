docker pull mcr.microsoft.com/mssql/server:2019-latest
docker container stop mssql
docker container rm mssql
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Test@123#456" -e "MSSQL_DATA_DIR=/sql/data" -e "MSSQL_LOG_DIR=/sql/log" -e "MSSQL_BACKUP_DIR=/sql/backup" -p 11433:1433 --name mssql -v F:\Sql\data:/sql/data -v F:\Sql\log:/sql/log -v F:\Sql\backup:/sql/backup -d mcr.microsoft.com/mssql/server:2019-latest
copy D:\Dev\jpiquot\SqlBackupUtil\test\SqlBackupTest\Bak\*.* F:\Sql\backup /Y
pause