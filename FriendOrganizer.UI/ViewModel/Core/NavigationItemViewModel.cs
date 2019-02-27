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
     *
     * Note : this class should used as a virtual wrapper of Lookups
     *  thats mean we will not implement LookupItem SemiModel here
     *  But we will use this {NavigationItemViewModel} inside navigation view instead of lookupItem model
     *  so when we fetch data from Db With {I[Model]ookupDataService.Get[Model]LookupAsync()} this will
     *  return LookupItem SemiModel Type and we will manually cast it to NavigationItemViewModel
     *      Because we this type {NavigationItemViewModel} will bind to The View Not LookupItem type
     *
     * The Feature of this class :
     *  Has : OpenDetailViewCommand : to execcute when we need to open the view which instance summary it
     *      Because when OpenDetailViewCommand executed OnOpenDetailViewExecute() will execcuted
     *      and this method we implement it ot publish an event of type OpenDetailViewEvent
     *      supplied with :
     *
     *          1- ViewModelName   => to use it to create new instance of this view model With IIndex
     *              See the implementaion in MainViewModel => OnOpenDetailView => this function will subscribed to
     *                  OpenDetailViewEvent
     *
     *          2- Id              => we will use it to fetch data from The Db
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

        /**
         * Backend field of DisplayMember property
         * used to show a name of lookup
         */
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
