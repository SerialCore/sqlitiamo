using Microsoft.Data.Sqlite;
using System;
using System.Data;
using Windows.Storage;

namespace sqlitiamo
{
    public class DataStorage
    {
        public DataStorage(string dbname)
        {
            this._dbname = dbname;
            this._dbpath = ApplicationData.Current.LocalCacheFolder.Path + "\\" + dbname + ".db";
        }

        private string _dbname;
        private string _dbpath;
        private SqliteConnection _conn
        {
            get
            {
                var con = new SqliteConnection(string.Format("Data Source={0};", this._dbpath));
                con.Open();
                return con;
            }
        }

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

            _conn.Close();
            _conn.Dispose();

            return affectedRows;
        }

        public SqliteDataReader ExecuteRead(string sql)
        {
            try
            {
                SqliteDataReader reader = new SqliteCommand(sql, _conn).ExecuteReader();

                _conn.Close();
                _conn.Dispose();

                return reader;
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"\tSqliteException: {ex.Message}");

                _conn.Close();
                _conn.Dispose();

                return null;
            }
        }

    }
}
