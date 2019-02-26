using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FriendOrganizer.Model.Model
{
    public class Friend
    {
        public Friend()
        {
            /**
             * Initialize any Collection to prevent any run time exception
             */
            PhoneNumbers = new Collection<FriendPhoneNumber>();
            Meetings=new Collection<Meeting>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string LastName { get; set; }

        [StringLength(50)]
        [EmailAddress]
        public string Email { get; set; }

        [ForeignKey("FavoriteLanguage")]
        public int? FavoriteLanguageId { get; set; }

        /**
         * This [Timestamp] attribute wiil make entity track changes from database before submit any changes
         * This created to support multi-user scenario
         * This must used with a validation on DbUpdateConcurrencyException
         *
         * We impelement this in : FriendOrganizer.UI.DetailViewModelBase.SaveWithOptimisticConcurrencyAsync()
         *
         * we have two type
         *  1- Optimistic Concurrency : Client Win  :   so client can ovveride database changes
         *  2- Pesimistic Concurrency : Db Win      :   so client will retrieve the value from db and update it's[Client] entity
         *  SaveWithOptimisticConcurrencyAsync() : Support two scenarios
         */
        [Timestamp]
        public byte[] RowVersion { get; set; }

        public ProgrammingLanguage FavoriteLanguage { get; set; }

        public ICollection<FriendPhoneNumber> PhoneNumbers { get; set; }

        public ICollection<Meeting> Meetings { get; set; }
    }
}
