using System.Collections.Generic;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Data.Repositories.Core
{
    /**
     * Generic Repository That have general methods used with any repository
     * that provid any data
     */
    public interface IGenericRepository<T>
    {
        Task<T> GetByIdAsync(int id);

        Task SaveAsync();

        Task<IEnumerable<T>> GetAllAsync();

        bool HasChanges();

        void Add(T model);

        void Remove(T model);
    }
}
