using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Windows;
using System.Data;
using System.Configuration;
using System.IO;

namespace NetworkMonitor.Utilities
{
    class Database
    {
        public static string database { get; set; }
        public static string DbSqliteConnection { get; set; }
        SQLiteDataAdapter da;
        SQLiteCommand cmd;
        DataSet ds;

        public Database()
        {
            database = ConfigurationManager.AppSettings["DBName"];
            DbSqliteConnection = ConfigurationManager.AppSettings["DbSqliteConnection"];
        }

        public SQLiteConnection GetConnection()
        {
            SQLiteConnection connection = null;
            if (!File.Exists(database))
            {
                MessageBox.Show("DataBase is not present. Contact your administrator");
            }
            else
            {
                try
                {
                    connection = new SQLiteConnection(DbSqliteConnection);
                    return connection;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            return connection;
        }
        public void CreateDatabase()
        {
            

            SQLiteConnection m_dbConnection = null;
            DataTable dataTable = new DataTable();

            if (!File.Exists(database))
            {
                SQLiteConnection.CreateFile(database);

                m_dbConnection = new SQLiteConnection(DbSqliteConnection);
                m_dbConnection.Open();

                string sql = "create table network (id integer primary KEY AUTOINCREMENT , events varchar(1000))";

                SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
                command.ExecuteNonQuery();

                m_dbConnection.Close();
            }
        }

        public DataTable GetDataWithQuery(string tableName, string whereQuery)
        {
            SQLiteConnection connection = null;
            DataTable dataTable = new DataTable();

            if (!File.Exists(database))
            {
                MessageBox.Show("DataBase is not present. Contact your administrator");
            }
            else
            {
                try
                {
                    connection = new SQLiteConnection(DbSqliteConnection);
                    connection.Open();
                    SQLiteCommand sqlite_cmd;
                    sqlite_cmd = connection.CreateCommand();
                    sqlite_cmd.CommandText = String.Concat(whereQuery);

                    dataTable.Load(sqlite_cmd.ExecuteReader());
                    connection.Close();
                    return dataTable;
                }
                catch (Exception ex)
                {
                    
                }
            }
            return dataTable;
        }

        public int ExecuteNonQuery(string sql)
        {
            SQLiteConnection cnn = new SQLiteConnection(DbSqliteConnection);
            cnn.Open();
            SQLiteCommand mycommand = new SQLiteCommand(cnn);
            mycommand.CommandText = sql;
            int rowsUpdated = mycommand.ExecuteNonQuery();
            cnn.Close();
            return rowsUpdated;
        }

        public Boolean DeleteData(string tableName)
        {
            try
            {
                this.ExecuteNonQuery(String.Format("delete from {0};", tableName));
                this.ExecuteNonQuery(String.Format("delete from sqlite_sequence where name ='{0}';", tableName));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
