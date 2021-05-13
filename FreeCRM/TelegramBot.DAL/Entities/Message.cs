using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TelegramBot.DAL.Entities
{
    [Table("MESSAGES")]
    public class Message
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public long Id { get; set; }

        public string Caption { get; set; }

        public string NewChatTitle { get; set; }

        public bool? DeleteChatPhoto { get; set; }

        public bool? GroupChatCreated { get; set; }

        public bool? SupergroupChatCreated { get; set; }

        public bool? ChannelChatCreated { get; set; }

        public long? MigrateToChatId { get; set; }

        public long? MigrateFromChatId { get; set; }

        public string ConnectedWebsite { get; set; }

        public int FromUserId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public int ForwardFromMessageId { get; set; }

        public string ForwardSignature { get; set; }

        public string ForwardSenderName { get; set; }

        public DateTime? ForwardDate { get; set; }

        public DateTime? EditDate { get; set; }

        public string MediaGroupId { get; set; }

        public string AuthorSignature { get; set; }

        public string Text { get; set; }

        public int Type { get; }

        [Required]
        [ForeignKey("User")]
        public long UserId { get; set; }
        public User User { get; set; }

        [Required]
        [ForeignKey("Chat")]
        public long ChatId { get; set; }
        public Chat Chat { get; set; }
    }
}
