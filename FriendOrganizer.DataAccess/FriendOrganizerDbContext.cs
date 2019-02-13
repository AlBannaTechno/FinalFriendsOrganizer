using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using FriendOrganizer.Model;

namespace FriendOrganizer.DataAccess
{
    public class FriendOrganizerDbContext:DbContext
    {
        // https://stackoverflow.com/questions/30084916/no-connection-string-named-could-be-found-in-the-application-config-file/37697318
        // using :base(name=FriendOrganizerDb") only allowable if
        // 1- we set ConnectionString in The StartUp Project Configs
        // 2- or we mark current project as startUp Project
        // Otherwise we must use :base("FriendOrganizerDb") before execute enable-migrations
        public FriendOrganizerDbContext()
            :base("FriendOrganizerDb")
        {
            
        }
        public DbSet<Friend> Friends { get; set; }
        public DbSet<ProgrammingLanguage> ProgrammingLanguages { get; set; }
        public DbSet<FriendPhoneNumber> FriendPhoneNumbers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }

}
