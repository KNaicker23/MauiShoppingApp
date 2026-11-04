using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace ShoppingApp.Models
{
    [Table("Customer")]
    public class Customer
    {
        [PrimaryKey, AutoIncrement]
        public int CustomerID { get; set; }

        [NotNull]
        public string CustomerName { get; set; } = string.Empty;

        [NotNull]
        public string CustomerLastName { get; set; } = string.Empty;

        public string? CustomerNumber { get; set; }

        public string? CustomerEmail { get; set; }
    }
}
