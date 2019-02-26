using System.Collections.Generic;
using System.Threading.Tasks;
using FriendOrganizer.Model.SemiModel;

namespace FriendOrganizer.UI.Data.Lookups.Shared
{
    public interface IFriendLookupDataService
    {
        Task<IEnumerable<LookupItem>> GetFriendLookupAsync();
    }
}
