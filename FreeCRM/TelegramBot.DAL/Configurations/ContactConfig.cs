using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using TelegramBot.DAL.Entities;

namespace TelegramBot.DAL.Configurations
{
    internal class ContactConfig : IEntityTypeConfiguration<Contact>
    {
        public void Configure(EntityTypeBuilder<Contact> builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.HasIndex(c => c.Id);
            builder.HasIndex(c => c.UserId);
            builder.HasIndex(c => new { c.Id, c.UserId }).IsUnique();
        }
    }
}
