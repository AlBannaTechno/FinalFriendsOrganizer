using FriendOrganizer.Model;

namespace FriendOrganizer.DataAccess.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<FriendOrganizer.DataAccess.FriendOrganizerDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(FriendOrganizer.DataAccess.FriendOrganizerDbContext context)
        {
            context.Friends.AddOrUpdate(
                f=>f.FirstName,
                new Friend() { FirstName = "Osama", LastName = "AlBanna", Email = "Al_Banna_Tehno@yahoo.com", Id = 1 },
                new Friend() { FirstName = "Nour", LastName = "Sad", Email = "Nour@yahoo.com", Id = 2 },
                new Friend() { FirstName = "Omar", LastName = "Fekry", Email = "OmarF@yahoo.com", Id = 3 },
                new Friend() { FirstName = "Rihcodo", LastName = "Rich", Email = "RihRich@yahoo.com", Id = 4 }
               );

            context.ProgrammingLanguages.AddOrUpdate(p=>p.Name,
                new ProgrammingLanguage() { Name = "C"},
                new ProgrammingLanguage() { Name = "C++"},
                new ProgrammingLanguage() { Name = "C#"},
                new ProgrammingLanguage() { Name = "Python"},
                new ProgrammingLanguage() { Name = "Java"}
                );
        }
    }
}
