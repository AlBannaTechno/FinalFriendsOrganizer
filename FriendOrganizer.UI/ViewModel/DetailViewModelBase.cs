using FriendOrganizer.UI.Event;
using Prism.Commands;
using Prism.Events;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FriendOrganizer.UI.ViewModel
{
    public abstract class DetailViewModelBase : ViewModelBase, IDetailViewModel
    {
        private bool _hasChanges;

        protected readonly IEventAggregator EventAggregator;
        private  int _id;
        private string _title;

        protected DetailViewModelBase(IEventAggregator eventAggregator)
        {
            EventAggregator = eventAggregator;
            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
            DeleteCommand = new DelegateCommand(OnDeleteExecute);
            CloseDetailViewCommand=new DelegateCommand(OnCloseDetailViewExecute);
        }

        

        public ICommand CloseDetailViewCommand { get; }

        // means sub class must implement them
        protected abstract void OnDeleteExecute();

        protected abstract bool OnSaveCanExecute();

        protected abstract void OnSaveExecute();

        public abstract Task LoadAsync(int? id);

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

        }
    }
}
