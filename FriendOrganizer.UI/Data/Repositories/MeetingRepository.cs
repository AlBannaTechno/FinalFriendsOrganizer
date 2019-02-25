﻿using System.Threading.Tasks;
using FriendOrganizer.DataAccess;
using FriendOrganizer.Model;
using System.Data.Entity;
namespace FriendOrganizer.UI.Data.Repositories
{
    public class MeetingRepository:GenericRepository<Meeting,FriendOrganizerDbContext>, IMeetingRepository
    {
        protected MeetingRepository(FriendOrganizerDbContext context) : base(context)
        {
        }

        public override async Task<Meeting> GetByIdAsync(int id)
        {
            return await Context.Meetings.Include(m => m.Friends).SingleAsync(m=>m.Id==id);
        }
    }
}
