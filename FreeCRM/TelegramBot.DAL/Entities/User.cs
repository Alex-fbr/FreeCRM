using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TelegramBot.DAL.Entities
{
    [Table("USERS")]
    public class User
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public long Id { get; set; }

        [Required]
        public DateTime Created { get; set; }

        [Required]
        public bool IsBot { get; set; }

        [Required]
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserName { get; set; }

        public string LanguageCode { get; set; }

        public bool? CanJoinGroups { get; set; }

        public bool? CanReadAllGroupMessages { get; set; }

        public bool? SupportsInlineQueries { get; set; }

        public virtual ICollection<Message> Messages { get; set; } = new HashSet<Message>();

    }
}