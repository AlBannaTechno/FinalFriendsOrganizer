using System.Collections.Generic;
using System.Threading.Tasks;
using FriendOrganizer.Model.SemiModel;

namespace FriendOrganizer.UI.Data.Lookups.Shared
{
    public interface IProgrammingLanguageLookupDataService
    {
        Task<IEnumerable<LookupItem>> GetProgrammingLanguageLookupAsync();
    }
}
