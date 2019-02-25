using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FriendOrganizer.UI.Services;
using Prism.Events;

namespace FriendOrganizer.UI.ViewModel
{
    public class ProgrammingLanguagedDetailViewModel:DetailViewModelBase
    {
        public ProgrammingLanguagedDetailViewModel(IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService) 
            : base(eventAggregator, messageDialogService)
        {
            Title = "Programming Languages";
        }

        protected override void OnDeleteExecute()
        {
            throw new NotImplementedException();
        }

        protected override bool OnSaveCanExecute()
        {
            throw new NotImplementedException();
        }

        protected override void OnSaveExecute()
        {
            throw new NotImplementedException();
        }

        public override Task LoadAsync(int id)
        {
            // TODO : load data here
            Id = id;
            return Task.Delay(0);
        }
    }
}
