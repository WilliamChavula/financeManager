using System;
using System.IO;
using SQLite;
using Xamarin.Essentials;

namespace personalFinanceManager.services
{
    public static class DBConnection
    {
        public static SQLiteAsyncConnection Init()
        {
            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "FinanceManager.db");

            SQLiteAsyncConnection db = new SQLiteAsyncConnection(databasePath);

            return db;
        }
    }
}
