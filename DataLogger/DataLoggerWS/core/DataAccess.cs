using Dapper;
using System;
using System.Configuration;
using System.Data.SqlClient;
using DataLoggerWS.models;
using System.Data.SqlTypes;
using NLog;

namespace DataLoggerWS.core
{
    public interface IDataAccess
    {
        int CreateFile(string _fileName);

        void InsertFileDataLog(FileDataLog[] fileDataLogs);
    }

    public class DataAccess : IDataAccess
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly string connectionString = ConfigurationManager.ConnectionStrings["DataLoggerDb"].ConnectionString;
        public int CreateFile(string _fileName)
        {
            var fileId = 0;
            try
            {
                var sql = @"
                        insert into FileDetails (Name) values (@Name) 
                        select scope_identity()
                        ";

                using (var conn = new SqlConnection(connectionString))
                {
                    if (!conn.ExecuteScalar<bool>("select count(1) from FileDetails where [Name] = @Name", new { Name = _fileName }))
                    {
                        fileId = conn.QuerySingle<int>(sql, new FileDetails() { Name = _fileName });
                    }
                }
            }
            catch(Exception ex)
            {
                logger.Error(ex, $"Error when updating the database for a new file {_fileName}");
            }
 
            return fileId;
        }

        public void InsertFileDataLog(FileDataLog[] _fileDataLogs)
        {
            try
            {
                var sql = @"insert into FileDataLog (FileId, LogData) values (@FileId, @LogData)";

                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Execute(sql, _fileDataLogs);
                }
            }
            catch(Exception ex)
            {
                logger.Error(ex, $" Error while updating the database with file data");
            }
        }
    }
}

namespace DataLoggerWS.models
{
    public class FileDetails
    {
        public int FileId { get; }
        public string Name { get; set; }
        public SqlDateTime Created { get; }
    }

    public class FileDataLog
    {
        public int FileId { get; set; }
        public string LogData { get; set; }
    }
}