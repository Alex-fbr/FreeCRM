using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TelegramBot.DAL.Entities
{
    public class Contact
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public long Id { get; set; }

        [Required]
        public DateTime Created { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Vcard { get; set; }

        [Required]
        [ForeignKey("User")]
        public long UserId { get; set; }
        public User User { get; set; }
    }
}
