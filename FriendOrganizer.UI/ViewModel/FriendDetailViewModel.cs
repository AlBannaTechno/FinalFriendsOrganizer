using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using FriendOrganizer.Model;
using FriendOrganizer.UI.Data;
using FriendOrganizer.UI.Data.Repositories;
using FriendOrganizer.UI.Event;
using FriendOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;

namespace FriendOrganizer.UI.ViewModel
{
    class FriendDetailViewModel : ViewModelBase, IFriendDetailViewModel
    {
        private readonly IFriendRepository _repository;
        private FriendWrapper _friend;
        private readonly IEventAggregator _eventAggregator;

        public FriendDetailViewModel(IFriendRepository repository,IEventAggregator eventAggregator)
        {
            _repository = repository;
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<OpenFriendDetailViewEvent>()
                .Subscribe(OnOpenFriendDetailView);

            SaveCommand=new DelegateCommand(OnSaveExecute,OnSaveCanExecute);
        }

        private async void OnSaveExecute()
        {
            await _repository.SaveAsync();
            _eventAggregator.GetEvent<AfterFriendSavedEvent>()
                .Publish(new AfterFriendSavedEventArgs()
                {
                    Id = Friend.Id,
                    DisplayMember = Friend.FirstName +" "+Friend.LastName
                });
        }

        private bool OnSaveCanExecute()
        {
            // TODO : check if friend has changes
            return Friend!=null && !Friend.HasErrors;
        }

        private async void OnOpenFriendDetailView(int friendId)
        {
            await LoadAsync(friendId);
        }

        public async Task LoadAsync(int friendId)
        {
            var friend = await _repository.GetByIdAsync(friendId);
            Friend=new FriendWrapper(friend);

            Friend.PropertyChanged += (s, e) =>
            {
                // we will just Rais CanExecuteChanged When the message comes from HasErrors property
                if (e.PropertyName == nameof(Friend.HasErrors))
                {
                    ((DelegateCommand) SaveCommand).RaiseCanExecuteChanged();
                }
            };

            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        public FriendWrapper Friend
        {
            get => _friend;
            set
            {
                _friend = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveCommand { get;}
    }
}
