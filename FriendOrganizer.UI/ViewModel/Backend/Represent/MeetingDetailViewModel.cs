﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using FriendOrganizer.Model.Model;
using FriendOrganizer.UI.Data.Repositories.Shared;
using FriendOrganizer.UI.Event.Shared;
using FriendOrganizer.UI.Services.Shared;
using FriendOrganizer.UI.ViewModel.Core;
using FriendOrganizer.UI.Wrapper;
using FriendOrganizer.UI.Wrapper.Shared;
using Prism.Commands;
using Prism.Events;

namespace FriendOrganizer.UI.ViewModel.Backend.Represent
{
    public class MeetingDetailViewModel : DetailViewModelBase, IMeetingDetailViewModel
    {
        private readonly IMeetingRepository _meetingRepository;
        private MeetingWrapper _meeting;

        private Friend _selectedAvailableFriend;
        private Friend _selectedAddedFriend;
        private List<Friend> _allFriends;

        public MeetingDetailViewModel(
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService,
            IMeetingRepository meetingRepository
            ) : base(eventAggregator,messageDialogService)
        {
            _meetingRepository = meetingRepository;
            eventAggregator.GetEvent<AfterDetailSavedEvent>().Subscribe(AfterDetailSaved);
            eventAggregator.GetEvent<AfterDetailDeletedEvent>().Subscribe(AfterDetailDeleted);
            AddedFriends=new ObservableCollection<Friend>();
            AvailableFriends=new ObservableCollection<Friend>();

            AddFriendCommand=new DelegateCommand(OnAddFriendExecute,OnAddFriendCanExecute);
            RemoveFriendCommand=new DelegateCommand(OnRemoveFriendExecute,OnRemoveFriendCanExecute);

        }

        


        public ICommand AddFriendCommand { get;}
        public ICommand RemoveFriendCommand { get;}

        public ObservableCollection<Friend> AddedFriends { get;}
        public ObservableCollection<Friend> AvailableFriends { get;}

        public Friend SelectedAvailableFriend
        {
            get => _selectedAvailableFriend;
            set
            {
                _selectedAvailableFriend = value;
                OnPropertyChanged();
                ((DelegateCommand)AddFriendCommand).RaiseCanExecuteChanged();
            }
        }

        public Friend SelectedAddedFriend
        {
            get => _selectedAddedFriend;
            set
            {
                _selectedAddedFriend = value;
                OnPropertyChanged();
                ((DelegateCommand)RemoveFriendCommand).RaiseCanExecuteChanged();
            }
        }
        public MeetingWrapper Meeting
        {
            get => _meeting;
            set
            {
                _meeting = value;
                OnPropertyChanged();
            }
        }
        protected override async void OnDeleteExecute()
        {
            var result =
                await MessageDialogService.ShowOkCancelDialogAsync($"Do you realy want to delete the meeting {Meeting.Title}","Delete Meeting ");
            if (result==MessageDialogResult.Ok)
            {
                _meetingRepository.Remove(Meeting.Model);
                await _meetingRepository.SaveAsync();
                RaisDetailDeletedEvent(Meeting.Id);
            }
        }

        protected override bool OnSaveCanExecute()
        {
            return Meeting != null && !Meeting.HasErrors && HasChanges;
        }

        protected override async void OnSaveExecute()
        {
            await _meetingRepository.SaveAsync();
            HasChanges = _meetingRepository.HasChanges();
            Id = Meeting.Id;
            RaisDetailSavedEvent(Meeting.Id,Meeting.Title);
        }

        public override async Task LoadAsync(int meetingId)
        {
            var meeting = meetingId>0
                ? await _meetingRepository.GetByIdAsync(meetingId)
                : CreateNewMeeting();

            // Id Used with tabs
            Id = meetingId;

            InitializeMeeting(meeting);

            _allFriends=await _meetingRepository.GetAllFriendsAsync();
            SetupPicklist();
        }

        private void SetupPicklist()
        {

            //            var meetingFriendIds = Meeting.Model.Friends.Select(f => f.Id).ToList();
            //            var addedFriends = _allFriends.Where(f => meetingFriendIds.Contains(f.Id)).OrderBy(f => f.FirstName);
            //            var availableFriends = _allFriends.Except(addedFriends).OrderBy(f=>f.FirstName);

            var meetingFriends = Meeting.Model.Friends.OrderBy(f=>f.FirstName).ToList();
            var availableFriends = _allFriends.Except(meetingFriends).OrderBy(f=>f.FirstName);


            AddedFriends.Clear();
            AvailableFriends.Clear();

            foreach (var addedFriend in meetingFriends)
            {
                AddedFriends.Add(addedFriend);
            }

            foreach (var avaliableFriend in availableFriends)
            {
                AvailableFriends.Add(avaliableFriend);
            }
        }

        private void InitializeMeeting(Meeting meeting)
        {
            Meeting=new MeetingWrapper(meeting);
            Meeting.PropertyChanged += (s, e) =>
            {
                if (!HasChanges)
                {
                    HasChanges = _meetingRepository.HasChanges();
                }

                if (e.PropertyName==nameof(Meeting.HasErrors))
                {
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }

                if (e.PropertyName==nameof(Meeting.Title))
                {
                    SetTitle();
                }

            };
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();

            // little trick to trigger validation
            if (Meeting.Id==0)
            {
                Meeting.Title = "";
            }
            SetTitle();
        }

        private void SetTitle()
        {
            Title = Meeting.Title;
        }

        private Meeting CreateNewMeeting()
        {
            var meeting=new Meeting()
            {
                DateFrom = DateTime.Now,
                DateTo = DateTime.Now,
            };
            _meetingRepository.Add(meeting);
            return meeting;
        }

        private bool OnRemoveFriendCanExecute()
        {
            return SelectedAddedFriend != null;
        }

        private void OnRemoveFriendExecute()
        {
            var friendToRemove = SelectedAddedFriend;

            Meeting.Model.Friends.Remove(friendToRemove);
            AvailableFriends.Add(friendToRemove);
            AddedFriends.Remove(friendToRemove);
            HasChanges = _meetingRepository.HasChanges();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        private bool OnAddFriendCanExecute()
        {
            return SelectedAvailableFriend != null;
        }

        private void OnAddFriendExecute()
        {
            var friendToAdd = SelectedAvailableFriend;

            Meeting.Model.Friends.Add(friendToAdd);
            AddedFriends.Add(friendToAdd);
            AvailableFriends.Remove(friendToAdd);
            HasChanges = _meetingRepository.HasChanges();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        private async void AfterDetailSaved(AfterDetailSavedEventArgs args)
        {
            if (args.ViewModelName==nameof(FriendDetailViewModel))
            {
                await _meetingRepository.ReloadFriendAsync(args.Id);
                _allFriends = await _meetingRepository.GetAllFriendsAsync();
                SetupPicklist();
            }
        }

        private async void AfterDetailDeleted(AfterDetailDeletedEventArgs args)
        {
            if (args.ViewModelName == nameof(FriendDetailViewModel))
            {
                _allFriends = await _meetingRepository.GetAllFriendsAsync();
                SetupPicklist();
            }
        }

    }
}
