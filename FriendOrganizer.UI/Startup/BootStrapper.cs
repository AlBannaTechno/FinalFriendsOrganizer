using Autofac;
using FriendOrganizer.DataAccess;
using FriendOrganizer.UI.Data.Lookups;
using FriendOrganizer.UI.Data.Lookups.Core;
using FriendOrganizer.UI.Data.Repositories;
using FriendOrganizer.UI.Data.Repositories.Shared;
using FriendOrganizer.UI.Services.Shared;
using FriendOrganizer.UI.ViewModel;
using FriendOrganizer.UI.ViewModel.Backend.Container;
using FriendOrganizer.UI.ViewModel.Backend.Represent;
using FriendOrganizer.UI.ViewModel.Core;
using Prism.Events;

namespace FriendOrganizer.UI.Startup
{
    /**
     * This class is the Bootstrapper for this project and use dependency injection to supply/register all classes
     *  and interfaces
     * And we use autofac library to do that
     * we must use this class to just retrieve the first initialized instance for this project
     *  then all other sub/internal instances will retrieved automaticaly
     *  => this instance is the mainWindow and located at App.xaml.cs : App_OnStartup event handler
     *  => this event handler we specifiy it inside App.xaml : Startup="App_OnStartup"
     */
    public class BootStrapper
    {
        public IContainer Bootstrap()
        {
            var builder = new ContainerBuilder();

            // we request SingleInstance to make it work across the application layers
            builder.RegisterType<EventAggregator>().As<IEventAggregator>().SingleInstance();

            builder.RegisterType<MainWindow>().AsSelf();
            builder.RegisterType<MainViewModel>().AsSelf();

            // we use AsImplementedInterfaces() because we will create other []LookupInterfaces
            // And implement it inside LookupDataService class
            // so we just say here to autofac to always return LookupDataService instance
            // whenever we request any interface this class implement it
            builder.RegisterType<LookupDataService>().AsImplementedInterfaces();
            builder.RegisterType<FriendRepository>().As<IFriendRepository>();
            builder.RegisterType<ProgrammingLanguageRepository>().As<IProgrammingLanguageRepository>();
            builder.RegisterType<MeetingRepository>().As<IMeetingRepository>();
            builder.RegisterType<NavigationViewModel>().As<INavigationViewModel>();

            // To support autofac indexer {IIndex} => IIndex<string, IDetailViewModel> _detailViewModelCreator

            builder.RegisterType<FriendDetailViewModel>().Keyed<IDetailViewModel>(nameof(FriendDetailViewModel));
            builder.RegisterType<MeetingDetailViewModel>().Keyed<IDetailViewModel>(nameof(MeetingDetailViewModel));
            builder.RegisterType<ProgrammingLanguagedDetailViewModel>().Keyed<IDetailViewModel>(nameof(ProgrammingLanguagedDetailViewModel));

            builder.RegisterType<FriendOrganizerDbContext>().AsSelf();

            builder.RegisterType<MessageDialogService>().As<IMessageDialogService>();
            return builder.Build();
        }
    }
}
