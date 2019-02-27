using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Autofac.Features.Indexed;
using FriendOrganizer.UI.Event.Shared;
using FriendOrganizer.UI.Services.Shared;
using Prism.Commands;
using Prism.Events;

namespace FriendOrganizer.UI.ViewModel.Core
{
    /**
     * This class is the main class of this project Because this will pass as an argument to
     * MainWindow constructor And This class will binding to MainWindow.xaml/cs=> DataContext
     *
     * This class designed to controll every thing on the main window
     * like : navigation {create/remove}.. and support multiNavigation
     */
    public class MainViewModel : ViewModelBase
    {
        /**
         * _selectedDetailViewModel : we use IDetailViewModel interface because of all DetailViewModels implement it
         * => this field : is a backend field for SelectedDetailViewModel property
         * So this property will bind to SelectedItem => TabControl : in MainWindow.xaml [to support track current  selected tab]
         *
         */
        private IDetailViewModel _selectedDetailViewModel;

        /**
         * Just a messageDialogService field to support showing messages to user
         */
        private readonly IMessageDialogService _messageDialogService;

        /**
         * This field created as IIndex dictionary of string => viewModelName , IDetailViewModel type => ViewModel class
         * To use this indexer we need to register All IDetailViewModel implementers {classes implemented IDetailViewModel}
         *  As {for example}=>  builder.RegisterType<FriendDetailViewModel>().Keyed<IDetailViewModel>(nameof(FriendDetailViewModel));
         * So after that we can use index => _detailViewModelCreator[viewModelName] to get an instance of this viewModel
         *
         * We provide this indexer to prevent overflow the constructor parameter
         * because if we just pass IDetailViewModel implementers to the constructor
         * we must add new paremter for each new DetailView : and this is very un reable solution so we use IIndex => autofac indexer
         *
         */
        private readonly IIndex<string, IDetailViewModel> _detailViewModelCreator;

        /**
         * This as an EventAggregator from Prism
         * we use it to listen to publish events and subscribe to it base on predefined events on
         *      => FriendOrganizer.UI.Event.Shared directory
         * Note : when we register this event we commanded autofac to get the same instance every once we request it
         *  and this require to make it work across the application
         *
         */
        private readonly IEventAggregator _eventAggregator;

        /**
         * The constructor 
         */
        public MainViewModel(INavigationViewModel navigationViewModel,
            
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService,
            IIndex<string,IDetailViewModel> detailViewModelCreator)
        {
            NavigationViewModel = navigationViewModel;
            _eventAggregator = eventAggregator;
            _messageDialogService = messageDialogService;
            _detailViewModelCreator = detailViewModelCreator;

            /**
             * Subscribe to OpenDetailViewEvent : this event will publish from NavigationItemViewModel=>OpenDetailViewCommand
             * and we will use it to create a new DetailViewModel to DetailViewModels collection [see definition below]
             */
            DetailViewModels = new ObservableCollection<IDetailViewModel>();
            _eventAggregator.GetEvent<OpenDetailViewEvent>()
                .Subscribe(OnOpenDetailView);

            /**
             * Subscribe to AfterDetailDeletedEvent : this event will publish from DetailViewModelBase => RaisDetailDeletedEvent
             *      Which related to DeleteCommand inside it
             *      Note : {DetailViewModelBase class} is The main Parent of all DetialViewModels => [Model]DetailViewModel
             *          So all those subClasses which a Backend of the view {DataContext} will rais this event
             *          when DeleteCommand executed
             */
            _eventAggregator.GetEvent<AfterDetailDeletedEvent>()
                .Subscribe(AfterDetailDeleted);

            /**
             * Like Previous [AfterDetailDeletedEvent] but related to [CloseDetailViewCommand]
             */
            _eventAggregator.GetEvent<AfterDetailClosedEvent>()
                .Subscribe(AfterDetailClosed);

            /**
             * Initialize CreateNewDetailCommand with generic type of Type [C# Type]
             * we make this generic because we will pass the type of the DetailViewModel we need to create it from
             * The View [MainWindow] directly as CommandParameter
             *      eg. CommandParameter="{x:Type viewModelRepresent:FriendDetailViewModel}
             *
             * Also notic the way we implement OnCreateNewDetailExecute : will allow to add multible
             * new windows/[view-ViewModel]s at the same time
             */
            CreateNewDetailCommand = new DelegateCommand<Type>(OnCreateNewDetailExecute);

            /**
             * Initialize OpenSingleDetailViewCommand
             * the difference between this and CreateNewDetailCommand : that this will not allow
             * to initialize many Windows[view-model]s with the same type
             * so we should use it when we need to restrict user to just show single view from the type we want
             */
            OpenSingleDetailViewCommand = new DelegateCommand<Type>(OnOpenSingleDetailViewExecute);
        }

      


        private void AfterDetailDeleted(AfterDetailDeletedEventArgs args)
        {
            if (args.Id != null) RemoveDetailViewModel(args.Id.Value, args.ViewModelName);
        }

        private void AfterDetailClosed(AfterDetailClosedEventArgs args)
        {
            RemoveDetailViewModel(args.Id, args.ViewModelName);
        }

        /**
         * RemoveDetailViewModel : Notic this method just remove the detailViewModel from DetailViewModels collection
         *  not the entity[Db] it self :
         *  So we shared it with AfterDetailDeleted/AfterDetailClosed because either entity deleted
         *  or user decide to close this detailWindow we need to do the same behaviour : just close the window
         */
        private void RemoveDetailViewModel(int id,string viewModelName)
        {
            /**
             * Get the desired detailViewModel by it's id and viewModelName
             * Because if we used just id may be the entites from different tables has the same id
             * but this is impossible with thee entities at the same table
             * and we use separate viewModel for very single table
             * so we need to query with viewModelName
             */
            var detailViewModel = DetailViewModels
                .SingleOrDefault(vm => vm.Id == id && vm.GetType().Name == viewModelName);
            if (detailViewModel != null)
            {
                DetailViewModels.Remove(detailViewModel);
            }
        }

        /**
         * Load All Navigation items
         * This function use NavigationViewModel => LoadAsync()  to get these data because this class {MainViewModel}
         * is just a container so loading details not implemente here
         *
         * Also Note : this method should called in MainWindow.cs after loading it => LoadAsync() <=> Loaded [event]
         */
        public async Task LoadAsync()
        {
            await NavigationViewModel.LoadAsync();
        }

        public ICommand CreateNewDetailCommand { get; }
        public ICommand OpenSingleDetailViewCommand { get; }

        /**
         * This will bind as DataContext to NavigationView
         */
        public INavigationViewModel NavigationViewModel { get; }

        /**
         * DetailViewModels Collection to support show multible items/tabs
         * this will bind to ItemsSource of multiPage Control eg. TabControl
         */
        public ObservableCollection<IDetailViewModel> DetailViewModels { get;}

        /**
         * Selected Detail ViewModel : to track current selected view/viewModel
         * this will bind to SelectedItem of multiPage Control eg. TabControl
         *
         * And used essentially to active specific view at some cases :
         *  1- if we create new View
         *  2- if we select currently opened view from Navigation view
         *      we will just active it : no need to create it agian
         */
        public IDetailViewModel SelectedDetailViewModel
        {
            get => _selectedDetailViewModel;
            set
            {
                _selectedDetailViewModel = value;
                OnPropertyChanged();
            }
        }

        /**
         * This method to open detail view : [new , currentOpened , SingleType View ,....]
         */
        private async void OnOpenDetailView(OpenDetailViewEventArgs args)
        {
            /**
             * Query the detailViewModel by id and ViewModelName
             */
            var detailViewModel = DetailViewModels
                .SingleOrDefault(vm => vm.Id == args.Id && vm.GetType().Name == args.ViewModelName);

            // IF not exist : thats means we will create a new DetailViewModel
            if (detailViewModel==null)
            {
                // get a new instance of DetailViewModel by it's name With autofac IIndex dictionary
                detailViewModel= _detailViewModelCreator[args.ViewModelName];
                try
                {
                    // try to load detailViewModel contents from The DataBase Before preview it
                    await detailViewModel.LoadAsync(args.Id);
                }
                catch
                {
                    /*
                     * if this faild and Connection to db still exist [always true]
                     * thats mean this entity deleted from the database because we have it's id
                     * and it's not in the Db
                     * So we will show the next message then reload The NavigationViewModel Data
                     * to remove any non-valid/existed entities
                     * Note : This scenario desined to Multi-users case
                     */
                    await _messageDialogService.ShowInfoDialogAsync("Coudl not load the entity ," +
                                "May be it was deleted in the meantime by another user" +
                                "The navigation will refreshed for you");
                    await NavigationViewModel.LoadAsync();
                    // then return : no need to add this non valid detailViewModel to DetailViewModels Collection
                    return;
                }

                // if fetch entity success we just add the new detailViewModel to DetailViewModels Collection
                DetailViewModels.Add(detailViewModel);
            }

            /**
             * in all cases we need to change SelectedDetailViewModel to the requested  detailViewModel
             *  1- if it the same : nothing changed
             *  2- if it new we will move the foucs to the new window/tab
             *  3- if it exist but not in focus we will just move the focus to it
             */
            SelectedDetailViewModel = detailViewModel;

        }

        /**
         * next item id : to support open/create multible new empty view/[detailViewModel] of the same type
         * so we will decrement it every once we create new empty detailViewModel
         * so all ids of new detailViewModels will be negative (-) and when we save it this value will change
         *  By OnSaveExecute() from DetailViewModelBase abstract class : OnSaveExecute was implemented in
         *      new subClassess of DetailViewModelBase because this method is  an abstract
         */
        private int _nextItemNewId = 0;

        private void OnCreateNewDetailExecute(Type viewModelType)
        {
            /**
             * provide variable id : to support : open/create multible new empty view/[detailViewModel] of the same type
             */
            OnOpenDetailView(new OpenDetailViewEventArgs() {Id = _nextItemNewId--, ViewModelName = viewModelType.Name });
        }

        private void OnOpenSingleDetailViewExecute(Type viewModelType)
        {
            // provide the same id every once to open the same instance
            OnOpenDetailView(new OpenDetailViewEventArgs() { Id = -1, ViewModelName = viewModelType.Name });
        }
    }
}
