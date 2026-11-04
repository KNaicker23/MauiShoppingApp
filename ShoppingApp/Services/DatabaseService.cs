using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using ShoppingApp.Models;
using System.Linq;

namespace ShoppingApp.Services
{
    public class DatabaseService 
    {
        private string dbFileName = "shopping.db";
        private string localDbPath = string.Empty;
        private SQLiteAsyncConnection? dbConnection;

        public DatabaseService()
        {
            localDbPath = Path.Combine(FileSystem.Current.AppDataDirectory, dbFileName);
        }
        private async Task<SQLiteAsyncConnection> GetDbConnectionAsync()
        {
            if (dbConnection != null)
            {
                return dbConnection;
            }

            if (!File.Exists(localDbPath))
            {
                await ExtractDbFromMauiAssetAsync();
            }

            dbConnection = new SQLiteAsyncConnection(localDbPath);

            await dbConnection.ExecuteAsync("PRAGMA foreign_keys = ON;");

            return dbConnection;
        }
        public async Task<List<Product>> GetProductsAsync()//for products
        {
            var db = await GetDbConnectionAsync();

            return await db.Table<Product>().ToListAsync();
        }
        public async Task<List<Customer>> GetCustomersAsync()//for customer
        {
            var db = await GetDbConnectionAsync();

            return await db.Table<Customer>().ToListAsync();
        }
        public async Task<List<Category>> GetCategoriesAsync()//for category
        {
            var db = await GetDbConnectionAsync();

            return await db.Table<Category>().ToListAsync();
        }
        public async Task<List<CartItem>> GetCartItemsAsync(int customerID)//takes parameter for allowing to get a specific customers cart item instead of all customers
        {
            var db = await GetDbConnectionAsync();

            return await db.Table<CartItem>()
                           .Where(item => item.CustomerID == customerID)
                           .ToListAsync();
        }
        public async Task<List<CartDisplayItem>> GetCartDisplayItemsAsync(int customerID)
        {
            var db = await GetDbConnectionAsync();

            var cartItems = await db.Table<CartItem>()
                .Where(c => c.CustomerID == customerID)
                .ToListAsync();

            var products = await db.Table<Product>().ToListAsync();

            var displayItems = new List<CartDisplayItem>();

            foreach (var cartItem in cartItems)
            {
                var product = products.FirstOrDefault(
                    p => p.ProductID == cartItem.ProductID);

                if (product != null)
                {
                    displayItems.Add(new CartDisplayItem
                    {
                        CartItemID = cartItem.CartItemID,
                        ProductID = product.ProductID,
                        ProductName = product.ProductName,
                        ImageName = product.ImageName,
                        ProductPrice = product.ProductPrice,
                        Quantity = cartItem.CartItemQuantity
                    });
                }
            }

            return displayItems;
        }
        public async Task<bool> RemoveCartItemAsync(int cartItemID)
        {
            try
            {
                var db = await GetDbConnectionAsync();

                var cartItem = await db.Table<CartItem>()
                    .Where(c => c.CartItemID == cartItemID)
                    .FirstOrDefaultAsync();

                if (cartItem == null)
                    return false;

                await db.DeleteAsync(cartItem);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task ExtractDbFromMauiAssetAsync()
        {
            using var packagedDatabase =
                await FileSystem.Current.OpenAppPackageFileAsync(dbFileName);

            using var destination = File.Create(localDbPath);

            await packagedDatabase.CopyToAsync(destination);
        }


        public async Task<(bool Success, string Message)> AddToCartAsync(
            int customerID,
            int productID,
            int quantityToAdd)
        {
            try
            {
                var db = await GetDbConnectionAsync();

                // Validate the requested quantity.
                if (quantityToAdd <= 0)
                {
                    return (false, "Please select a valid quantity.");
                }

                // Get the latest product information from the database.
                var product = await db.Table<Product>()
                    .Where(p => p.ProductID == productID)
                    .FirstOrDefaultAsync();

                if (product == null)
                {
                    return (false, "Product could not be found.");
                }

                if (product.StockAvailable <= 0)
                {
                    return (false, "This product is out of stock.");
                }

                // Check whether this product is already in this customer's cart.
                var existingCartItem = await db.Table<CartItem>()
                    .Where(c => c.CustomerID == customerID
                             && c.ProductID == productID)
                    .FirstOrDefaultAsync();

                int currentQuantity = existingCartItem?.CartItemQuantity ?? 0;

                // Calculate the new total quantity.
                int newQuantity = currentQuantity + quantityToAdd;

                // Prevent the customer from exceeding available stock.
                if (newQuantity > product.StockAvailable)
                {
                    return (false,
                        $"Only {product.StockAvailable - currentQuantity} more units can be added.");
                }

                if (existingCartItem == null)
                {
                    // Create a new cart item.
                    var newCartItem = new CartItem
                    {
                        CustomerID = customerID,
                        ProductID = productID,
                        CartItemQuantity = quantityToAdd
                    };

                    await db.InsertAsync(newCartItem);
                }
                else
                {
                    // Update the existing cart item's quantity.
                    existingCartItem.CartItemQuantity = newQuantity;

                    await db.UpdateAsync(existingCartItem);
                }

                return (true, "Product successfully added to your cart!");
            }
            catch (Exception ex)
            {
                return (false, $"Unable to add product to cart: {ex.Message}");
            }
        }

    }





}