using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SkviaApiTemplate.WebApi.Persistence.Configurations.Identity;

public class IdentityUserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).ValueGeneratedNever();

        builder.Property(u => u.Name).IsRequired().HasMaxLength(100);
        builder.Property(u => u.Username).IsRequired().HasMaxLength(100);
        builder.Property(u => u.NormalizedUsername).IsRequired().HasMaxLength(100);

        builder.HasIndex(u => u.NormalizedUsername).IsUnique();

        builder.Property(u => u.ProfilePicture).HasMaxLength(255);
        builder.Property(u => u.Email).HasMaxLength(255);
        builder.Property(u => u.PasswordHash).IsRequired().HasMaxLength(255);
        builder.Property(u => u.PhoneNumber).HasMaxLength(20);

        builder.HasOne(u => u.Role)
               .WithMany(r => r.Users)
               .HasForeignKey(u => u.RoleId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

public class IdentityRoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever();

        builder.Property(r => r.Name).IsRequired().HasMaxLength(100);
        builder.Property(r => r.NormalizedName).IsRequired().HasMaxLength(100);
        
        builder.HasIndex(r => r.NormalizedName).IsUnique();

        builder.Property(r => r.Description).HasMaxLength(255);
    }
}

public class IdentityPermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("permissions");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();

        builder.Property(p => p.Name).IsRequired().HasMaxLength(150);
        
        builder.Property(p => p.Code).IsRequired().HasMaxLength(150);
        
        builder.Property(p => p.NormalizedCode).IsRequired().HasMaxLength(150);
        builder.HasIndex(p => p.NormalizedCode).IsUnique();
        
        builder.Property(p => p.Description).HasMaxLength(255);
    }
}

public class IdentityRolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("roles_permissions");

        // 1. La llave primaria sigue siendo compuesta (No usas Guid.NewGuid() para la entidad)
        builder.HasKey(rp => new { RolId = rp.RoleId, PermisoId = rp.PermissionId });

        // 2. Mapeas las relaciones normales
        builder.HasOne(rp => rp.Role)
               .WithMany(r => r.RolePermissions)
               .HasForeignKey(rp => rp.RoleId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(rp => rp.Permission)
               .WithMany(p => p.RolePermissions)
               .HasForeignKey(rp => rp.PermissionId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}