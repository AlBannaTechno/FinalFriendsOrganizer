using System.Threading.Tasks;
using FriendOrganizer.Model;

namespace FriendOrganizer.UI.Data.Repositories
{
    public interface IGenericRepository<T>
    {
        Task<T> GetByIdAsync(int id);
        Task SaveAsync();
        bool HasChanges();
        void Add(T model);
        void Remove(T model);
    }
}