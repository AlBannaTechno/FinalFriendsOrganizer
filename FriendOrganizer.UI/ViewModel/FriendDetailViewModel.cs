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
        private readonly IFriendRepository _friendRepository;
        private FriendWrapper _friend;
        private readonly IEventAggregator _eventAggregator;
        private bool _hashChanges;

        public FriendDetailViewModel(IFriendRepository friendRrepository,IEventAggregator eventAggregator)
        {
            _friendRepository = friendRrepository;
            _eventAggregator = eventAggregator;
         
            SaveCommand=new DelegateCommand(OnSaveExecute,OnSaveCanExecute);
            DeleteCommand=new DelegateCommand(OnDeleteExecute);
        }

        private async void OnDeleteExecute()
        {
            _friendRepository.Remove(Friend.Model);
            await _friendRepository.SaveAsync();
            _eventAggregator.GetEvent<AfterFriendDeletedEvent>().Publish(Friend.Id);
        }

        private async void OnSaveExecute()
        {
            await _friendRepository.SaveAsync();
            HasChanges = _friendRepository.HasChanges();
            _eventAggregator.GetEvent<AfterFriendSavedEvent>()
                .Publish(new AfterFriendSavedEventArgs()
                {
                    Id = Friend.Id,
                    DisplayMember = Friend.FirstName +" "+Friend.LastName
                });
        }

        private bool OnSaveCanExecute()
        {
            return Friend!=null && !Friend.HasErrors && HasChanges;
        }
  
        public async Task LoadAsync(int? friendId)
        {
            var friend =friendId.HasValue
                ? await _friendRepository.GetByIdAsync(friendId.Value)
                    :CreateNewFriend()
                ;
            Friend=new FriendWrapper(friend);

            Friend.PropertyChanged += (s, e) =>
            {
                if (!HasChanges)
                {
                    HasChanges = _friendRepository.HasChanges();
                }
                // we will just Rais CanExecuteChanged When the message comes from HasErrors property
                if (e.PropertyName == nameof(Friend.HasErrors))
                {
                    ((DelegateCommand) SaveCommand).RaiseCanExecuteChanged();
                }
            };

            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            if (Friend.Id==0)
            {
                // Little trick to trigger the validation when creating new friend
                Friend.FirstName = "";
            }
        }

        private Friend CreateNewFriend()
        {
            var friend=new Friend();
            _friendRepository.Add(friend);
            return friend;
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

        public ICommand DeleteCommand { get;}

        public bool HasChanges
        {
            get => _hashChanges;
            set
            {
                if (HasChanges!=value)
                {
                    _hashChanges = value;
                    OnPropertyChanged();
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
            }
        }
    }
}
