using System.Collections.Generic;
using FriendOrganizer.Model;

namespace FriendOrganizer.UI.Data
{
    interface IFriendDataService
    {
        IEnumerable<Friend> GetAll();
    }
}