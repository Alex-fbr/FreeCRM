using Microsoft.EntityFrameworkCore;
using TelegramBot.DAL.Entities;

namespace TelegramBot.DAL.Contexts
{
    public interface ITelegramBaseDbContext
    {
        DbSet<User> Users { get; set; }
        DbSet<Contact> Contacts { get; set; }
        DbSet<Message> Messages { get; set; }
        DbSet<Chat> Chats { get; set; }
        DbSet<ChatPermission> ChatPermissions { get; set; }
    }
}
