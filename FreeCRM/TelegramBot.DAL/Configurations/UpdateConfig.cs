using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using TelegramBot.DAL.Entities;

namespace TelegramBot.DAL.Configurations
{
    internal class UpdateConfig : IEntityTypeConfiguration<Update>
    {
        public void Configure(EntityTypeBuilder<Update> builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.HasIndex(c => c.Id);

            // Один Chat ко многим Messages
            builder.HasOne(o => o.Message)
                   .WithOne(c => c.Update)
                   .HasForeignKey<Update>(b => b.MessageId);
        }
    }
}
