using Microsoft.EntityFrameworkCore;

namespace TelegramBot.DAL.Contexts
{
    public class TelegramMsSqlDbContext : TelegramBaseDbContext
    {
        public TelegramMsSqlDbContext(DbContextOptions<TelegramMsSqlDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
