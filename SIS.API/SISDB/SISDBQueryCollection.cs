using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIS.API.SISDB
{
    internal static class SISDBQueryCollection
    {
        internal static string qGetAllStocks { get; } = "SELECT TStocks.*, TCategories.Category, TStatusType.Status FROM ((TStocks " +
                "INNER JOIN TCategories ON TCategories.CatID = TStocks.CatID) " +
                "INNER JOIN TStatusType ON TStatusType.StatusID = TStocks.StockStatusID) ";

        //internal static string qGetAStock { get; } = "SELECT * FROM TStock where StockID=@varStockID";

        internal static string qGetAStock { get; } = "SELECT TStocks.*, TCategories.Category, TStatusType.Status FROM ((TStocks " +
                "INNER JOIN TCategories ON TCategories.CatID = TStocks.CatID) " +
                "INNER JOIN TStatusType ON TStatusType.StatusID = TStocks.StockStatusID) " +
                "WHERE (TStocks.StockID= @varStockID);";
        internal static string qGetAStockDetails { get; } = "SELECT TStocks.*, TStockDetails.* FROM TStocks " +
                "INNER JOIN TStockDetails ON TStocks.StockID = TStockDetails.StockID " +
                "WHERE (TStocks.StockID= @varStockID)";
        internal static string qGetCategories { get; } = "SELECT * from TCategories order by Category ASC";
        internal static string qGetStatusType { get; } = "SELECT * from TStatusType order by Status ASC";

       internal static readonly string qGetRediffCode = "SELECT TStocks.StockID, TStocks.StockName, " +
            "TStockDetails.RCode FROM (TStatusType " +
            "INNER JOIN TStocks ON TStatusType.StatusID = TStocks.StockStatusID) " +
            "INNER JOIN TStockDetails ON TStocks.StockID = TStockDetails.StockID " +
            "WHERE TStatusType.Status= @StatusType";

     internal static readonly string qGetYahooCode = "SELECT TStocks.StockID, TStocks.StockName, " +
     "TStockDetails.YFCode FROM (TStatusType " +
     "INNER JOIN TStocks ON TStatusType.StatusID = TStocks.StockStatusID) " +
     "INNER JOIN TStockDetails ON TStocks.StockID = TStockDetails.StockID " +
     "WHERE TStatusType.Status= @StatusType";
        internal static readonly string qInsertLatestStockPrice = "INSERT INTO [TRates] ('StockID', 'Price','Ondate') VALUES ";// ({0},{1},'{2}')";
        internal static readonly string qGetLatestStockPrice = "select t.StockID, t.Ondate, t.Price from TRates t inner join " +
            "(select StockID, max(Ondate) as MaxDate from TRates group by StockID) " +
            "tm on t.StockID = tm.StockID and t.Ondate = tm.MaxDate ORDER by t.StockID ASC";
    }
}

//SELECT TStockCategories.Categorie, TStocks.*
//FROM TStockCategories INNER JOIN TStocks ON TStockCategories.ID = TStocks.StockCategorie_ID
//WHERE (((TStocks.ID)= 10));
