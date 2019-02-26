using System.Data.Entity;
using System.Threading.Tasks;
using FriendOrganizer.DataAccess;
using FriendOrganizer.Model.Model;
using FriendOrganizer.UI.Data.Repositories.Core;

namespace FriendOrganizer.UI.Data.Repositories.Shared
{
    public class ProgrammingLanguageRepository : GenericRepository<ProgrammingLanguage,FriendOrganizerDbContext>,IProgrammingLanguageRepository
    {
        public ProgrammingLanguageRepository(FriendOrganizerDbContext context) : base(context)
        {
        }

        public async Task<bool> IsReferencedByFriendAsync(int programmingLanguageId)
        {
            return await Context.Friends.AsNoTracking().AnyAsync(f => f.FavoriteLanguageId == programmingLanguageId);
        }
    }
}
