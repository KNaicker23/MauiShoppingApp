using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingApp.Models
{
    [Table("Category")]
    public class Category
    {
        [PrimaryKey, AutoIncrement]
        public int CategoryID { get; set; }

        [NotNull]
        public string CategoryName { get; set; } = string.Empty;
    }
}
