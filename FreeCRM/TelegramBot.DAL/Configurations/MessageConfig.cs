using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using TelegramBot.DAL.Entities;

namespace TelegramBot.DAL.Configurations
{
    internal class MessageConfig : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.HasIndex(c => c.Id);

            // Один Chat ко многим Messages
            builder.HasOne(o => o.Chat)
                   .WithMany(c => c.Messages)
                   .HasForeignKey(o => o.ChatId);

            // Один User ко многим Messages
            builder.HasOne(o => o.User)
                   .WithMany(c => c.Messages)
                   .HasForeignKey(o => o.UserId);
        }
    }
}
