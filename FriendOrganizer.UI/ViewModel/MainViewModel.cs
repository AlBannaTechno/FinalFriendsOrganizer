
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.Services;
using Prism.Commands;
using Prism.Events;

namespace FriendOrganizer.UI.ViewModel
{
    public class MainViewModel:ViewModelBase
    {
        private IEventAggregator _eventAggregator;
        private IDetailViewModel _detailViewModel;
        private IMessageDialogService _messageDialogService;
        public Func<IFriendDetailViewModel> FriendDetailViewModelCreator { get; set; }


        public MainViewModel(INavigationViewModel navigationViewModel,
            Func<IFriendDetailViewModel> friendDetailViewModelCreatorCreator, IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService)
        {
            NavigationViewModel = navigationViewModel;
            _eventAggregator = eventAggregator;
            _messageDialogService = messageDialogService;
            _eventAggregator.GetEvent<OpenFriendDetailViewEvent>()
                .Subscribe(OnOpenFriendDetailView);
            _eventAggregator.GetEvent<AfterFriendDeletedEvent>()
                .Subscribe(AfterFriendDeleted);

            FriendDetailViewModelCreator = friendDetailViewModelCreatorCreator;

            CreateNewFriendCommand=new DelegateCommand(OnCreateNewFriendExecute);
        }

        private void AfterFriendDeleted(int obj)
        {
            DetailViewModel = null;
        }

        public async Task LoadAsync()
        {
            await NavigationViewModel.LoadAsync();
        }

        public ICommand CreateNewFriendCommand { get; }

        public INavigationViewModel NavigationViewModel { get; }

        public IDetailViewModel DetailViewModel
        {
            get => _detailViewModel;
            private set
            {
                _detailViewModel = value; 
                OnPropertyChanged();
            }
        }

        private async void OnOpenFriendDetailView(int? friendId)
        {
            if (DetailViewModel != null && DetailViewModel.HasChanges)
            {
                var result = _messageDialogService.ShowOkCancelDialog("You Have Made changes ! Go AnyWay ?","Changes Lose Warning");
                if(result==MessageDialogResult.Cancel)
                    return;
            }
            DetailViewModel = FriendDetailViewModelCreator();
            await DetailViewModel.LoadAsync(friendId);
        }

        private void OnCreateNewFriendExecute()
        {
            OnOpenFriendDetailView(null);
        }
    }
}
