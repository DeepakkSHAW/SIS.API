using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SIS.API.Models;
using SIS.API.SISDB;

namespace SIS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SISController : ControllerBase
    {
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
        [HttpGet("GetCategories")]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                return Ok(await SISDBContext.CategoriesdbAsync());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status417ExpectationFailed, new JsonResult(new { Status = "Error", Message = ex.Message }));
            }
        }

        [HttpGet("GetStatusType")]
        public async Task<IActionResult> GetStatusType()
        {
            try
            {
                return Ok(await SISDBContext.StatusdbAsync());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status417ExpectationFailed, new JsonResult(new { Status = "Error", Message = ex.Message }));
            }
        }

        [HttpGet("GetAllStocks")]
        public async Task<IActionResult> GetAllStocks()
        {
            try
            {
                return Ok(await SISDBContext.AllStocksdbAsync());

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status417ExpectationFailed, new JsonResult(new { Status = "Error", Message = ex.Message }));
            }
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {

            try
            {
                return Ok(await SISDBContext.AStocksdbAsync(id));
            }
            catch (Exception ex)
            {
                //return new JsonResult(new { Status = "Error", Message = ex.Message });
                return StatusCode(StatusCodes.Status417ExpectationFailed, new JsonResult(new { Status = "Error", Message = ex.Message }));
            }
        }

        [HttpGet("GetStockDetails/{id:int}")]
        public async Task<IActionResult> GetStockDetails([FromRoute] int id)
        {
            //var myStocks = new StockInfo();
            var sDetails = new StockDetails();

            try
            {
                return Ok(await SISDBContext.AStockDetailsdbAsync(id));
            }
            catch (Exception ex)
            {
                //return new JsonResult(new { Status = "Error", Message = ex.Message });
                return StatusCode(StatusCodes.Status417ExpectationFailed, new JsonResult(new { Status = "Error", Message = ex.Message }));
            }
        }

        [HttpGet("GetStocks")]
        public async Task<IActionResult> GetStocks(string status = "Active", string category = "")
        {
            //var myStocks = new StockInfo();
            //var v = Request.QueryString.Value;
            var stocks = new List<Stock>();
            try
            {
                return Ok(await SISDBContext.StocksdbAsync(status, category));
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
                // var x = await SISDBContext.RediffStocksdbAsync("Active");

                //return Ok(SISDBContext.AllStocks_db());
                //var latest_sp = await DownloadAndSync.DownloadAndSync_Rediff();
                var latest_sp = await DownloadAndSync.DownloadAndSync_Yahoo();

                //StockPrice sp =  WebScraping.ScrapingStart(LoadFrom.Rediff, "11060011");
                //StockPrice sp = WebScraping.ScrapingStart(LoadFrom.Yahoo, "BHEL.NS");
                //return Ok(sp);
                return Ok(latest_sp);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }
    }

}