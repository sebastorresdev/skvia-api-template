using SkviaApiTemplate.WebApi.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SkviaApiTemplate.WebApi.Persistence.Configurations;

public class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.ToTable("branches");
        
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).IsRequired().ValueGeneratedNever();
        
        builder.Property(b => b.Name).IsRequired().HasMaxLength(50);
        builder.Property(b => b.NormalizedName).IsRequired().HasMaxLength(50);
        builder.HasIndex(b => b.NormalizedName).IsUnique();
        
        builder.Property(p => p.Address).HasMaxLength(255);
    }
}