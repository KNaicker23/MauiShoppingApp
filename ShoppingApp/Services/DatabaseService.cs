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

                
                if (quantityToAdd <= 0)// To validate the requested quantity.
                {
                    return (false, "Please select a valid quantity.");
                }

                
                var product = await db.Table<Product>()// to  get the latest product information from the database.
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

                
                int newQuantity = currentQuantity + quantityToAdd;//  the new total quantity.

                
                if (newQuantity > product.StockAvailable)// Prevent the customer from exceeding available stock.
                {
                    return (false,
                        $"Only {product.StockAvailable - currentQuantity} more units can be added.");
                }

                if (existingCartItem == null)//creating new cart item if there is none
                {
                    
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
                    
                    existingCartItem.CartItemQuantity = newQuantity;// Update the existing cart item's quantity.

                    await db.UpdateAsync(existingCartItem);
                }

                return (true, "Product successfully added to your cart!");
            }
            catch (Exception ex)
            {
                return (false, $"Unable to add product to cart: {ex.Message}");
            }
        }
        public async Task<(bool Success, string Message, int OrderID)>
           CheckoutAsync(int customerID)
        {
            var db = await GetDbConnectionAsync();

            try
            {
                int orderID = 0;

                await db.RunInTransactionAsync(connection =>
                {
                    
                    var cartItems = connection.Table<CartItem>()// Get the customer's cart items
                        .Where(c => c.CustomerID == customerID)
                        .ToList();

                    if (cartItems.Count == 0)
                        throw new Exception("Your cart is empty.");

                    decimal orderTotal = 0;

                    var orderProducts = new List<(CartItem CartItem, Product Product)>();

                   
                    foreach (var cartItem in cartItems) //to validate stock and calculate the total
                    {
                        var product = connection.Table<Product>()
                            .FirstOrDefault(p => p.ProductID == cartItem.ProductID);

                        if (product == null)
                            throw new Exception(
                                "A product in your cart could not be found.");

                        if (cartItem.CartItemQuantity <= 0)
                            throw new Exception("An invalid quantity was found.");

                        if (product.StockAvailable < cartItem.CartItemQuantity)
                            throw new Exception($"Insufficient stock for {product.ProductName}.");

                        orderTotal += product.ProductPrice *
                                      cartItem.CartItemQuantity;//logic to calc the order total

                        orderProducts.Add((cartItem, product));//add the product to cartitem which then gets ordered
                    }

                   
                    var newOrder = new Order //Create the Order record
                    {
                        CustomerID = customerID,
                        OrderDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        OrderTotal = (decimal)orderTotal//casting
                    };

                    connection.Insert(newOrder);

                    orderID = newOrder.OrderID;

                    
                    foreach (var item in orderProducts)// Creates OrderItem records and update stock
                    {
                        var orderItem = new OrderItem//instances
                        {
                            OrderID = orderID,
                            ProductID = item.Product.ProductID,
                            Quantity = item.CartItem.CartItemQuantity,
                            PriceAtPurchase = item.Product.ProductPrice
                        };

                        connection.Insert(orderItem);

                        
                        item.Product.StockAvailable -= item.CartItem.CartItemQuantity;// Deduct purchased stock

                        connection.Update(item.Product);

                        
                        connection.Delete(item.CartItem);// Removes purchased item from cart
                    }
                });

                return (true, "Your order was placed successfully!", orderID);
            }
            catch (Exception ex)
            {
                return (false, ex.Message, 0);
            }
        }

    }





}