using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using FriendOrganizer.DataAccess;
using FriendOrganizer.Model.SemiModel;
using FriendOrganizer.UI.Data.Lookups.Shared;

namespace FriendOrganizer.UI.Data.Lookups.Core
{
    public class LookupDataService : IFriendLookupDataService, IProgrammingLanguageLookupDataService
        ,IMeetingLookupDataService
    {
        private readonly Func<FriendOrganizerDbContext> _contextCreator;

        public LookupDataService(Func<FriendOrganizerDbContext> contextCreator)
        {
            _contextCreator = contextCreator;
        }

        public async Task<IEnumerable<LookupItem>> GetFriendLookupAsync()
        {
            // Warn : We can't use string interp.. int Linq query
            // so we can't do : DisplayMember = $"{f.FirstName} {f.LastName}"
            using (var ctx = _contextCreator())
            {
                return await ctx.Friends.AsNoTracking()
                    .Select(f =>
                        new LookupItem()
                        {
                            Id = f.Id,
                            DisplayMember = f.FirstName + " " + f.LastName
                        }).ToListAsync();
            }
        }

        public async Task<IEnumerable<LookupItem>> GetProgrammingLanguageLookupAsync()
        {
            using (var ctx = _contextCreator())
            {
                return await ctx.ProgrammingLanguages.AsNoTracking()
                    .Select(f =>
                        new LookupItem()
                        {
                            Id = f.Id,
                            DisplayMember = f.Name
                        }).ToListAsync();
            }
        }

        public async Task<List<LookupItem>> GetMeetingLookupSync()
        {
            using (var ctx = _contextCreator())
            {
                return await ctx.Meetings.AsNoTracking()
                    .Select(m =>
                        new LookupItem()
                        {
                            Id = m.Id,
                            DisplayMember = m.Title
                        }).ToListAsync();
            }
        }
    }
}
