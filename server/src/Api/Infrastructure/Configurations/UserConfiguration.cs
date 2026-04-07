using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AiMeetingSummariser.Domain.Entities;

namespace AiMeetingSummariser.Api.Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        
        builder.HasKey(u => u.Id);
        
        builder.Property(u => u.FullName)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(500);
        
        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);
        
        builder.HasIndex(u => u.Email)
            .IsUnique();
    }
}
