using SQLite;
using System;
using System.Collections.Generic;
using System.Text;


namespace ShoppingApp.Models
{
    [Table("CustomerAddress")]
    public class CustomerAddress
    {
        [PrimaryKey, AutoIncrement]
        public int CustomerAddressID { get; set; }

        public string? StreetNumber { get; set; }

        public string? StreetName { get; set; }

        public string? Suburb { get; set; }

        public string? City { get; set; }

        public string? PostalCode { get; set; }

        [NotNull]
        public string Province { get; set; } = string.Empty;

        [NotNull]
        public int CustomerID { get; set; }
    }
}
