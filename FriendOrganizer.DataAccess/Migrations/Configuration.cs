using System;
using System.Collections.Generic;
using FriendOrganizer.Model;
using FriendOrganizer.Model.Model;

namespace FriendOrganizer.DataAccess.Migrations
{
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
                f => f.FirstName,
                new Friend() { FirstName = "Osama", LastName = "AlBanna", Email = "Al_Banna_Tehno@yahoo.com", Id = 1 },
                new Friend() { FirstName = "Nour", LastName = "Sad", Email = "Nour@yahoo.com", Id = 2 },
                new Friend() { FirstName = "Omar", LastName = "Fekry", Email = "OmarF@yahoo.com", Id = 3 },
                new Friend() { FirstName = "Rihcodo", LastName = "Rich", Email = "RihRich@yahoo.com", Id = 4 }
               );

            context.ProgrammingLanguages.AddOrUpdate(p => p.Name,
                new ProgrammingLanguage() { Name = "C" },
                new ProgrammingLanguage() { Name = "C++" },
                new ProgrammingLanguage() { Name = "C#" },
                new ProgrammingLanguage() { Name = "Python" },
                new ProgrammingLanguage() { Name = "Java" }
                );

            context.SaveChanges();

            context.FriendPhoneNumbers.AddOrUpdate(pn => pn.Number,
                new FriendPhoneNumber() { Number = "+20 198470235", FriendId = context.Friends.First().Id }
                );

            context.Meetings.AddOrUpdate(m=>m.Title,new Meeting()
            {
                Title = "Watching tv",
                DateFrom = new DateTime(2019,5,17),
                DateTo = new DateTime(2020,3,18),
                Friends = new List<Friend>()
                {
                    context.Friends.Single(f=>f.FirstName=="Osama"),
                    context.Friends.Single(f=>f.FirstName=="Rihcodo")
                }
            });
        }
    }
}
