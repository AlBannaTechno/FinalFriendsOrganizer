using System.Collections.Generic;
using System.Linq;
using FriendOrganizer.DataAccess;
using FriendOrganizer.Model;

namespace FriendOrganizer.UI.Data
{
    class FriendDataService : IFriendDataService
    {
        public IEnumerable<Friend> GetAll()
        {
            using (var ctx=new FriendOrganizerDbContext())
            {
                return ctx.Friends.AsNoTracking().ToList();
            }
        }
    }
}
