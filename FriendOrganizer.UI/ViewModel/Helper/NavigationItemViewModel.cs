using System.Windows.Input;
using FriendOrganizer.UI.Event;
using Prism.Commands;
using Prism.Events;

namespace FriendOrganizer.UI.ViewModel.Helper
{
    /*
     * This is just a helper class to implement iNotifyPropertyChanged instead of implement it
     * Inside LookupItem model class
     * Because we need Models to be fully isolated
     */
    public class NavigationItemViewModel:ViewModelBase
    {

        public NavigationItemViewModel(int id,string displayMember,IEventAggregator eventAggregator)
        {
            Id = id;
            DisplayMember = displayMember;
            _eventAggregator = eventAggregator;
            OpenFriendDetailViewCommand =new DelegateCommand(OnOpenFriendDetailView);
        }

        private void OnOpenFriendDetailView()
        {
            _eventAggregator.GetEvent<OpenFriendDetailViewEvent>()
                .Publish(Id);
        }

        public int Id { get; }
        private string _displayMember;
        private IEventAggregator _eventAggregator;

        public string DisplayMember
        {
            get => _displayMember;
            set
            {
                _displayMember = value;
                OnPropertyChanged();
            }
        }
        public ICommand OpenFriendDetailViewCommand { get; }
    }
}
