using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using FriendOrganizer.UI.Event.Shared;
using FriendOrganizer.UI.Services.Shared;
using Prism.Commands;
using Prism.Events;

namespace FriendOrganizer.UI.ViewModel.Core
{
    /**
     * This class created to be a base class for all classes that represent detailView
     * This class should have next Features
     *  Support Saving changes
     *  Support Delete entites
     *  Support Close DetailView or rais an event to do that
     *  
     */
    public abstract class DetailViewModelBase : ViewModelBase, IDetailViewModel
    {
        private bool _hasChanges;

        protected readonly IEventAggregator EventAggregator;
        protected readonly IMessageDialogService MessageDialogService;
        private  int _id;

        /*
         * This tilte may used to show in tab header
         */
        private string _title;

        protected DetailViewModelBase(IEventAggregator eventAggregator,IMessageDialogService messageDialogService)
        {
            EventAggregator = eventAggregator;
            MessageDialogService = messageDialogService;
            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
            DeleteCommand = new DelegateCommand(OnDeleteExecute);
            CloseDetailViewCommand=new DelegateCommand(OnCloseDetailViewExecute);
        }


        public ICommand CloseDetailViewCommand { get; }

        #region Abstracts Methods

        protected abstract void OnDeleteExecute();

        protected abstract bool OnSaveCanExecute();

        protected abstract void OnSaveExecute();

        public abstract Task LoadAsync(int id);

        #endregion


        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        public bool HasChanges
        {
            get => _hasChanges;
            set
            {
                if (_hasChanges != value)
                {
                    _hasChanges = value;
                    OnPropertyChanged();
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
            }
        }

        // this id must set inside any class inherited from this class like : 
        
        public int Id
        {
            get => _id;
            protected set => _id = value;
        }

        public ICommand SaveCommand { get; private set; }

        public ICommand DeleteCommand { get; private set; }


        /**
         *  TODO : DOCs
         *
         */
        protected virtual void RaisCollectionSavedEvent()
        {
            EventAggregator.GetEvent<AfterCollectionSavedEvent>().Publish(new AfterCollectionSavedEventArgs()
            {
                ViewModelName = this.GetType().Name
            });
        }

        /**
         * This function must called from implementaion of OnDeleteExecute()
         * Also we should subscribe to AfterDetailDeletedEvent : at [MainViewModel]
         *  Or any place will Contains The View Or the collection which contains the view
         *      Which it's backend [DataContext] inherited from this class : DetailViewModelBase
         *  Like : [ProgrammingLanguagedDetailViewModel,FriendDetailViewModel,MeetingDetailViewModel] 
         */
        protected virtual void RaisDetailDeletedEvent(int modelId)
        {
            EventAggregator.GetEvent<AfterDetailDeletedEvent>().Publish(
                new AfterDetailDeletedEventArgs()
                {
                    Id = modelId,
                    // next will get type of the current instance [the sub class]
                    ViewModelName = this.GetType().Name
                }
                );
        }

        /**
         * This function must called from implementaion of OnSaveExecute()
         * AfterDetailSavedEvent : should subscribed from any ViewModel one or more of it's fields
         * depending on values from DetailViewModelBase childs/subClasses
         *  and this like NavigationViewModel : which has Friends/Meetings collections of NavigationItemViewModel
         *  which get it's values from Lookupitem fetched entites from Db and view it with DisplayMember property
         *  So this property may chaged if we change values of entity/[Db-model instance]
         *   for that NavigationViewModel must subscribe to AfterDetailSavedEvent
         * 
         */
        protected virtual void RaisDetailSavedEvent(int modelId, string displayMember)
        {
            EventAggregator.GetEvent<AfterDetailSavedEvent>().Publish(
                new AfterDetailSavedEventArgs()
                {
                    Id = modelId,
                    // next will get type of the current instance [the sub class]
                    ViewModelName = this.GetType().Name,
                    DisplayMember = displayMember
                }
            );
        }

        /**
         * To Execute When CloseDetailViewCommand executed
         */
        protected virtual async void OnCloseDetailViewExecute()
        {
            if (HasChanges)
            {
                var result =
                    await MessageDialogService.ShowOkCancelDialogAsync("You've made changes . Close this item ?", "Question");
                if (result==MessageDialogResult.Cancel)
                {
                    return;
                }
            }

            /**
             * publish AfterDetailClosedEvent
             * This evetn should subscribed from The ViewModel which contain the instance of the class implemented
             *      this class => DetailViewModelBase , or continers containe the instance ........||...
             *  Like : [ProgrammingLanguagedDetailViewModel,FriendDetailViewModel,MeetingDetailViewModel]
             *
             * And That class is like MainViewModel
             */
            EventAggregator.GetEvent<AfterDetailClosedEvent>().Publish(new AfterDetailClosedEventArgs()
            {
                Id = this.Id,
                ViewModelName = this.GetType().Name
            });
        }

        /**
         * This method designed to call from the subClass with pass saveFunc as save function
         *  contains the logic of saving for this custom subClass
         * And afterSaveAction : to do some stuff like update HasChanges
         *  and invoke RaisDetailSavedEvent()
         */
        protected async Task SaveWithOptimisticConcurrencyAsync(Func<Task> saveFunc,Action afterSaveAction)
        {
            try
            {
                await saveFunc();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var databaseValues = ex.Entries.Single().GetDatabaseValues();
                if (databaseValues == null)
                {
                    await MessageDialogService.ShowInfoDialogAsync("The Entity has been Deleted by another user");
                    RaisDetailDeletedEvent(Id);
                    return;
                }
                var result = await MessageDialogService.ShowOkCancelDialogAsync("The entity changed from another uses" +
                                                                     "Click Ok to save your version any way or Cancel to get the new value from Db", "Warning");

                if (result == MessageDialogResult.Ok)
                {
                    // TODO : Need to undetstand next lines : how we save data to the database here !???
                    // Update the original value with database value [save this value to database] => Client Wins
                    var entity = ex.Entries.Single(); // current entity
                    // set current entity values to values from database
                    entity.OriginalValues.SetValues(entity.GetDatabaseValues());
                    await saveFunc();
                }
                else
                {
                    // Reload entity from database
                    await ex.Entries.Single().ReloadAsync();
                    await LoadAsync(Id);
                }
            }

            afterSaveAction();
        }
    }
}
