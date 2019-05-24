using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace SIS.API.SISDB
{
    //public class SISDBContext
    //{

    //}
    public class SISDBContext
    {
        private const string dbconn_string = @"URI=file:DB\SISAPI.db";
        /// <summary>
        /// Overrides the connection timeout
        /// </summary>
        public static int ConnectionTimeout { get; set; }
        /// <summary>
        /// Property used to override the name of the application
        /// </summary>
        public static string ApplicationName
        {
            get;
            set;
        }
        /// <summary>
        /// Returns an opened connection to the caller
        /// </summary>
        /// <returns></returns>
        public static SQLiteConnection GetdbConnection()
        {
            SQLiteConnection conn = new SQLiteConnection(dbconn_string);
            conn.Open();
            return conn;
        }

        //public static string ConnectionString
        //{
        //    get
        //    {
        //        string connStr = ConfigurationManager.ConnectionStrings["AWConnection"].ToString();

        //        SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder(connStr);
        //        sb.ApplicationName = ApplicationName ?? sb.ApplicationName;
        //        sb.ConnectTimeout = (ConnectionTimeout > 0) ? ConnectionTimeout : sb.ConnectTimeout;

        //        return sb.ToString();
        //    }
        //}

        internal int GetLastRowId(SQLiteConnection cnn)
        {
            try
            {
                using (SQLiteCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT last_insert_rowid()";
                    if (cnn.State != ConnectionState.Open) cnn.Open();
                    cmd.ExecuteNonQuery();
                    int lastID = Convert.ToInt32(cmd.ExecuteScalar());

                    return lastID;
                }
            }
            catch (Exception ex)
            {
                //DoException(ex);
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            return 0;
        }

        public static string GetDBStatus()
        {
            ConnectionState conState = GetdbConnection().State;
            string output = conState.ToString();
            return output.ToLower();
        }
    }
}
