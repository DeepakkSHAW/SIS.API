using Newtonsoft.Json;
using OpenScraping;
using OpenScraping.Config;
using SIS.API.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SIS.API.Web
{
    public static class HtmlExtractor
    {
        internal static StockPrice FilerTheStockpriceFromRediff(string httpResposeMessage)
        {
            TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime tm = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
            DateTime dt = DateTime.Today;

            StockPrice sp = new StockPrice();
            var configJson = @"
                        {
                            'price':'//span[2]',
                            'LastTradedDate':'//span[6]',
                            'LastTradedTime':'//span[7]'
                        }";

            //            var configJson = @"
            //            {
            //                'title1': '//h1',
            //                'title': '//script',  
            //                'price':'//span[2]',
            //                'LastTradedDate':'//span[6]',
            //                'LastTradedTime':'//span[7]',
            //'body': '//div[contains(@class, \'article\')]'
            //            }
            //            ";
            //            var html = "<html><body><h1>Article title</h1><div class='article'>Article contents</div></body></html>";
            //            html = httpResposeMessage;
            try
            {
                var config = StructuredDataConfig.ParseJsonString(configJson);

                var openScraping = new StructuredDataExtractor(config);
                var scrapingResults = openScraping.Extract(httpResposeMessage);

                System.Diagnostics.Debug.WriteLine(JsonConvert.SerializeObject(scrapingResults, Formatting.Indented));

                var thePrice = scrapingResults["price"];
                var theDate = scrapingResults["LastTradedDate"];
                var theTime = scrapingResults["LastTradedTime"];

                sp.Price = double.Parse(thePrice.ToString().Trim());

                //DateTime dt = DateTime.ParseExact(theDate.ToString().Trim(), "dd MMM", CultureInfo.InvariantCulture);
                //DateTime tm = DateTime.ParseExact(theTime.ToString().Trim(), "HH:mm:ss", CultureInfo.InvariantCulture);

                //** precaution in case missing date & time**//
                if (!string.IsNullOrEmpty(theDate.ToString()))
                {
                    if (theDate.ToString().IsDateType())
                    //    dt = (DateTime)Convert.ChangeType(theDate, typeof(DateTime));
                          dt = DateTime.ParseExact(theDate.ToString().Trim(), "dd MMM", CultureInfo.InvariantCulture);

                    if (theTime.ToString().IsDateType())
                        tm = (DateTime)Convert.ChangeType(theTime, typeof(DateTime));
                }
                else {
                    var DataNTime = theTime.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if (DataNTime.Length == 2) //has date and time
                    {
                        if (DataNTime[0].IsDateType())
                            //dt = (DateTime)Convert.ChangeType(DataNTime[0], typeof(DateTime));
                            dt = DateTime.ParseExact(DataNTime[0], "dd MMM", CultureInfo.InvariantCulture);
                        if (DataNTime[1].ToString().IsDateType())
                            tm = (DateTime)Convert.ChangeType(DataNTime[1], typeof(DateTime));
                    }
                    else { //has time only
                        if (DataNTime[0].ToString().IsDateType())
                            tm = (DateTime)Convert.ChangeType(DataNTime[0], typeof(DateTime));
                    }

                }
                //** adjust the date if date in missing in the downloaded time stamp**//
                var currectedDate = dt.Date.Add(tm.TimeOfDay);
                currectedDate = currectedDate > TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE) ?
                    currectedDate.AddYears(-1) : currectedDate;
                sp.ValueOn = currectedDate;
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return sp;
        }
        internal static StockPrice FilerTheStockpriceFromYahoo(string httpResposeMessage)
        {
            StockPrice sp = new StockPrice();

            TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime tm = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
            DateTime dt = DateTime.Today;

            var configJson = @"
                        {
                            'price':'//div[1]/div/div/div[1]/div/div[2]/div/div/div[4]/div/div/div/div[3]/div/div/span[1]',
                            'Closing':'//div[1]/div/div/div[1]/div/div[2]/div/div/div[4]/div/div/div/div[3]/div/div/div/span',
                            'DK':'//span[7]'
                        }";
            try
            {
                var config = StructuredDataConfig.ParseJsonString(configJson);

                var openScraping = new StructuredDataExtractor(config);
                var scrapingResults = openScraping.Extract(httpResposeMessage);

                System.Diagnostics.Debug.WriteLine(JsonConvert.SerializeObject(scrapingResults, Formatting.Indented));

                //Closing >> At close: May 24 3:29PM IST
                //Opening >> As of  9:23AM IST. Market open.
                //As of May 27 9:30AM IST. Market open.
                //As of May 27 9:26AM IST. Market open.
                //As of May 24 3:52PM IST. Market open.

                var thePrice = scrapingResults["price"];
                var splits = scrapingResults["Closing"].ToString().Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var index = Array.FindIndex(splits, x => x.ToUpper().TrimEnd(new char[] { '.', ','}) == "IST");
                string sDate = string.Empty, sTime = string.Empty;
                if (index > -1)
                {
                    if (index - 1 > -1) sTime = $"{splits[index - 1]}";
                    if (index - 3 > -1) sDate = $"{splits[index - 2]} {splits[index - 3]}";
                }

                //** precaution in case missing date & time**//
                if (sDate.IsDateType())
                    dt = (DateTime)Convert.ChangeType(sDate, typeof(DateTime));
                if (sTime.IsDateType())
                    tm = (DateTime)Convert.ChangeType(sTime, typeof(DateTime));

                //** adjust the date if date in missing in the downloaded time stamp**//
                var currectedDate = dt.Date.Add(tm.TimeOfDay);
                currectedDate = currectedDate > TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE) ?
                    currectedDate.AddDays(-1) : currectedDate;

                //** Convert the downloaded time into last business day**//
                if (currectedDate.IsWeekend())
                    currectedDate = currectedDate.PreviousWorkDay();

                sp.Price = double.Parse(thePrice.ToString().Trim());
                sp.ValueOn = currectedDate;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return sp;
        }
    }
}
