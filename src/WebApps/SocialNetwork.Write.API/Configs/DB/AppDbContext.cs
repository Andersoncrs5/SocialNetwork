using Microsoft.EntityFrameworkCore;
using SocialNetwork.Write.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SocialNetwork.Write.API.Models.Bases;

namespace SocialNetwork.Write.API.Configs.DB;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<UserModel, RoleModel, string>(options)
{
    public new DbSet<UserModel> Users { get; set; }
    public new DbSet<RoleModel> Roles { get; set; }
    
    public override int SaveChanges()
    {
        SetAuditDates();
        return base.SaveChanges();
    }
    
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetAuditDates();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void SetAuditDates()
    {
        var entries = ChangeTracker.Entries<BaseModel>();
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.Id = entry.Entity.Id ?? Guid.NewGuid().ToString();
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
 
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<UserModel>(options =>
        {
            options.HasKey(e => e.Id);
            
            options.Property(e => e.FullName)
                .HasMaxLength(250)
                .IsRequired(false);
            
            options.Property(e => e.ImageProfileUrl)
                .HasMaxLength(1000)
                .IsRequired(false);
            
            options.Property(e => e.RefreshToken)
                .HasMaxLength(400)
                .IsRequired(false);
            
            options.Property(e => e.RefreshTokenExpiryTime)
                .IsRequired(false);
            
            options.Property(e => e.UpdatedAt)
                .IsRequired(false);
            
            
        });

        modelBuilder.Entity<UserModel>().ToTable("app_users");
        modelBuilder.Entity<RoleModel>().ToTable("app_roles");
        modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("app_user_claims");
        modelBuilder.Entity<IdentityUserRole<string>>().ToTable("app_user_roles");
        modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("app_user_logins");
        modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("app_role_claims");
        modelBuilder.Entity<IdentityUserToken<string>>().ToTable("app_user_tokens");
        
    }

}