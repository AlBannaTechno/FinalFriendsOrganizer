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
    /**
     * This class Created To be a front interface for all Lookup interfaces to support navigation
     * So for registeration purpose in autofac we implement all Lookups interfaces in this class
     * where every interface related to lookup for specific model
     *  => so in BootStrapper.cs we register it as : {builder.RegisterType<LookupDataService>().AsImplementedInterfaces();}
     *      this mean whenever we request on of the interfaces which this class implemented
     *      autofac will return this class
     *
     * Note that all implemented methods should return {Task<IEnumerable<LookupItem>>} and must be an async
     *  asyn , Task : to support async
     *  IEnumerable : to improve performance
     *  LookupItem  : because this type which all methods in this class should return
     *
     * Also Note : we specifiy all fetched entities AsNoTracking() because Lookup must use to only view a summary of contents
     * not for changing it
     */
    public class LookupDataService : IFriendLookupDataService, IProgrammingLanguageLookupDataService
        ,IMeetingLookupDataService
    {
        private readonly Func<FriendOrganizerDbContext> _contextCreator;

        public LookupDataService(Func<FriendOrganizerDbContext> contextCreator)
        {
            _contextCreator = contextCreator;
        }

        /*
         * Implemented from IFriendLookupDataService
         */
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

        /**
         * Implemented from IProgrammingLanguageLookupDataService
         */
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

        /**
         * Implemented from IMeetingLookupDataService
         */
        public async Task<IEnumerable<LookupItem>> GetMeetingLookupSync()
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
