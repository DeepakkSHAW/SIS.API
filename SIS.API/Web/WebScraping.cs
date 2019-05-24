using SIS.API.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using OpenScraping.Config;
using OpenScraping;
using Newtonsoft.Json;
using System.Globalization;

namespace SIS.API.Web
{
    public enum LoadFrom {Rediff = 1, BSE=2, Yahoo=3};
    public static class WebScraping
    {
        private static async Task<string> GetWebRespose(string uri)
        {
            var webProxy = new WebProxy(new Uri(@"http://192.168.0.1:8000"), BypassOnLocal: false);
            webProxy.Credentials = new NetworkCredential("ID", "PWD");

            var proxyHttpClientHandler = new HttpClientHandler { Proxy = webProxy, UseProxy = false, UseDefaultCredentials = true };
            var httpClient = new System.Net.Http.HttpClient(proxyHttpClientHandler)
            {
                //BaseAddress = new Uri( sURL), //not needed since this is not an API based call
                Timeout = new TimeSpan(0, 5, 0),
                MaxResponseContentBufferSize = 1024 * 1024,
            };
            try
            {
                ////var response = await httpClient.GetStringAsync(sURL);
                //var response = await httpClient.GetStringAsync(sURL);
                //System.Diagnostics.Debug.WriteLine(response);

                HttpResponseMessage response = await httpClient.GetAsync(uri);
                if (response.IsSuccessStatusCode)
                {
                    String urlContents = await response.Content.ReadAsStringAsync();
                    return urlContents;
                }
                else
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                        throw new Exception("No valid API key provided.");
                    if (response.StatusCode >= System.Net.HttpStatusCode.InternalServerError)
                        throw new Exception($"There is a problem with the http service {uri}.");
                }

            }
            catch (WebException ex)
            {
                using (HttpWebResponse response = (HttpWebResponse)ex.Response)
                {
                    using (System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream()))
                    {
                        string respStr = sr.ReadToEnd();
                        int statusCode = (int)response.StatusCode;
                        System.Diagnostics.Debug.WriteLine("exception: " + ex.Message);
                    }
                }
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("exception: " + ex.Message);
                throw new Exception(ex.Message);
            }
            return null;
        }
        private static StockPrice FilerTheStockprice(string httpResposeMessage)
        {
            StockPrice sp = new StockPrice();
            var configJson = @"
            {
                'title1': '//h1',
                'title': '//script',  
                'price':'//span[2]',
                'LastTradedDate':'//span[6]',
                'LastTradedTime':'//span[7]',
'body': '//div[contains(@class, \'article\')]'
            }
            ";
            var config = StructuredDataConfig.ParseJsonString(configJson);
            var html = "<html><body><h1>Article title</h1><div class='article'>Article contents</div></body></html>";
            html = httpResposeMessage;
            var openScraping = new StructuredDataExtractor(config);
            var scrapingResults = openScraping.Extract(html);

            System.Diagnostics.Debug.WriteLine(scrapingResults["price"]);
            System.Diagnostics.Debug.WriteLine("----------------------------");
            System.Diagnostics.Debug.WriteLine(JsonConvert.SerializeObject(scrapingResults, Formatting.Indented));

            var thePrice = scrapingResults["price"];
            var theDate = scrapingResults["LastTradedDate"];
            var theTime = scrapingResults["LastTradedTime"];

            sp.Price = double.Parse(thePrice.ToString().Trim());

            DateTime dt = DateTime.ParseExact(theDate.ToString().Trim(), "dd MMM", CultureInfo.InvariantCulture);
            DateTime tm = DateTime.ParseExact(theTime.ToString().Trim(), "hh:mm:ss", CultureInfo.InvariantCulture);
            sp.ValueOn = dt.Date.Add(tm.TimeOfDay);


            return sp;
        }
        public static void ScrapingStart(LoadFrom from)
        {
            var exceptions = new List<Exception>();
            var TotalNumberOfAttempts = 3;
            var numberOfAttempts = 0;
            StockPrice stockPrice = new StockPrice();

            string webURL = string.Empty;
            var code = "11060011"; // DK: Remove this after development

            switch (from)
            {
                case LoadFrom.Rediff:
                    webURL = $"https://money.rediff.com/money/jsp/current_stat.jsp?companyCode={code}";

                    break;
                case LoadFrom.Yahoo:
                    break;
                case LoadFrom.BSE:
                    break;
                default:
                    break;
              }

            var httpResposeMessage = string.Empty;
            while (true)
            {
                try
                {
                    httpResposeMessage = GetWebRespose(webURL).GetAwaiter().GetResult();
                    stockPrice = FilerTheStockprice(httpResposeMessage);
                    break;
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                    numberOfAttempts++;
                    System.Threading.Thread.Sleep(1*1000);
                    if (numberOfAttempts >= TotalNumberOfAttempts)
                        break;
                }
            }
            if (exceptions.Count > 0)
                throw new AggregateException(exceptions);
        }



        public static string RediffScraping(string code)
        {
            string sURL = string.Empty;
            var httpResMessage = string.Empty;
            code = "11060011"; // DK: Remove this after development
            try
            {
                WebProxy wp = new WebProxy("192.168.0.1", 8000);
                wp.Credentials = new NetworkCredential("ID", "PWD");
                wp.UseDefaultCredentials = false;
                wp.BypassProxyOnLocal = false; // proxy will apply on local url as well

                using (var client = new WebClient())
                {
                    sURL = $"https://money.rediff.com/money/jsp/current_stat.jsp?companyCode={code}";
                    client.Encoding = Encoding.UTF8;
                    if (!client.IsBusy)
                    {
                        if (false)
                            client.Proxy = wp;


                        httpResMessage = client.DownloadString(sURL);



                        return httpResMessage;
                    }
                    return "busy";
                }
            }
            catch (WebException we)
            {
                System.Diagnostics.Debug.WriteLine(we.Message);
                throw we;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                throw ex;
            }

        }
    }
}
