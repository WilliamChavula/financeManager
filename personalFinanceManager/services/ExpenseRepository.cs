using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using personalFinanceManager.interfaces;
using personalFinanceManager.models;
using SQLite;

namespace personalFinanceManager.services
{
    public class ExpenseRepository : IRepository<Expense>
    {
        static SQLiteAsyncConnection database;

        public event EventHandler<Expense> OnItemAdded;
        public event EventHandler<Expense> OnItemDeleted;

        public static async Task Init()
        {
            if (database != null)
                return;

            database = DBConnection.Init();

            await database.CreateTableAsync<Expense>();
        }

        public async Task AddItems(Expense item)
        {
            await Init();

            var temp = new Expense
            {
                ID = Guid.NewGuid().ToString(),
                ExpenseAmount = item.ExpenseAmount,
                ExpenseCategory = item.ExpenseCategory,
                ExpenseDate = item.ExpenseDate,
                ExpenseName = item.ExpenseName
            };

            await database.InsertAsync(temp);
            OnItemAdded?.Invoke(this, temp);
        }

        public async Task AddOrUpdate(Expense item)
        {
            await Init();

            var entries = await GetItems();
            if (entries.Count == 0)
            {
                await AddItems(item);
                return;
            }
            // 1. Query db for item.ID.
            var dbItem = entries.Where(dbEntry => dbEntry.ID.Equals(item?.ID)).FirstOrDefault();
            // 2. If ID found, then UpdateItems.
            if (dbItem != null)
                await UpdateItem(item);
            // 3. If ID not found, then new entry, AddItems
            else
                await AddItems(item);
        }

        public async Task DeleteItem(Expense item)
        {
            await Init();
            await database.DeleteAsync(item);
            OnItemDeleted?.Invoke(this, item);
        }

        public async Task<Expense> GetItemByID(string ID)
        {
            await Init();
            return await database.GetAsync<Expense>(ID);
        }

        public async Task<List<Expense>> GetItems()
        {
            await Init();
            return await database.Table<Expense>().ToListAsync();
        }

        public async Task UpdateItem(Expense item)
        {
            await Init();
            await database.UpdateAsync(item);
            OnItemAdded?.Invoke(this, item);
        }
    }
}
