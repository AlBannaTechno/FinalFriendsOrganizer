using System.Threading.Tasks;
using FriendOrganizer.Model.Model;
using FriendOrganizer.UI.Data.Repositories.Core;

namespace FriendOrganizer.UI.Data.Repositories.Shared
{
    public interface IFriendRepository : IGenericRepository<Friend>
    {
        void RemovePhoneNumber(FriendPhoneNumber model);

        Task<bool> HasMeetingsAsync(int friendId);
    }
}
