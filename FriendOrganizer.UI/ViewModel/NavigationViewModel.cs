using System.Collections.ObjectModel;
using System.Threading.Tasks;
using FriendOrganizer.Model;
using FriendOrganizer.UI.Data;
using FriendOrganizer.UI.Event;
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
            Friends =new ObservableCollection<LookupItem>();
        }

        public async Task LoadAsync()
        {
            var lookup =await _friendLookupService.GetFriendLookupAsync();
            Friends.Clear();
            foreach (LookupItem friend in lookup)
            {
                Friends.Add(friend);
            }
        }
        public ObservableCollection<LookupItem> Friends { get;}

        private LookupItem _selectedFriend;

        public LookupItem SelectedFriend
        {
            get => _selectedFriend;
            set
            {
                _selectedFriend = value;
                OnPropertyChanged();
                if (_selectedFriend!=null)
                {
                    _eventAggregator.GetEvent<OpenFriendDetailViewEvent>()
                        .Publish(_selectedFriend.Id);
                }
            }
        }
    }
}
