using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace SIS.API.Models
{
    public class aStocks
    {
        public int Id { get; set; }

        [Required, StringLength(20)]
        public string StockName { get; set; }
        [RegularExpression("^[0-9a-zA-Z].{1,9}$", ErrorMessage = "Invalid Short Name - only 10 characters & numbers allowed.")]
        public string StockShortName { get; set; }

    }
    public class Stock
    {
        public int StockID { get; set; }
        public int StockCatID { get; set; }
        public int StatusID { get; set; }
        public string Category { get; set; }
        public string Status { get; set; }
        public string StockName { get; set; }
        public string ShortName { get; set; }


        public void Load(SQLiteDataReader reader)
        {
            //reading data from db Table Name TStocks
            StockID = Int32.Parse(reader["StockID"].ToString());
            StockCatID = Int32.Parse(reader["CatID"].ToString());
            Category = reader["Category"].ToString();
            Status = reader["Status"].ToString();
            StockName = reader["StockName"].ToString();
            ShortName = reader["ShortName"].ToString();
            StatusID = Int32.Parse(reader["StockStatusID"].ToString());
        }
    }
    public class StockDetails
    {
        public int StockID { get; set; }
        public string StockName { get; set; }
        public string ShortName { get; set; }
        public List<string> codes { get; set; } = new List<string>();
        public string Details { get; set; }
        public void Load(SQLiteDataReader reader)
        {
            //reading data from db Table Name TStockDetails
            StockID = Int32.Parse(reader["StockID"].ToString());
            StockName = reader["StockName"].ToString();
            ShortName = reader["ShortName"].ToString();
            codes.Add(reader["YFCode"].ToString());
            codes.Add(reader["BSECode"].ToString());
            codes.Add(reader["RCode"].ToString());
            codes.Add(reader["HDFCCode"].ToString());
            Details = reader["Details"].ToString();
        }
    }
    public class StockPrice
    {
        public int StockID { get; set; }
        public string StockName { get; set; }
        public double Price { get; set; }
        public DateTime ValueOn { get; set; }

        public void Load(SQLiteDataReader reader)
        {
            //reading data from db Table Name TRates
            StockID = Int32.Parse(reader[0].ToString());
            StockName = reader[1].ToString();
            Price = int.Parse(reader[2].ToString());
            ValueOn = DateTime.Parse(reader[3].ToString());
        }

    }

}
