﻿
using System.Threading.Tasks;

namespace FriendOrganizer.UI.ViewModel
{
    public class MainViewModel:ViewModelBase
    {


        public MainViewModel(INavigationViewModel navigationViewModel,
            IFriendDetailViewModel friendDetailViewModel)
        {
            NavigationViewModel = navigationViewModel;
            FriendDetailViewModel = friendDetailViewModel;
        }

        public async Task LoadAsync()
        {
            await NavigationViewModel.LoadAsync();
        }

        public INavigationViewModel NavigationViewModel { get; }
        public IFriendDetailViewModel FriendDetailViewModel { get; }
    }
}
