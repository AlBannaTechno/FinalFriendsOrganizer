using System.Threading.Tasks;

namespace FriendOrganizer.UI.ViewModel.Core
{
    public interface IDetailViewModel
    {
        Task LoadAsync(int id);

        bool HasChanges { get; }
        int Id { get;}
    }
}
