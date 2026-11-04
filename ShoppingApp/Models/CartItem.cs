using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace ShoppingApp.Models
{
    [Table("CartItem")]
    public class CartItem
    {
        [PrimaryKey, AutoIncrement]
        public int CartItemID { get; set; }

        [NotNull]
        public int CartItemQuantity { get; set; }

        [NotNull]
        public int ProductID { get; set; }

        [NotNull]
        public int CustomerID { get; set; }
    }
}
