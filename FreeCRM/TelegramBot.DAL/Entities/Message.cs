using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TelegramBot.DAL.Entities
{
    public class Message
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public long Id { get; set; }

        /// <summary>
        /// Id предыдущего сообщения
        /// </summary>
        public long? PreviousMessageId { get; set; }

        public string Caption { get; set; }

        public string NewChatTitle { get; set; }

        public bool? DeleteChatPhoto { get; set; }

        public bool? GroupChatCreated { get; set; }

        public bool? SupergroupChatCreated { get; set; }

        public bool? ChannelChatCreated { get; set; }

        public long? MigrateToChatId { get; set; }

        public long? MigrateFromChatId { get; set; }

        public string ConnectedWebsite { get; set; }

        public long FromUserId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public int? ForwardFromMessageId { get; set; }

        public string ForwardSignature { get; set; }

        public string ForwardSenderName { get; set; }

        public DateTime? ForwardDate { get; set; }

        public DateTime? EditDate { get; set; }

        public string MediaGroupId { get; set; }

        public string AuthorSignature { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public int Type { get; set; }

        [Required]
        [ForeignKey("User")]
        public long UserId { get; set; }
        public User User { get; set; }

        [Required]
        [ForeignKey("Chat")]
        public long ChatId { get; set; }
        public Chat Chat { get; set; }

        public Update Update { get; set; }
    }
}
