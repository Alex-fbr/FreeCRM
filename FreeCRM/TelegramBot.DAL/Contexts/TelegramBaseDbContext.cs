using Microsoft.EntityFrameworkCore;
using TelegramBot.DAL.Configurations;
using TelegramBot.DAL.Entities;

namespace TelegramBot.DAL.Contexts
{
    public abstract class TelegramBaseDbContext : DbContext, ITelegramBaseDbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<ChatPermission> ChatPermissions { get; set; }

        protected TelegramBaseDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new ChatConfig());
            modelBuilder.ApplyConfiguration(new ChatPermissionConfig());
            modelBuilder.ApplyConfiguration(new UserConfig());
            modelBuilder.ApplyConfiguration(new ContactConfig());
            modelBuilder.ApplyConfiguration(new MessageConfig());
        }
    }
}
