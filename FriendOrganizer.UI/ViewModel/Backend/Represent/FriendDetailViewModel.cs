﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using FriendOrganizer.Model.Model;
using FriendOrganizer.Model.SemiModel;
using FriendOrganizer.UI.Data.Lookups.Shared;
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
    class FriendDetailViewModel : DetailViewModelBase, IFriendDetailViewModel
    {
        private readonly IFriendRepository _friendRepository;

        private FriendWrapper _friend;


        private readonly IProgrammingLanguageLookupDataService _programmingLanguageLookupDataService;

        public FriendDetailViewModel(IFriendRepository friendRrepository, IEventAggregator eventAggregator
            , IMessageDialogService messageDialogService,
            IProgrammingLanguageLookupDataService programmingLanguageLookupDataService)
            : base(eventAggregator,messageDialogService)
        {
            _friendRepository = friendRrepository;
            _programmingLanguageLookupDataService = programmingLanguageLookupDataService;

            eventAggregator.GetEvent<AfterCollectionSavedEvent>().Subscribe(AfterCollectionSaved);

            AddPhoneNumberCommand = new DelegateCommand(OnAddPhoneNumberExecute);
            RemovePhoneNumberCommand = new DelegateCommand(OnRemovePhoneNumberExecute, OnRemovePhoneNumberCanExecute);

            ProgrammingLanguages = new ObservableCollection<LookupItem>();
            PhoneNumbers = new ObservableCollection<FriendPhoneNumberWrapper>();
        }



        private bool OnRemovePhoneNumberCanExecute()
        {
            return SelectedPhoneNumber != null;
        }

        private async void OnRemovePhoneNumberExecute()
        {
            var result = await MessageDialogService.ShowOkCancelDialogAsync($"Are You Sure :Removing Phone Number : {SelectedPhoneNumber.Number}", "Phone Remove Warning");
            if (result == MessageDialogResult.Ok)
            {
                SelectedPhoneNumber.PropertyChanged -= FriendPhoneNumberWrapper_PropertyChanged;
                _friendRepository.RemovePhoneNumber(SelectedPhoneNumber.Model);
                //                Friend.Model.PhoneNumbers.Remove(SelectedPhoneNumber.Model); // We can't do that
                PhoneNumbers.Remove(SelectedPhoneNumber);
                SelectedPhoneNumber = null;
                HasChanges = _friendRepository.HasChanges();
                ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }

        private void OnAddPhoneNumberExecute()
        {
            var newNumber = new FriendPhoneNumberWrapper(new FriendPhoneNumber());
            newNumber.PropertyChanged += FriendPhoneNumberWrapper_PropertyChanged;
            PhoneNumbers.Add(newNumber);
            Friend.Model.PhoneNumbers.Add(newNumber.Model);
            newNumber.Number = ""; // To trigger the validation
        }

        public ObservableCollection<LookupItem> ProgrammingLanguages { get; }

        public ObservableCollection<FriendPhoneNumberWrapper> PhoneNumbers { get; }

        private FriendPhoneNumberWrapper _selectedPhoneNumber;

        public FriendPhoneNumberWrapper SelectedPhoneNumber
        {
            get => _selectedPhoneNumber;
            set
            {
                _selectedPhoneNumber = value;
                OnPropertyChanged();
                ((DelegateCommand)RemovePhoneNumberCommand).RaiseCanExecuteChanged();
            }
        }

        protected override async void OnDeleteExecute()
        {
            if (await _friendRepository.HasMeetingsAsync(Friend.Id))
            {
                await MessageDialogService.ShowInfoDialogAsync($"Can't Remove {Friend.FirstName} {Friend.LastName} : This has at least one meeting!");
                return;
            }

            var result = await MessageDialogService.ShowOkCancelDialogAsync($"Are Your Sure delete Frien : {Friend.FirstName} {Friend.LastName} ?", "Delete Warning");
            if (result == MessageDialogResult.Ok)
            {
                _friendRepository.Remove(Friend.Model);
                await _friendRepository.SaveAsync();
                RaisDetailDeletedEvent(Friend.Id);
            }

        }

        protected override async void OnSaveExecute()
        {
            await SaveWithOptimisticConcurrencyAsync(_friendRepository.SaveAsync, () =>
            {
                HasChanges = _friendRepository.HasChanges();
                Id = Friend.Id;
                RaisDetailSavedEvent(Friend.Id, Friend.FirstName + " " + Friend.LastName);
            });
        }

        protected override bool OnSaveCanExecute()
        {
            return Friend != null
                   && !Friend.HasErrors
                   && HasChanges && PhoneNumbers.All(pn => !pn.HasErrors);
        }

        public override async Task LoadAsync(int friendId)
        {
            var friend = friendId > 0
                ? await _friendRepository.GetByIdAsync(friendId)
                    : CreateNewFriend()
                ;

            // This Id is the id of DetailViewModel to work with tabs 
            Id = friendId;

            InitializeFriend(friend);
            InitializeFrienPhoneNumbers(friend.PhoneNumbers);
            await LoadProgrammingLanguagesLookupAsync();
        }

        private void InitializeFrienPhoneNumbers(ICollection<FriendPhoneNumber> friendPhoneNumbers)
        {
            foreach (var wrapper in PhoneNumbers)
            {
                wrapper.PropertyChanged -= FriendPhoneNumberWrapper_PropertyChanged;
            }
            PhoneNumbers.Clear();
            foreach (var friendPhoneNumber in friendPhoneNumbers)
            {
                var wrapper = new FriendPhoneNumberWrapper(friendPhoneNumber);
                PhoneNumbers.Add(wrapper);
                wrapper.PropertyChanged += FriendPhoneNumberWrapper_PropertyChanged;
            }
        }

        private void FriendPhoneNumberWrapper_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!HasChanges)
            {
                HasChanges = _friendRepository.HasChanges();
            }

            // if go throw next : there is an error with the phone number
            if (e.PropertyName == nameof(FriendPhoneNumberWrapper.HasErrors))
            {
                ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }

        private void InitializeFriend(Friend friend)
        {
            Friend = new FriendWrapper(friend);

            Friend.PropertyChanged += (s, e) =>
            {
                if (!HasChanges)
                {
                    HasChanges = _friendRepository.HasChanges();
                }
                // we will just Rais CanExecuteChanged When the message comes from HasErrors property
                if (e.PropertyName == nameof(Friend.HasErrors))
                {
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }

                if (
                    e.PropertyName==nameof(Friend.FirstName)
                    || e.PropertyName==nameof(Friend.LastName)
                    )
                {
                    SetTitle();
                }
            };

            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            if (Friend.Id == 0)
            {
                // Little trick to trigger the validation when creating new friend
                Friend.FirstName = "";
            }
            SetTitle();
        }

        private void SetTitle()
        {
            Title = $"{Friend.FirstName} {Friend.LastName}";
        }


        private async void AfterCollectionSaved(AfterCollectionSavedEventArgs args)
        {
            if (args.ViewModelName==nameof(ProgrammingLanguagedDetailViewModel))
            {
                await LoadProgrammingLanguagesLookupAsync();
            }
        }

        private async Task LoadProgrammingLanguagesLookupAsync()
        {
            ProgrammingLanguages.Clear();
            ProgrammingLanguages.Add(new NullLookupItem() { DisplayMember = "!Not Prefered" });
            var lookup = await _programmingLanguageLookupDataService.GetProgrammingLanguageLookupAsync();
            foreach (var lookupItem in lookup)
            {
                ProgrammingLanguages.Add(lookupItem);
            }
        }

        private Friend CreateNewFriend()
        {
            var friend = new Friend();
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

        public ICommand AddPhoneNumberCommand { get; }

        public ICommand RemovePhoneNumberCommand { get; }
    }
}
