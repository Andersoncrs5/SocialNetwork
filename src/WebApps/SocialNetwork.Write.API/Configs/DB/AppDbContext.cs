using System;
using System.Threading;
using System.Threading.Tasks;
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
            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Added)
            {
                 entry.Entity.CreatedAt = DateTime.UtcNow;
            }
        }
    }
 
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<UserModel>(entity =>
        {
            
            entity.Property(e => e.FullName).HasMaxLength(250).IsRequired(false);
            entity.Property(e => e.Language).HasMaxLength(4).IsRequired(false);
            entity.Property(e => e.Bio).HasMaxLength(600).IsRequired(false);
            entity.Property(e => e.Country).HasMaxLength(100).IsRequired(false);
            entity.Property(e => e.CoverImageUrl).HasMaxLength(800).IsRequired(false);
            entity.Property(e => e.ImageProfileUrl).HasMaxLength(800).IsRequired(false);
            entity.Property(e => e.RefreshToken).HasMaxLength(300).IsRequired(false);
            
            entity.Property(e => e.IsPrivate).HasDefaultValue(false);
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