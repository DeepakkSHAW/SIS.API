using Microsoft.AspNetCore.Mvc;
using SIS.API.Models;
using SIS.API.Web;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace SIS.API.SISDB
{
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

        #region Database IO Async Calls

        internal static async Task<List<string>> StatusdbAsync()
        {
            var status = new List<string>();
            try
            {
                string stm = SISDBQueryCollection.qGetStatusType;
                using (SQLiteConnection conn = GetdbConnection())
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(stm, conn))
                    {
                        cmd.Parameters.Clear();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                                status.Add(reader["Status"].ToString());
                        }
                    }
                }
                return status;
            }
            catch (SQLiteException ex)
            {
                System.Diagnostics.Debug.WriteLine("SQLiteException:" + ex.Message);
                throw ex;
            }
            catch (Exception ex) { throw ex; }
        }
        internal static async Task<List<string>> CategoriesdbAsync()
        {
            var categoties = new List<string>();
            try
            {
                string stm = SISDBQueryCollection.qGetCategories;
                using (SQLiteConnection conn = GetdbConnection())
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(stm, conn))
                    {
                        cmd.Parameters.Clear();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                                categoties.Add(reader["Category"].ToString());
                        }
                    }
                }
                return categoties;
            }
            catch (SQLiteException ex)
            {
                System.Diagnostics.Debug.WriteLine("SQLiteException:" + ex.Message);
                throw ex;
            }
            catch (Exception ex) { throw ex; }
        }

        internal static async Task<List<Stock>> AllStocksdbAsync()
        {
            var stocks = new List<Stock>();

            try
            {
                string stm = SISDBQueryCollection.qGetAllStocks;
                using (SQLiteConnection conn = SISDBContext.GetdbConnection())
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(stm, conn))
                    {
                        cmd.Parameters.Clear();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var s = new Stock();
                                s.Load((SQLiteDataReader)reader);
                                stocks.Add(s);
                            }
                        }
                    }
                    return stocks;
                }
            }
            catch (SQLiteException ex)
            {
                System.Diagnostics.Debug.WriteLine("SQLiteException:" + ex.Message);
                throw ex;
            }
            catch (Exception ex) { throw ex; }
        }

        internal static async Task<Stock> AStocksdbAsync(int id)
        {
            var aStock = new Stock();

            try
            {
                string stm = SISDBQueryCollection.qGetAStock;
                using (SQLiteConnection conn = SISDBContext.GetdbConnection())
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(stm, conn))
                    {
                        cmd.Parameters.Clear();
                        SQLiteParameter p1 = new SQLiteParameter("varStockID", System.Data.DbType.Int16);
                        p1.Value = id;
                        cmd.Parameters.Add(p1);
                        using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow))
                        {
                            while (await reader.ReadAsync())
                                aStock.Load((SQLiteDataReader)reader);
                        }
                    }
                    return aStock;
                }
            }
            catch (SQLiteException ex)
            {
                System.Diagnostics.Debug.WriteLine("SQLiteException:" + ex.Message);
                throw ex;
            }
            catch (Exception ex) { throw ex; }
        }
        internal static async Task<List<Stock>> StocksdbAsync(string status = "Active", string category = "")
        {
            var stocks = new List<Stock>();

            try
            {
                string stm = SISDBQueryCollection.qGetAllStocks;
                using (SQLiteConnection conn = SISDBContext.GetdbConnection())
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(stm, conn))
                    {
                        cmd.Parameters.Clear();
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var s = new Stock();
                                s.Load((SQLiteDataReader)reader);
                                stocks.Add(s);
                            }
                        }
                    }
                }

                if (string.IsNullOrEmpty(category))
                    return (stocks.Where(e => e.Status.Contains(status, StringComparison.OrdinalIgnoreCase)).ToList());
                else
                    return (stocks.Where(e => e.Status.Contains(status, StringComparison.OrdinalIgnoreCase) &&
                                                e.Category.Contains(category, StringComparison.OrdinalIgnoreCase)).ToList());

            }
            catch (SQLiteException ex)
            {
                System.Diagnostics.Debug.WriteLine("SQLiteException:" + ex.Message);
                throw ex;
            }
            catch (Exception ex) { throw ex; }
        }
        internal static async Task<StockDetails> AStockDetailsdbAsync(int id)
        {
            var sDetails = new StockDetails();

            try
            {
                string stm = SISDBQueryCollection.qGetAStockDetails;
                using (SQLiteConnection conn = SISDBContext.GetdbConnection())
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(stm, conn))
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@varStockID", id);
                        using (var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SingleRow))
                        {
                            while (await reader.ReadAsync())
                                sDetails.Load((SQLiteDataReader)reader);
                        }
                    }
                    return sDetails;
                }
            }
            catch (SQLiteException ex)
            {
                System.Diagnostics.Debug.WriteLine("SQLiteException:" + ex.Message);
                throw ex;
            }
            catch (Exception ex) { throw ex; }
        }
        internal static async Task<List<object>> GetStocksCodedbAsync(string IsActive, LoadFrom from)
        {
            var stocks = new List<Stock>();
            var vReturn = new List<object>();
            try
            {
                string stm = string.Empty;
                switch (from)
                {
                    case LoadFrom.Rediff:
                        stm = SISDBQueryCollection.qGetRediffCode;
                        break;
                    case LoadFrom.Yahoo:
                        stm = SISDBQueryCollection.qGetYahooCode;
                        break;
                }

                using (SQLiteConnection conn = SISDBContext.GetdbConnection())
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(stm, conn))
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@StatusType", IsActive);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                //var o = new JsonResult();
                                vReturn.Add(new JsonResult(new
                                {
                                    SID = reader[0],
                                    SName = reader[1].ToString(),
                                    Code = reader[2].ToString()
                                }));
                            }
                        }
                    }
                    return vReturn;
                }
            }
            catch (SQLiteException ex)
            {
                System.Diagnostics.Debug.WriteLine("SQLiteException:" + ex.Message);
                throw ex;
            }
            catch (Exception ex) { throw ex; }
        }
        internal static async Task<bool> StockPriceBulkUpdatedbAsync(List<StockPrice> sp)
        {
            var sSql = SISDBQueryCollection.qInsertLatestStockPrice;
            foreach (var s in sp)
            {
                var values = ($"({s.StockID}, {s.Price}, '{s.ValueOn.ToString("yyyy-MM-dd HH:mm:ss")}'),");
                sSql = sSql + values;
            }
            sSql = sSql.TrimEnd(',');
            try
            {
                using (SQLiteConnection conn = SISDBContext.GetdbConnection())
                {
                    using (SQLiteCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = sSql;
                        if (conn.State != ConnectionState.Open) conn.Open();
                        await cmd.ExecuteNonQueryAsync();
                       // int lastID = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
                return true;
            }
            catch (SQLiteException ex)
            {
                System.Diagnostics.Debug.WriteLine("SQLiteException:" + ex.Message);
                throw ex;
            }
            catch (Exception ex) { throw ex; }
        }
        #endregion
    }
    public class StockInfo
    {
        public DataTable GetStocks()
        {
            DataSet ds = new DataSet();
            DataTable dtStock;

            ds.Reset();
            using (SQLiteConnection conn = SISDBContext.GetdbConnection())
            {
                string stm = "SELECT * FROM TStockName";// LIMIT 5";
                using (SQLiteCommand cmd = new SQLiteCommand(stm, conn))
                {
                    SQLiteDataAdapter daStocks = new SQLiteDataAdapter(cmd);
                    daStocks.Fill(ds);
                    dtStock = ds.Tables[0];
                }
            }
            //var myStocks = ds.Tables[0].AsEnumerable().Select(dataRow => new Stock { StockName = dataRow.Field<string>("StockName") }).ToList();
            return dtStock;
        }


    }
}
