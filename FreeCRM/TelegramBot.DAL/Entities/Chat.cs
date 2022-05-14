using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TelegramBot.DAL.Entities
{
    public class Chat
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public long Id { get; set; }

        [Required]
        public DateTime Created { get; set; }

        [Required]
        public int Type { get; set; }

        public string Title { get; set; }

        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Description { get; set; }

        public string InviteLink { get; set; }

        public int? SlowModeDelay { get; set; }

        public string StickerSetName { get; set; }

        public bool? CanSetStickerSet { get; set; }

        public virtual ICollection<Message> Messages { get; set; } = new HashSet<Message>();
    }
}
