namespace FriendOrganizer.UI.ViewModel.Helper
{
    /*
     * This is just a helper class to implement iNotifyPropertyChanged instead of implement it
     * Inside LookupItem model class
     * Because we need Models to be fully isolated
     */
    public class NavigationItemViewModel:ViewModelBase
    {

        public NavigationItemViewModel(int id,string displayMember)
        {
            Id = id;
            DisplayMember = displayMember;
        }
        public int Id { get; }
        private string _displayMember;

        public string DisplayMember
        {
            get => _displayMember;
            set
            {
                _displayMember = value;
                OnPropertyChanged();
            }
        }
    }
}
