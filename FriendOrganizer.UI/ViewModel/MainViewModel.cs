using System.Collections.ObjectModel;
using FriendOrganizer.Model;
using FriendOrganizer.UI.Data;

namespace FriendOrganizer.UI.ViewModel
{
    class MainViewModel:ViewModelBase
    {
        private IFriendDataService _friendDataService;
        private Friend _selectedFrien;

        public MainViewModel(IFriendDataService friendDataService)
        {
            Friends=new ObservableCollection<Friend>();
            _friendDataService = friendDataService;
        }

        public void Load()
        {
            var friends = _friendDataService.GetAll();
            Friends.Clear();
            foreach (Friend friend in friends)
            {
                Friends.Add(friend);
            }
        }

        public ObservableCollection<Friend> Friends { get; set; }

        public Friend SelectedFriend
        {
            get { return _selectedFrien; }
            set
            {
                _selectedFrien = value; 
                OnPropertyChanged();
            }
        }
    }
}
