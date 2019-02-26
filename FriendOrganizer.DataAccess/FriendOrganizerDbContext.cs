using FriendOrganizer.Model;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using FriendOrganizer.Model.Model;

/**
 * In This File We Create FriendOrganizerDbContext To Contian FriendOrganizer Application entities
 */
namespace FriendOrganizer.DataAccess
{
    public class FriendOrganizerDbContext : DbContext
    {
        // https://stackoverflow.com/questions/30084916/no-connection-string-named-could-be-found-in-the-application-config-file/37697318
        // using :base(name=FriendOrganizerDb") only allowable if
        // 1- we set ConnectionString in The StartUp Project Configs
        // 2- or we mark current project as startUp Project
        // Otherwise we must use :base("FriendOrganizerDb") before execute enable-migrations

        /**
         * [FriendOrganizerDb] => this name specified in App.config as a connectionString in
         *      1- FriendOrganizer.DataAccess
         *      2- FriendOrganizer.UI
         */
        public FriendOrganizerDbContext()
            : base("FriendOrganizerDb")
        {
        }

        #region Define all entites

        public DbSet<Friend> Friends { get; set; }

        public DbSet<ProgrammingLanguage> ProgrammingLanguages { get; set; }

        public DbSet<FriendPhoneNumber> FriendPhoneNumbers { get; set; }

        public DbSet<Meeting> Meetings { get; set; }

        #endregion

        // In This function we should configure entites
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // prevent EF from PluralizingTableName => [Game => Games] -> we need it Game only
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
