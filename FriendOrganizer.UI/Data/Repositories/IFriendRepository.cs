using FriendOrganizer.Model;

namespace FriendOrganizer.UI.Data.Repositories
{
    public interface IFriendRepository:IGenericRepository<Friend>
    {
        void RemovePhoneNumber(FriendPhoneNumber model);
    }
}