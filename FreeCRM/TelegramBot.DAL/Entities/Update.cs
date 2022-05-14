using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TelegramBot.DAL.Entities
{
    public class Update
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public long Id { get; set; }

        [Required]
        public int Type { get; set; }

        [Required]
        [ForeignKey("Message")]
        public long MessageId { get; set; }
        public Message Message { get; set; }
    }
}
