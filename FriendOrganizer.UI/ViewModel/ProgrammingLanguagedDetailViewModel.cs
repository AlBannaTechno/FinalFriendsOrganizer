using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FriendOrganizer.Model;
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
        private ProgrammingLanguageWrapper _selectedProgrammingLanguage;

        public ProgrammingLanguagedDetailViewModel(IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService
            ,IProgrammingLanguageRepository programmingLanguageRepository) 
            : base(eventAggregator, messageDialogService)
        {
            _programmingLanguageRepository = programmingLanguageRepository;
            Title = "Programming Languages";
            ProgrammingLanguages=new ObservableCollection<ProgrammingLanguageWrapper>();

            AddCommand=new DelegateCommand(OnAddExecute);
            RemoveCommand=new DelegateCommand(OnRemoveExecute,OnRemoveCanExecute);
        }

       
        public ObservableCollection<ProgrammingLanguageWrapper> ProgrammingLanguages { get; }

        public ProgrammingLanguageWrapper SelectedProgrammingLanguage
        {
            get => _selectedProgrammingLanguage;
            set
            {
                _selectedProgrammingLanguage = value;
                OnPropertyChanged();
                ((DelegateCommand)RemoveCommand).RaiseCanExecuteChanged();
            }
        }

        public ICommand AddCommand { get; }
        public ICommand RemoveCommand { get;  }

        // this delete this vieWmodel : no need to it at all
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

        private bool OnRemoveCanExecute()
        {
            return SelectedProgrammingLanguage != null;
        }

        private async void OnRemoveExecute()
        {
            var isReferenced =
                await _programmingLanguageRepository.IsReferencedByFriendAsync(SelectedProgrammingLanguage.Id);
            if (isReferenced)
            {
                MessageDialogService.ShowInfoDialog($"You can't remove this language : {SelectedProgrammingLanguage.Name} it's referenced by at least one friend");
                return;
            }

            SelectedProgrammingLanguage.PropertyChanged -= Wrapper_PropertyChanged;
            _programmingLanguageRepository.Remove(SelectedProgrammingLanguage.Model);
            ProgrammingLanguages.Remove(SelectedProgrammingLanguage);
            SelectedProgrammingLanguage = null;
            HasChanges = _programmingLanguageRepository.HasChanges();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        private void OnAddExecute()
        {
            var wrapper=new ProgrammingLanguageWrapper(new ProgrammingLanguage());
            wrapper.PropertyChanged += Wrapper_PropertyChanged;
            _programmingLanguageRepository.Add(wrapper.Model);
            ProgrammingLanguages.Add(wrapper);
            wrapper.Name = "";
        }

    }
}
