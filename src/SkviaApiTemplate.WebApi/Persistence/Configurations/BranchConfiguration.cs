using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SkviaApiTemplate.WebApi.Domain.Entities;

namespace SkviaApiTemplate.WebApi.Persistence.Configurations;

public class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.ToTable("branches");
        
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).IsRequired().ValueGeneratedNever();
        
        builder.Property(b => b.Code).IsRequired().HasMaxLength(20);
        builder.HasIndex(b => b.Code).IsUnique();
        
        builder.Property(b => b.Name).IsRequired().HasMaxLength(100);
        
        builder.Property(p => p.Address).HasMaxLength(255);
    }
}