using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FriendOrganizer.Model
{
    public class Meeting
    {
        public Meeting()
        {
            Friends=new List<Friend>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }

        public ICollection<Friend> Friends { get; set; } 
    }
}
