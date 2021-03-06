﻿using System;
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

            EventAggregator.GetEvent<AfterDetailClosedEvent>().Publish(new AfterDetailClosedEventArgs()
            {
                Id = this.Id,
                ViewModelName = this.GetType().Name
            });
        }

        protected  async Task SaveWithOptimisticConcurrencyAsync(Func<Task> saveFunc,Action afterSaveAction)
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
