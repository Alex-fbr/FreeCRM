using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TelegramBot.DAL.Entities
{
    [Table("CHATPERMISSIONS")]
    public class ChatPermission
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long Id { get; set; }

        public bool? CanSendMessages { get; set; }

        public bool? CanSendMediaMessages { get; set; }

        public bool? CanSendPolls { get; set; }

        public bool? CanSendOtherMessages { get; set; }

        public bool? CanAddWebPagePreviews { get; set; }

        public bool? CanChangeInfo { get; set; }

        public bool? CanInviteUsers { get; set; }

        public bool? CanPinMessages { get; set; }


        [Required]
        [ForeignKey("Chat")]
        public long ChatId { get; set; }
        public Chat Chat { get; set; }
    }
}
