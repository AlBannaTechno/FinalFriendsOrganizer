using System.Windows.Input;
using FriendOrganizer.UI.Event.Shared;
using Prism.Commands;
using Prism.Events;

namespace FriendOrganizer.UI.ViewModel.Core
{
    /*
     * This is just a helper class to implement iNotifyPropertyChanged instead of implement it
     * Inside LookupItem model class
     * Because we need Models to be fully isolated
     */
    public class NavigationItemViewModel : ViewModelBase
    {
        public NavigationItemViewModel(int id, string displayMember, IEventAggregator eventAggregator,
            string detailViewModelName)
        {
            Id = id;
            DisplayMember = displayMember;
            _eventAggregator = eventAggregator;
            _detailViewModelName = detailViewModelName;
            OpenDetailViewCommand = new DelegateCommand(OnOpenDetailViewExecute);
        }

        private void OnOpenDetailViewExecute()
        {
            _eventAggregator.GetEvent<OpenDetailViewEvent>()
                .Publish(new OpenDetailViewEventArgs()
                {
                    Id = Id,
                    ViewModelName = _detailViewModelName

                });
        }

        public int Id { get; }

        private string _displayMember;

        private readonly IEventAggregator _eventAggregator;

        private readonly string _detailViewModelName;

        public string DisplayMember
        {
            get => _displayMember;
            set
            {
                _displayMember = value;
                OnPropertyChanged();
            }
        }

        public ICommand OpenDetailViewCommand { get; }
    }
}
