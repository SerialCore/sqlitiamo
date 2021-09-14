using Microsoft.Data.Sqlite;
using System;
using Windows.Storage;

namespace sqlitiamo
{
    public class DataStorage
    {
        public DataStorage(string dbname)
        {
            this._dbname = dbname;
            this._dbpath = ApplicationData.Current.LocalCacheFolder.Path + "\\" + dbname + ".db";
            this._conn = new SqliteConnection(string.Format("Data Source={0};", this._dbpath));
            this._conn.Open();
        }

        private string _dbname;
        private string _dbpath;
        private SqliteConnection _conn;

        public string Database => _dbname;

        public string AbsolutePath => _dbpath;

        public int ExecuteWrite(string sql)
        {
            int affectedRows = 0;

            try
            {
                affectedRows = new SqliteCommand(sql, _conn).ExecuteNonQuery();
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"\tSqliteException: {ex.Message}");
            }

            return affectedRows;
        }

        public SqliteDataReader ExecuteRead(string sql)
        {
            try
            {
                SqliteDataReader reader = new SqliteCommand(sql, _conn).ExecuteReader();
                
                return reader;
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"\tSqliteException: {ex.Message}");
                
                return null;
            }
        }

        public void Close()
        {
            _conn.Close();
            _conn.Dispose();
        }

    }
}
