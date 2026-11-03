using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using ShoppingApp.Models;

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
        public async Task<List<Product>> GetProductsAsync()
        {
            var db = await GetDbConnectionAsync();

            return await db.Table<Product>().ToListAsync();
        }

        private async Task ExtractDbFromMauiAssetAsync()
        {
            using var packagedDatabase =
                await FileSystem.Current.OpenAppPackageFileAsync(dbFileName);

            using var destination = File.Create(localDbPath);

            await packagedDatabase.CopyToAsync(destination);
        }
    }




    
}