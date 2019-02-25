using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.Services;
using Prism.Commands;
using Prism.Events;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Autofac.Features.Indexed;

namespace FriendOrganizer.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private IDetailViewModel _detailViewModel;
        private readonly IMessageDialogService _messageDialogService;
        private readonly IIndex<string, IDetailViewModel> _detailViewModelCreator;

        public MainViewModel(INavigationViewModel navigationViewModel,
            
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService,
            IIndex<string,IDetailViewModel> detailViewModelCreator
            )
        {
            NavigationViewModel = navigationViewModel;
            var eventAggregator1 = eventAggregator;
            _messageDialogService = messageDialogService;
            _detailViewModelCreator = detailViewModelCreator;
            eventAggregator1.GetEvent<OpenDetailViewEvent>()
                .Subscribe(OnOpenDetailView);
            eventAggregator1.GetEvent<AfterDetailDeletedEvent>()
                .Subscribe(AfterDetailDeleted);


            CreateNewDetailCommand = new DelegateCommand<Type>(OnCreateNewDetailExecute);
        }

        private void AfterDetailDeleted(AfterDetailDeletedEventArgs args)
        {
            DetailViewModel = null;
        }

        public async Task LoadAsync()
        {
            await NavigationViewModel.LoadAsync();
        }

        public ICommand CreateNewDetailCommand { get; }

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

        private async void OnOpenDetailView(OpenDetailViewEventArgs args)
        {
            if (DetailViewModel != null && DetailViewModel.HasChanges)
            {
                var result = _messageDialogService.ShowOkCancelDialog("You Have Made changes ! Go AnyWay ?", "Changes Lose Warning");
                if (result == MessageDialogResult.Cancel)
                    return;
            }

            DetailViewModel = _detailViewModelCreator[args.ViewModelName];

            if (DetailViewModel != null) await DetailViewModel.LoadAsync(args.Id);
        }

        private void OnCreateNewDetailExecute(Type viewModelType)
        {
            OnOpenDetailView(new OpenDetailViewEventArgs() { ViewModelName = viewModelType.Name });
        }
    }
}
