using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using FriendOrganizer.Model.SemiModel;
using FriendOrganizer.UI.Data.Lookups.Shared;
using FriendOrganizer.UI.Event.Shared;
using FriendOrganizer.UI.ViewModel.Backend.Represent;
using FriendOrganizer.UI.ViewModel.Core;
using Prism.Events;

namespace FriendOrganizer.UI.ViewModel.Backend.Container
{
    /**
     * This class created to show navigation With Lookups
     * So this class will be a dataContext of NavigationView
     * Via binding : this binding is an explicit binding in MainWindow.xaml
     *
     * This class should be a backend of left-side bar {NavigationView.xaml}
     * So this class should provide a Lookup collections for any model need navigations
     *  like Friends/Meetings
     *
     * Also : should fire/rais AfterDetailSaved/Delted Events
     *
     * Note : OpenDetailView implemented in NavigationItemViewModel which the model{friends/meetings} collections
     * Consist of 
     */
    public class NavigationViewModel : ViewModelBase, INavigationViewModel
    {
        private readonly IFriendLookupDataService _friendLookupService;
        private readonly IMeetingLookupDataService _meetingLookupDataService;

        private readonly IEventAggregator _eventAggregator;

        public NavigationViewModel(IFriendLookupDataService friendLookupService,
            IMeetingLookupDataService meetingLookupDataService
            , IEventAggregator eventAggregator)
        {
            _friendLookupService = friendLookupService;
            _meetingLookupDataService = meetingLookupDataService;
            _eventAggregator = eventAggregator;
            Friends = new ObservableCollection<NavigationItemViewModel>();
            Meetings = new ObservableCollection<NavigationItemViewModel>();
            _eventAggregator.GetEvent<AfterDetailSavedEvent>().Subscribe(AfterDetailSaved);
            _eventAggregator.GetEvent<AfterDetailDeletedEvent>().Subscribe(AfterDetailDeleted);
        }

        private void AfterDetailDeleted(AfterDetailDeletedEventArgs args)
        {
            switch (args.ViewModelName)
            {
                case nameof(FriendDetailViewModel):
                    AfterDetailDeleted(Friends,args);
                    break;
                case nameof(MeetingDetailViewModel):
                    AfterDetailDeleted(Meetings, args);
                    break;
            }
        }

        private void AfterDetailDeleted(ObservableCollection<NavigationItemViewModel> items, AfterDetailDeletedEventArgs args)
        {
            var item = items.SingleOrDefault(f => f.Id == args.Id);
            if (item != null)
            {
                items.Remove(item);
            }
        }

        private void AfterDetailSaved(AfterDetailSavedEventArgs args)
        {
            switch (args.ViewModelName)
            {
                case nameof(FriendDetailViewModel):
                    AfterDetailSaved(Friends,args);
                    break;
                case nameof(MeetingDetailViewModel):
                    AfterDetailSaved(Meetings, args);
                    break;
            }
        }

        private void AfterDetailSaved(ObservableCollection<NavigationItemViewModel> items, AfterDetailSavedEventArgs args)
        {
            NavigationItemViewModel item = items.SingleOrDefault(f => f.Id == args.Id);
            if (item == null)
            {
                items.Add(new NavigationItemViewModel(args.Id, args.DisplayMember, _eventAggregator,
                    args.ViewModelName
                ));
            }
            else
            {
                item.DisplayMember = args.DisplayMember;
            }
        }

        /**
         * Because of this class will instanciated inside {MainViewModel} :
         * this method {LoadAsync()} will called from {LoadAsync()=>MainViewModel}
         * So Calling Order IS
         * {MainWindo=>OnLoaded}===>{MainViewModel=>LoadAsync()}===>{NavigationViewModel=>LoadAsync()}
         */
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

            lookup = await _meetingLookupDataService.GetMeetingLookupSync();
            Meetings.Clear();
            foreach (LookupItem meeting in lookup)
            {
                Meetings.Add(new NavigationItemViewModel(meeting.Id, meeting.DisplayMember, _eventAggregator,
                    nameof(MeetingDetailViewModel)
                ));
            }
        }

        public ObservableCollection<NavigationItemViewModel> Friends { get; }
        public ObservableCollection<NavigationItemViewModel> Meetings { get; }
    }
}
