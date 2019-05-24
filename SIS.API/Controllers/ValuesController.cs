using System;
using Microsoft.AspNetCore.Mvc;
using System.Data.SQLite;
using System.Data;
using SIS.API.Models;
using SIS.API.SISDB;

namespace SIS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [HttpGet]
        public ActionResult<object> GetInfo()
        {
            return new {
                Into = "SIS : Stock Information System",
                Version = 1.0,
                APIStatus = "Active",
                DatabaseStatus = SISDBContext.GetDBStatus().ToString() == "open" ? "Is On-line" : "is Off-line"
            };
        }
        // GET api/values
        //[HttpGet("{id}")]
        ////public ActionResult<IEnumerable<Stock>> Get()
        ////return new string[] { "value1", "value2" };
        //public ActionResult<Stock> Get([FromRoute] int id)
        //{

        //    var myStocks = new StockInfo();
        //    var aStock =  myStocks.GetStock(id);
        //    return aStock;
        //}
        //[HttpGet ("GetAllStocks")]
        //public ActionResult<DataTable> GetAll()
        //{

        //    var myStocks = new StockInfo();
        //    var allStocks = myStocks.GetStocks();
        //    return allStocks;
        //}

        //[HttpGet("GetLatstPriceOfASctock/{id}")]
        //public ActionResult<StockPrice> GetLatstPriceOfASctock([FromRoute] int id)
        //{
        //    var myStocks = new StockInfo();
        //    var aStock = myStocks.GetAStockLatestPrice(id);
        //    return aStock;
        //}
        //// POST api/values
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT api/values/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/values/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}

/*
 * SQLiteConnection cnn;
var connectionString = "data source=\"" + fullPath + "\"";
cnn = new SQLiteConnection(connectionString);
                cnn.Open();



//-------------------






        public void GetStats(out int numRows)
        {
            try
            {
                using (SQLiteCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT COUNT(*) as cnt from User";
                    cmd.CommandType = System.Data.CommandType.Text;

                    SQLiteDataReader reader;
                    reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        numRows = Convert.ToInt32(reader["cnt"]);
                        return;
                    }

                    numRows = 0;
                }
            }
            catch (Exception exc)
            {
                DoException(exc);
                numRows = 0;
            }
        }



        public void Vacuum()
        {
            try
            {
                using (SQLiteCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = "VACUUM";
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception exc)
            {
                DoException(exc);
            }
        }





        private void InsertUser(User user)
        {
            try
            {

                using (SQLiteCommand cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO [User] ([UserId], [VIP], [warnCounter], [kickCounter], [banned], [country], 
[userStatusType], [totalVisits], [LastVipDate], [waitingMessage], [waitingMessageRecurrence], [messageVisibilityType]) 
VALUES (@UserId, @VIP, @warnCounter, @kickCounter, @banned, @country, @userStatusType, @totalVisits, @LastVipDate, 
@waitingMessage, @waitingMessageRecurrence, @messageVisibilityType)";

                    cmd.Parameters.Add(new SQLiteParameter("@UserId") { Value = user.UserId, });
                    cmd.Parameters.Add(new SQLiteParameter("@VIP") { Value = user.VIP });
                    cmd.Parameters.Add(new SQLiteParameter("@warnCounter") { Value = user.warnCounter });
                    cmd.Parameters.Add(new SQLiteParameter("@kickCounter") { Value = user.kickCounter });
                    cmd.Parameters.Add(new SQLiteParameter("@banned") { Value = user.banned });
                    cmd.Parameters.Add(new SQLiteParameter("@country") { Value = user.country });
                    cmd.Parameters.Add(new SQLiteParameter("@userStatusType") { Value = user.userStatusType });
                    cmd.Parameters.Add(new SQLiteParameter("@totalVisits") { Value = user.totalVisits });
                    cmd.Parameters.Add(new SQLiteParameter("@LastVipDate") { Value = user.LastVipDate });
                    cmd.Parameters.Add(new SQLiteParameter("@waitingMessage") { Value = user.waitingMessage });
                    cmd.Parameters.Add(new SQLiteParameter("@waitingMessageRecurrence") { Value = user.waitingMessageRecurrence });
                    cmd.Parameters.Add(new SQLiteParameter("@messageVisibilityType") { Value = user.messageVisibilityType });
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception exc)
            {
                DoException(exc);
            }

        }
 */
