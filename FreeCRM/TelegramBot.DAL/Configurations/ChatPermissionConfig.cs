using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using TelegramBot.DAL.Entities;

namespace TelegramBot.DAL.Configurations
{
    internal class ChatPermissionConfig : IEntityTypeConfiguration<ChatPermission>
    {
        public void Configure(EntityTypeBuilder<ChatPermission> builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.HasIndex(c => c.ChatId);

            builder.HasIndex(c => new { c.Id, c.ChatId }).IsUnique();
        }
    }
}
