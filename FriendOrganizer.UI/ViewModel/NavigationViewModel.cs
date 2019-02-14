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
            _eventAggregator.GetEvent<AfterFriendDeletedEvent>().Subscribe(AfterFriendDeleted);
        }

        private void AfterFriendDeleted(int friendId)
        {
            var friend = Friends.SingleOrDefault(f=>f.Id==friendId);
            if (friend != null)
            {
                Friends.Remove(friend);
            }
        }

        private void AfterFriendSaved(AfterFriendSavedEventArgs friendItem)
        {
            NavigationItemViewModel friend =Friends.SingleOrDefault(f => f.Id == friendItem.Id);
            if (friend==null)
            {
                Friends.Add(new NavigationItemViewModel(friendItem.Id,friendItem.DisplayMember,_eventAggregator,
                    nameof(FriendDetailViewModel)
                    ));
            }
            else
            {
                friend.DisplayMember = friendItem.DisplayMember;
            }
        }

        public async Task LoadAsync()
        {
            var lookup =await _friendLookupService.GetFriendLookupAsync();
            Friends.Clear();
            foreach (LookupItem friend in lookup)
            {
                Friends.Add(new NavigationItemViewModel(friend.Id,friend.DisplayMember,_eventAggregator,
                    nameof(FriendDetailViewModel)
                    ));
            }
        }
        public ObservableCollection<NavigationItemViewModel> Friends { get;}
    }
}
