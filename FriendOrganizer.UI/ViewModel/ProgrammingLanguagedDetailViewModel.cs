using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FriendOrganizer.UI.Data.Repositories;
using FriendOrganizer.UI.Services;
using FriendOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;

namespace FriendOrganizer.UI.ViewModel
{
    public class ProgrammingLanguagedDetailViewModel:DetailViewModelBase, IProgrammingLanguagedDetailViewModel
    {
        private readonly IProgrammingLanguageRepository _programmingLanguageRepository;

        public ProgrammingLanguagedDetailViewModel(IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService
            ,IProgrammingLanguageRepository programmingLanguageRepository) 
            : base(eventAggregator, messageDialogService)
        {
            _programmingLanguageRepository = programmingLanguageRepository;
            Title = "Programming Languages";
            ProgrammingLanguages=new ObservableCollection<ProgrammingLanguageWrapper>();
        }

        public ObservableCollection<ProgrammingLanguageWrapper> ProgrammingLanguages { get; }

        protected override void OnDeleteExecute()
        {
            throw new NotImplementedException();
        }

        protected override bool OnSaveCanExecute()
        {
            return HasChanges && ProgrammingLanguages.All(p => !p.HasErrors);
        }

        protected override async void OnSaveExecute()
        {
            await _programmingLanguageRepository.SaveAsync();
            HasChanges = _programmingLanguageRepository.HasChanges();
            RaisCollectionSavedEvent();
        }

        public override async Task LoadAsync(int id)
        {
            // TODO : load data here
            Id = id;
            foreach (var programmingLanguageWrapper in ProgrammingLanguages)
            {
                programmingLanguageWrapper.PropertyChanged -= Wrapper_PropertyChanged;
            }
            ProgrammingLanguages.Clear();
            var languages = await _programmingLanguageRepository.GetAllAsync();
            foreach (var programmingLanguage in languages)
            {
                var wrapper=new ProgrammingLanguageWrapper(programmingLanguage);
                wrapper.PropertyChanged += Wrapper_PropertyChanged;
                ProgrammingLanguages.Add(wrapper);
            }
        }

        private void Wrapper_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!HasChanges)
            {
                HasChanges = _programmingLanguageRepository.HasChanges();
            }

            if (e.PropertyName==nameof(ProgrammingLanguageWrapper.HasErrors))
            {
                ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }
    }
}
