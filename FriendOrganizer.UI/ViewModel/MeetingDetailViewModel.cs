﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FriendOrganizer.Model;
using FriendOrganizer.UI.Data.Repositories;
using FriendOrganizer.UI.Services;
using FriendOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;

namespace FriendOrganizer.UI.ViewModel
{
    public class MeetingDetailViewModel : DetailViewModelBase, IMeetingDetailViewModel
    {
        private readonly IMessageDialogService _messageDialogService;
        private readonly IMeetingRepository _meetingRepository;
        private MeetingWrapper _meeting;

        public MeetingDetailViewModel(
            IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService,
            IMeetingRepository meetingRepository
            ) : base(eventAggregator)
        {
            _messageDialogService = messageDialogService;
            _meetingRepository = meetingRepository;
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
        protected override void OnDeleteExecute()
        {
            var result =
                _messageDialogService.ShowOkCancelDialog($"Do you realy want to delete the meeting {Meeting.Title}","Delete Meeting ");
            if (result==MessageDialogResult.Ok)
            {
                _meetingRepository.Remove(Meeting.Model);
                _meetingRepository.SaveAsync();
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
            RaisDetailSavedEvent(Meeting.Id,Meeting.Title);
        }

        public override async Task LoadAsync(int? meetingId)
        {
            var meeting = meetingId.HasValue
                ? await _meetingRepository.GetByIdAsync(meetingId.Value)
                : CreateNewMeeting();
            InitializeMeeting(meeting);
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
            };
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
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
    }
}
