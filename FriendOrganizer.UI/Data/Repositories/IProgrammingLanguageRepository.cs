using System.Threading.Tasks;
using FriendOrganizer.Model;

namespace FriendOrganizer.UI.Data.Repositories
{
    public interface IProgrammingLanguageRepository:IGenericRepository<ProgrammingLanguage>
    {
        Task<bool> IsReferencedByFriendAsync(int programmingLanguageId);
    }
}