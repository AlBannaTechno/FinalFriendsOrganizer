using FriendOrganizer.Model;
using FriendOrganizer.UI.Data.Lookups;
using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.ViewModel.Helper;
using Prism.Events;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.ViewModel
{
    public class NavigationViewModel : ViewModelBase, INavigationViewModel
    {
        private readonly IFriendLookupDataService _friendLookupService;

        private IEventAggregator _eventAggregator;

        public NavigationViewModel(IFriendLookupDataService friendLookupService, IEventAggregator eventAggregator)
        {
            _friendLookupService = friendLookupService;
            _eventAggregator = eventAggregator;
            Friends = new ObservableCollection<NavigationItemViewModel>();
            _eventAggregator.GetEvent<AfterDetailSavedEvent>().Subscribe(AfterDetailSaved);
            _eventAggregator.GetEvent<AfterDetailDeletedEvent>().Subscribe(AfterDetailDeleted);
        }

        private void AfterDetailDeleted(AfterDetailDeletedEventArgs args)
        {
            switch (args.ViewModelName)
            {
                case nameof(FriendDetailViewModel):
                    var friend = Friends.SingleOrDefault(f => f.Id == args.Id);
                    if (friend != null)
                    {
                        Friends.Remove(friend);
                    }
                    break;
            }
        }

        private void AfterDetailSaved(AfterDetailSavedEventArgs args)
        {
            switch (args.ViewModelName)
            {
                case nameof(FriendDetailViewModel):
                    NavigationItemViewModel friend = Friends.SingleOrDefault(f => f.Id == args.Id);
                    if (friend == null)
                    {
                        Friends.Add(new NavigationItemViewModel(args.Id, args.DisplayMember, _eventAggregator,
                            nameof(FriendDetailViewModel)
                        ));
                    }
                    else
                    {
                        friend.DisplayMember = args.DisplayMember;
                    }
                    break;
            }
        }

        public async Task LoadAsync()
        {
            var lookup = await _friendLookupService.GetFriendLookupAsync();
            Friends.Clear();
            foreach (LookupItem friend in lookup)
            {
                Friends.Add(new NavigationItemViewModel(friend.Id, friend.DisplayMember, _eventAggregator,
                    nameof(FriendDetailViewModel)
                    ));
            }
        }

        public ObservableCollection<NavigationItemViewModel> Friends { get; }
    }
}
