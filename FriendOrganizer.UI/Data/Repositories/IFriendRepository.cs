using System.Threading.Tasks;
using FriendOrganizer.Model;
using FriendOrganizer.Model.Model;

namespace FriendOrganizer.UI.Data.Repositories
{
    public interface IFriendRepository : IGenericRepository<Friend>
    {
        void RemovePhoneNumber(FriendPhoneNumber model);

        Task<bool> HasMeetingsAsync(int friendId);
    }
}
