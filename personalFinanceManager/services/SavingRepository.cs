using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using personalFinanceManager.interfaces;
using personalFinanceManager.models;
using SQLite;

namespace personalFinanceManager.services
{
    public class SavingRepository : IRepository<Saving>
    {
        static SQLiteAsyncConnection database;

        public event EventHandler<Saving> OnItemAdded;
        public event EventHandler<Saving> OnItemDeleted;

        public static async Task Init()
        {
            if (database != null)
                return;

            database = DBConnection.Init();

            await database.CreateTableAsync<Saving>();
        }

        public async Task AddItems(Saving item)
        {
            await Init();

            Saving temp = new Saving
            {
                ID = Guid.NewGuid().ToString(),
                SavingAmount = item.SavingAmount,
                SavingDate = item.SavingDate
            };

            await database.InsertAsync(temp);
            OnItemAdded?.Invoke(this, temp);
        }

        public async Task AddOrUpdate(Saving item)
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

        public async Task DeleteItem(Saving item)
        {
            await Init();
            await database.DeleteAsync(item);
            OnItemDeleted?.Invoke(this, item);
        }

        public Task<Saving> GetItemByID(string ID)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Saving>> GetItems()
        {
            await Init();
            return await database.Table<Saving>().ToListAsync();
        }

        public async Task UpdateItem(Saving item)
        {
            await Init();
            await database.UpdateAsync(item);
            OnItemAdded?.Invoke(this, item);
        }
    }
}
