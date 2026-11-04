using SQLite;
using System;
using System.Collections.Generic;
using System.Text;


namespace ShoppingApp.Models
{
    [Table("OrderItem")]
    public class OrderItem
    {
        [PrimaryKey, AutoIncrement]
        public int OrderItemID { get; set; }

        [NotNull]
        public int OrderID { get; set; }

        [NotNull]
        public int ProductID { get; set; }

        [NotNull]
        public int Quantity { get; set; }

        [NotNull]
        public decimal PriceAtPurchase { get; set; }
    }
}
