﻿using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using FriendOrganizer.DataAccess;
using FriendOrganizer.Model.Model;
using FriendOrganizer.UI.Data.Repositories.Core;

namespace FriendOrganizer.UI.Data.Repositories.Shared
{
    class FriendRepository : GenericRepository<Friend, FriendOrganizerDbContext>, IFriendRepository
    {
        public FriendRepository(FriendOrganizerDbContext context)
            : base(context)
        {
        }

        public override async Task<Friend> GetByIdAsync(int friendId)
        {
            return await Context.Friends.Include(f => f.PhoneNumbers).SingleAsync(f => f.Id == friendId);
        }

        public void RemovePhoneNumber(FriendPhoneNumber model)
        {
            Context.FriendPhoneNumbers.Remove(model);
        }

        public async Task<bool> HasMeetingsAsync(int friendId)
        {
            return await Context.Meetings.AsNoTracking()
                .Include(m => m.Friends)
                .AnyAsync(m => m.Friends.Any(f => f.Id == friendId));
        }
    }
}
