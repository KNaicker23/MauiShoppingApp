using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace ShoppingApp.Models
{
    [Table("Order")]
    public class Order
    {
        [PrimaryKey, AutoIncrement]
        public int OrderID { get; set; }

        [NotNull]
        public string OrderDate { get; set; } = string.Empty;

        [NotNull]
        public decimal OrderTotal { get; set; }

        [NotNull]
        public int CustomerID { get; set; }
    }
}
