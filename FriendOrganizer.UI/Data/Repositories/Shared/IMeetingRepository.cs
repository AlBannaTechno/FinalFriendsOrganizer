using System.Collections.Generic;
using System.Threading.Tasks;
using FriendOrganizer.Model.Model;
using FriendOrganizer.UI.Data.Repositories.Core;

namespace FriendOrganizer.UI.Data.Repositories.Shared
{
    public interface IMeetingRepository:IGenericRepository<Meeting>
    {
        Task<List<Friend>> GetAllFriendsAsync();
        Task ReloadFriendAsync(int friendId);
    }
}