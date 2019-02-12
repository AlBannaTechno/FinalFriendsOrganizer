using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using FriendOrganizer.Model;
using FriendOrganizer.UI.Data;
using FriendOrganizer.UI.Data.Lookups;
using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.ViewModel.Helper;
using Prism.Events;

namespace FriendOrganizer.UI.ViewModel
{
    public class NavigationViewModel : ViewModelBase,INavigationViewModel
    {
        private readonly IFriendLookupDataService _friendLookupService;
        private IEventAggregator _eventAggregator;

        public NavigationViewModel(IFriendLookupDataService friendLookupService,IEventAggregator eventAggregator)
        {
            _friendLookupService = friendLookupService;
            _eventAggregator = eventAggregator;
            Friends =new ObservableCollection<NavigationItemViewModel>();
            _eventAggregator.GetEvent<AfterFriendSavedEvent>().Subscribe(AfterFriendSaved);
        }

        private void AfterFriendSaved(AfterFriendSavedEventArgs friendItem)
        {
            NavigationItemViewModel friend =Friends.Single(f => f.Id == friendItem.Id);
            friend.DisplayMember = friendItem.DisplayMember;
        }

        public async Task LoadAsync()
        {
            var lookup =await _friendLookupService.GetFriendLookupAsync();
            Friends.Clear();
            foreach (LookupItem friend in lookup)
            {
                Friends.Add(new NavigationItemViewModel(friend.Id,friend.DisplayMember,_eventAggregator));
            }
        }
        public ObservableCollection<NavigationItemViewModel> Friends { get;}
    }
}
