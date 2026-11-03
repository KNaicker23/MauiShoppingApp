using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace ShoppingApp.Models
{
    [Table("Product")]
    public class Product
    {
        [PrimaryKey,AutoIncrement]
        public int ProductID { get; set; }

        [NotNull]
        public int CategoryID { get; set; }

        [NotNull]
        public string ProductName { get; set; } = string.Empty;

        public string? ProductDescription { get; set; }

        [NotNull]
        public double ProductPrice { get; set; }

        [NotNull]
        public int StockAvailable { get; set; }

        public string? ImageName { get; set; }
    }
}
