using System;
using System.Data.Entity;
using System.Threading.Tasks;
using FriendOrganizer.DataAccess;
using FriendOrganizer.Model;

namespace FriendOrganizer.UI.Data.Repositories
{
    class FriendRepository : IFriendRepository
    {
        /* Important Explanation
         *
         * we can use : Next : 
            private readonly FriendOrganizerDbContext _contextCreator;
             
             public FriendRepository(FriendOrganizerDbContext contextCreator)
                {
                    _contextCreator = contextCreator;
                }
            public IEnumerable<Friend> GetAll()
            {
                using (var ctx = _contextCreator)
                {
                    return ctx.Friends.AsNoTracking().ToList();
                }
            }

            But If we need to Support Create instance of FriendOrganizerDbContext inside Autofac
            We Must use the current => [use it inside a function to force autofac to create new instance every time]
                -------
                Benefit of doing that :
                    we can use that to allow autofac to use just one instance with every time we call GetAll()
                    this mean all thoses data will be in the same context
                    [This will be by changing BootStrapper.cs]
                        builder.RegisterType<FriendOrganizerDbContext>().AsSelf().SingleInstance();
                    But now we just create new instance every once we call GetAll() method
                    and this is impossible if we pass FriendOrganizerDbContext to the constructor 
                    Because autofac will just create the instance once
                Finally we can say
                Using FriendOrganizerDbContext directly
                    =
                        Func<FriendOrganizerDbContext> : this mean function return FriendOrganizerDbContext
                        +
                        builder.RegisterType<FriendOrganizerDbContext>().AsSelf().SingleInstance();
                        
         */

        private readonly FriendOrganizerDbContext _context;

        public FriendRepository(FriendOrganizerDbContext context)
        {
            _context = context;
        }

        public async Task<Friend> GetByIdAsync(int friendId)
        {
                return await _context.Friends.SingleAsync(f => f.Id == friendId);
        }

        public async Task SaveAsync()
        {
                await _context.SaveChangesAsync();
        }

        public bool HasChanges()
        {
            return _context.ChangeTracker.HasChanges();
        }

        public void Add(Friend friend)
        {
            _context.Friends.Add(friend);
        }

        public void Remove(Friend friendModel)
        {
            _context.Friends.Remove(friendModel);
        }
    }
}