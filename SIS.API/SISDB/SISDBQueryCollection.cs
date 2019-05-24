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



    }
}

//SELECT TStockCategories.Categorie, TStocks.*
//FROM TStockCategories INNER JOIN TStocks ON TStockCategories.ID = TStocks.StockCategorie_ID
//WHERE (((TStocks.ID)= 10));
