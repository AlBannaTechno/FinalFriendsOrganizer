using FriendOrganizer.UI.Event;
using Prism.Commands;
using Prism.Events;
using System.Threading.Tasks;
using System.Windows.Input;
using FriendOrganizer.UI.Services;

namespace FriendOrganizer.UI.ViewModel
{
    public abstract class DetailViewModelBase : ViewModelBase, IDetailViewModel
    {
        private bool _hasChanges;

        protected readonly IEventAggregator EventAggregator;
        protected readonly IMessageDialogService MessageDialogService;
        private  int _id;
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

        // means sub class must implement them
        protected abstract void OnDeleteExecute();

        protected abstract bool OnSaveCanExecute();

        protected abstract void OnSaveExecute();

        public abstract Task LoadAsync(int id);

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


        protected virtual void RaisCollectionSavedEvent()
        {
            EventAggregator.GetEvent<AfterCollectionSavedEvent>().Publish(new AfterCollectionSavedEventArgs()
            {
                ViewModelName = this.GetType().Name
            });
        }

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

        protected virtual void OnCloseDetailViewExecute()
        {
            if (HasChanges)
            {
                var result =
                    MessageDialogService.ShowOkCancelDialog("You've made changes . Close this item ?", "Question");
                if (result==MessageDialogResult.Cancel)
                {
                    return;
                }
            }

            EventAggregator.GetEvent<AfterDetailClosedEvent>().Publish(new AfterDetailClosedEventArgs()
            {
                Id = this.Id,
                ViewModelName = this.GetType().Name
            });
        }
    }
}
