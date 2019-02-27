using System.Threading.Tasks;

namespace FriendOrganizer.UI.ViewModel.Core
{
    /**
     * This interface created to be a base for all class that represent detailView
     *  => detailView : any class{ViewModel} represent [all/most of] model properites
     *      so here we has and Id of the model entity and LoadAsync to load the properites/fields
     *      and HasChanges to check if there are any changes has been happend to the entity
     */
    public interface IDetailViewModel
    {
        Task LoadAsync(int id);

        bool HasChanges { get; }
        int Id { get;}
    }
}
