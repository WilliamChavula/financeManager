using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace personalFinanceManager.interfaces
{
    public interface IRepository<T>
    {
        event EventHandler<T> OnItemAdded;
        event EventHandler<T> OnItemDeleted;

        Task<List<T>> GetItems();
        Task AddItems(T item);
        Task UpdateItem(T item);
        Task AddOrUpdate(T item);
        Task DeleteItem(T item);
        Task<T> GetItemByID(string ID);
    }
}
