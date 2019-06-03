using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SIS.API.Models;
using SIS.API.SISDB;
using SIS.API.Web;


namespace SIS.API
{
    internal static class DownloadAndSync
    {
        internal static async Task<List<StockPrice>> DownloadAndSync_Rediff()
        {
            int iCount = 0;
            var spCollection = new List<StockPrice>();
            foreach (var s in await SISDBContext.GetStocksCodedbAsync("Active", LoadFrom.Rediff))
            {
                JObject o = JObject.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(s));
                var theCode = (string)o["Value"]["Code"];
                System.Diagnostics.Debug.WriteLine($"Download started for stock {(string)o["Value"]["SName"]} code : {theCode}");

                var sp = WebScraping.ScrapingStart(LoadFrom.Rediff, theCode);
                sp.StockID = (int)o["Value"]["SID"];
                sp.StockName = (string)o["Value"]["SName"];
                spCollection.Add(sp);
                iCount++;
                 if (iCount > 9) break; //[DK: remove] just for test
            }
            if (await SISDBContext.StockPriceBulkUpdatedbAsync(spCollection))
                return spCollection;
            else
                throw new Exception ("DownloadAndSync_Rediff: Unable to update the latest Stock Price into database");
        }
        internal static async Task<List<StockPrice>> DownloadAndSync_Yahoo()
        {
            int iCount = 0;
            var spCollection = new List<StockPrice>();
            foreach (var s in await SISDBContext.GetStocksCodedbAsync("Active", LoadFrom.Yahoo))
            {
                JObject o = JObject.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(s));
                var theCode = (string)o["Value"]["Code"];
                System.Diagnostics.Debug.WriteLine($"Download started for stock {(string)o["Value"]["SName"]} code : {theCode}");

                var sp = WebScraping.ScrapingStart(LoadFrom.Yahoo, theCode);
                sp.StockID = (int)o["Value"]["SID"];
                sp.StockName = (string)o["Value"]["SName"];
                spCollection.Add(sp);
                iCount++;
                if (iCount > 9) break; //[DK: remove] just for test
            }
            if (await SISDBContext.StockPriceBulkUpdatedbAsync(spCollection))
                return spCollection;
            else
                throw new Exception("DownloadAndSync_Yahoo: Unable to update the latest Stock Price into database");
        }
    }
}
