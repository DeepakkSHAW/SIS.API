using SIS.API.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

        public static StockPrice ScrapingStart(LoadFrom from, string code)
        {
            var exceptions = new List<Exception>();
            var TotalNumberOfAttempts = 3;
            var numberOfAttempts = 0;
            StockPrice stockPrice = new StockPrice();
            var httpResposeMessage = string.Empty;
            //code = "16510001"; //[DK: Remove] just to test
            string webURL = string.Empty;
            bool found = false;
            while (!found)
            {
                try
                {
                    switch (from)
                    {
                        case LoadFrom.Rediff:
                            webURL = $"https://money.rediff.com/money/jsp/current_stat.jsp?companyCode={code}";
                            httpResposeMessage = GetWebRespose(webURL).GetAwaiter().GetResult();
                            stockPrice = HtmlExtractor.FilerTheStockpriceFromRediff(httpResposeMessage);
                            found = true;
                            break;
                        case LoadFrom.Yahoo:
                            webURL = $"https://finance.yahoo.com/quote/{code}";
                            httpResposeMessage = GetWebRespose(webURL).GetAwaiter().GetResult();
                            stockPrice = HtmlExtractor.FilerTheStockpriceFromYahoo(httpResposeMessage);
                            found = true;
                            break;
                        case LoadFrom.BSE:
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                    numberOfAttempts++;
                    Thread.Sleep(1 * 1000);
                }
                if (numberOfAttempts >= TotalNumberOfAttempts || found)
                    break;
            }
            if (exceptions.Count > 0)
                // throw new AggregateException(exceptions);
                System.Diagnostics.Debug.WriteLine(exceptions);
            return stockPrice;
        }



        //public static string RediffScraping(string code)
        //{
        //    string sURL = string.Empty;
        //    var httpResMessage = string.Empty;
        //    code = "11060011"; // DK: Remove this after development
        //    try
        //    {
        //        WebProxy wp = new WebProxy("192.168.0.1", 8000);
        //        wp.Credentials = new NetworkCredential("ID", "PWD");
        //        wp.UseDefaultCredentials = false;
        //        wp.BypassProxyOnLocal = false; // proxy will apply on local url as well

        //        using (var client = new WebClient())
        //        {
        //            sURL = $"https://money.rediff.com/money/jsp/current_stat.jsp?companyCode={code}";
        //            client.Encoding = Encoding.UTF8;
        //            if (!client.IsBusy)
        //            {
        //                if (false)
        //                    client.Proxy = wp;


        //                httpResMessage = client.DownloadString(sURL);



        //                return httpResMessage;
        //            }
        //            return "busy";
        //        }
        //    }
        //    catch (WebException we)
        //    {
        //        System.Diagnostics.Debug.WriteLine(we.Message);
        //        throw we;
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine(ex.Message);
        //        throw ex;
        //    }

        //}
    }
}
