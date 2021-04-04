using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using personalFinanceManager.interfaces;
using personalFinanceManager.models;
using SQLite;

namespace personalFinanceManager.services
{
    public class IncomeRepository : IRepository<Income>
    {
        static SQLiteAsyncConnection database;

        public event EventHandler<Income> OnItemAdded;
        public event EventHandler<Income> OnItemDeleted;

        public static async Task Init()
        {
            if (database != null)
                return;

            database = DBConnection.Init();

            await database.CreateTableAsync<Income>();
        }

        public async Task AddItems(Income item)
        {
            await Init();

            var temp = new Income
            {
                ID = Guid.NewGuid().ToString(),
                IncomeAmount = item.IncomeAmount,
                IncomeSource = item.IncomeSource
            };

            await database.InsertAsync(temp);
            OnItemAdded?.Invoke(this, temp);
        }

        public async Task AddOrUpdate(Income item)
        {
            await Init();

            // 1. Query db for item.ID.
            var entries = await GetItems();
            if (entries.Count == 0)
            {
                await AddItems(item);
                return;
            }
            var dbItem = entries.Where(dbEntry => dbEntry.ID.Equals(item?.ID)).FirstOrDefault();
            // 2. If ID found, then UpdateItems.
            if (dbItem != null)
                await UpdateItem(item);
            // 3. If ID not found, then new entry, AddItems
            else
                await AddItems(item);
        }

        public async Task DeleteItem(Income item)
        {
            await Init();
            await database.DeleteAsync(item);
            OnItemDeleted?.Invoke(this, item);
        }

        public async Task<Income> GetItemByID(string ID)
        {
            await Init();
            return await database.GetAsync<Income>(ID);
        }

        public async Task<List<Income>> GetItems()
        {
            await Init();
            return await database.Table<Income>().ToListAsync();
        }

        public async Task UpdateItem(Income item)
        {
            await Init();
            await database.UpdateAsync(item);
            OnItemAdded?.Invoke(this, item);
        }
    }
}
