using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SIS.API.Models;
using SIS.API.SISDB;
using SIS.API.Web;

namespace SIS.API.Controllers
{
    //[Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class SISController : ControllerBase
    {
        static List<aStocks> _stocks = new List<aStocks>()
            {
                new aStocks(){Id = 0,StockName = "Laptop",StockShortName = "RIL"},
                new aStocks(){Id = 1,StockName = "Mobile",StockShortName = "ABB"}
            };

        [HttpGet]
        public IActionResult GetInfo()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;

            var output = new JsonResult(new
            {
                Version = version, // 1.1,
                message = "SIS : Stock Information System",
                APIStatus = "Active",
                date = DateTime.UtcNow.ToString("dd-MMM-yyyy hh:mm:ss.fff"),
                DatabaseStatus = "is Off-line"//DB.GetDBStatus().ToString() == "open" ? "Is On-line" : "is Off-line"
            });
            output.ContentType = "application/json";
            output.StatusCode = 200;
            return output;
        }

        [HttpGet("GetStocks")]
        public IActionResult GetStocks(string status = "Active", string category = "")
        {
            //var myStocks = new StockInfo();
            //var v = Request.QueryString.Value;
            var stocks = new List<Stock>();
            try
            {
                string stm = SISDBQueryCollection.qGetAllStocks;
                using (SQLiteConnection conn = SISDBContext.GetdbConnection())
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(stm, conn))
                    {
                        cmd.Parameters.Clear();
                        SQLiteDataReader rdr = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                var s = new Stock();
                                s.Load(rdr);
                                stocks.Add(s);
                            }
                            if (string.IsNullOrEmpty(category))
                                return Ok(stocks.Where(e => e.Status.Contains(status, StringComparison.OrdinalIgnoreCase)).ToList());
                            else
                                return Ok(stocks.Where(e => e.Status.Contains(status, StringComparison.OrdinalIgnoreCase) &&
                                                            e.Category.Contains(category, StringComparison.OrdinalIgnoreCase)).ToList());
                        }
                        else
                            return StatusCode(StatusCodes.Status204NoContent);
                        //return NotFound();
                    }
                }
            }
            catch (SQLiteException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = ex.Message });
            }
            catch (Exception ex)
            {
                //return new JsonResult(new { Status = "Error", Message = ex.Message });
                return StatusCode(StatusCodes.Status417ExpectationFailed, new JsonResult(new { Status = "Error", Message = ex.Message }));
            }
        }

        [HttpGet("GetAllStocks")]
        public IActionResult GetAllStocks()
        {
            //var myStocks = new StockInfo();
            var stocks = new List<Stock>();
            var s = new Stock();
            try
            {
                string stm = SISDBQueryCollection.qGetAllStocks;
                using (SQLiteConnection conn = SISDBContext.GetdbConnection())
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(stm, conn))
                    {
                        cmd.Parameters.Clear();
                        SQLiteDataReader rdr = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                s.Load(rdr);
                                stocks.Add(s);
                            }
                            return Ok(stocks);
                        }
                        else
                            return StatusCode(StatusCodes.Status204NoContent);
                        //return NotFound();
                    }
                }
            }
            catch (SQLiteException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status417ExpectationFailed, new JsonResult(new { Status = "Error", Message = ex.Message }));
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get([FromRoute] int id)
        {
            //var myStocks = new StockInfo();
            var s = new Stock();
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

                        SQLiteDataReader rdr = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                        if (rdr.HasRows)
                        {
                            if (rdr.Read())
                            {
                                s.Load(rdr);
                            }
                            return Ok(s);
                        }
                        else
                            return StatusCode(StatusCodes.Status204NoContent);
                        //return NotFound();
                    }
                }
            }
            catch (SQLiteException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = ex.Message });
            }
            catch (Exception ex)
            {
                //return new JsonResult(new { Status = "Error", Message = ex.Message });
                return StatusCode(StatusCodes.Status417ExpectationFailed, new JsonResult(new { Status = "Error", Message = ex.Message }));
            }
        }

        [HttpGet("GetCategories")]
        public IActionResult GetCategories()
        {
            var categoties = new List<string>();
            try
            {
                string stm = SISDBQueryCollection.qGetCategories;
                using (SQLiteConnection conn = SISDBContext.GetdbConnection())
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(stm, conn))
                    {
                        cmd.Parameters.Clear();
                        SQLiteDataReader rdr = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                categoties.Add(rdr["Category"].ToString());
                            }
                            return Ok(categoties);
                        }
                        else
                            return StatusCode(StatusCodes.Status204NoContent);
                    }
                }
            }
            catch (SQLiteException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status417ExpectationFailed, new JsonResult(new { Status = "Error", Message = ex.Message }));
            }
        }

        [HttpGet("GetStatusType")]
        public IActionResult GetStatusType()
        {
            var categoties = new List<string>();
            try
            {
                string stm = SISDBQueryCollection.qGetStatusType;
                using (SQLiteConnection conn = SISDBContext.GetdbConnection())
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(stm, conn))
                    {
                        cmd.Parameters.Clear();
                        SQLiteDataReader rdr = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                categoties.Add(rdr["Status"].ToString());
                            }
                            return Ok(categoties);
                        }
                        else
                            return StatusCode(StatusCodes.Status204NoContent);
                    }
                }
            }
            catch (SQLiteException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status417ExpectationFailed, new JsonResult(new { Status = "Error", Message = ex.Message }));
            }
        }

        [HttpGet("GetStockDetails/{id:int}")]
        public IActionResult GetStockDetails([FromRoute] int id)
        {
            //var myStocks = new StockInfo();
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
                        using (var reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                        {
                            if (reader.HasRows)
                            {
                                if (reader.Read())
                                {
                                    //result = new User((int)reader["UserId"], reader["UserName"].ToString());
                                    sDetails.Load(reader);
                                }
                                return Ok(sDetails);
                            }
                            else
                            {
                                return StatusCode(StatusCodes.Status204NoContent);
                            }
                        }
                        //SQLiteParameter p1 = new SQLiteParameter("varStockID", System.Data.DbType.Int16);
                        //p1.Value = id;
                        //cmd.Parameters.Add(p1);

                        //SQLiteDataReader rdr = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                        //if (rdr.HasRows)
                        //{
                        //    if (rdr.Read())
                        //    {
                        //        //var r = Serialize(rdr);
                        //        //json = JsonConvert.SerializeObject(rdr);
                        //        dt.Load(rdr);
                        //        json = JsonConvert.SerializeObject(dt);
                        //    }
                        //    return Ok(json);
                        //}
                        //else
                        //    return StatusCode(StatusCodes.Status204NoContent);
                        //return NotFound();
                    }
                }
            }
            catch (SQLiteException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = "Error", Message = ex.Message });
            }
            catch (Exception ex)
            {
                //return new JsonResult(new { Status = "Error", Message = ex.Message });
                return StatusCode(StatusCodes.Status417ExpectationFailed, new JsonResult(new { Status = "Error", Message = ex.Message }));
            }
        }

        [HttpGet("StartScraping")]
        public async Task<IActionResult> StartScraping()
        {
            try
            {
                WebScraping.ScrapingStart(LoadFrom.Rediff);
                return Ok();
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }
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
        public StockPrice GetAStockLatestPrice(int id)
        {
            StockPrice p = new StockPrice();
            using (SQLiteConnection conn = SISDBContext.GetdbConnection())
            {
                string stm = "select StockID, 'DUMMY', Price, Ondate from TRates WHERE StockID = @varStockID ORDER by RateID DESC LIMIT 1";

                using (SQLiteCommand cmd = new SQLiteCommand(stm, conn))
                {
                    //cmd.CommandText = @"GetEmployeeDetails";
                    //cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.Clear();
                    SQLiteParameter p1 = new SQLiteParameter("varStockID", System.Data.DbType.Int16);
                    p1.Value = id;

                    cmd.Parameters.Add(p1);

                    SQLiteDataReader rdr = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                    if (rdr.Read())
                    {
                        p.Load(rdr);
                    }
                }
            }
            return p;
        }


    }
}