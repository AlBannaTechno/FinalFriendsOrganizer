using Autofac;
using FriendOrganizer.DataAccess;
using FriendOrganizer.UI.Data.Lookups;
using FriendOrganizer.UI.Data.Repositories;
using FriendOrganizer.UI.Services;
using FriendOrganizer.UI.ViewModel;
using Prism.Events;

namespace FriendOrganizer.UI.Startup
{
    public class BootStrapper
    {
        public IContainer Bootstrap()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<EventAggregator>().As<IEventAggregator>().SingleInstance();
            builder.RegisterType<MainWindow>().AsSelf();
            builder.RegisterType<MainViewModel>().AsSelf();

            // we use AsImplementedInterfaces() because we will create other []LookupInterfaces
            // And implement it inside LookupDataService class
            // so we just say here to autofac to always return LookupDataService instance
            // whenever we request any interface this class implement it
            builder.RegisterType<LookupDataService>().AsImplementedInterfaces();
            builder.RegisterType<FriendRepository>().As<IFriendRepository>();
            builder.RegisterType<NavigationViewModel>().As<INavigationViewModel>();
            builder.RegisterType<FriendDetailViewModel>().As<IFriendDetailViewModel>();
            builder.RegisterType<FriendOrganizerDbContext>().AsSelf();

            builder.RegisterType<MessageDialogService>().As<IMessageDialogService>();
            return builder.Build();
        }
    }
}
