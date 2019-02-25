using System.Collections.Generic;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.Data.Repositories
{
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
