using Microsoft.EntityFrameworkCore;

namespace TelegramBot.DAL.Contexts
{
    public class TelegramPostgresDbContext : TelegramBaseDbContext
    {
        public TelegramPostgresDbContext(DbContextOptions<TelegramPostgresDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema("public");
        }
    }
}
