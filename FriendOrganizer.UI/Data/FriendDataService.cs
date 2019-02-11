using System.Collections.Generic;
using FriendOrganizer.Model;

namespace FriendOrganizer.UI.Data
{
    class FriendDataService : IFriendDataService
    {
        public IEnumerable<Friend> GetAll()
        {
            // TODO : Load Data From Real Database
            yield return new Friend(){FirstName = "Osama",LastName = "AlBanna",Email ="Al_Banna_Tehno@yahoo.com",Id = 1};
            yield return new Friend(){FirstName = "Nour",LastName = "Sad",Email ="Nour@yahoo.com",Id = 2};
            yield return new Friend(){FirstName = "Omar",LastName = "Fekry",Email ="OmarF@yahoo.com",Id = 3};
            yield return new Friend(){FirstName = "Rihcodo",LastName = "Rich",Email ="RihRich@yahoo.com",Id = 4};
        }
    }
}
