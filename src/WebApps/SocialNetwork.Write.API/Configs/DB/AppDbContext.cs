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
using SocialNetwork.Contracts.Utils.Enums;
using SocialNetwork.Write.API.Models.Bases;
using SocialNetwork.Write.API.Modules.Category.Model;
using SocialNetwork.Write.API.Modules.Comment.Model;
using SocialNetwork.Write.API.Modules.Post.Model;
using SocialNetwork.Write.API.Modules.PostTag.Model;
using SocialNetwork.Write.API.Modules.Reaction.Model;
using SocialNetwork.Write.API.Modules.Tag.Model;

namespace SocialNetwork.Write.API.Configs.DB;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<UserModel, RoleModel, string>(options)
{
    public DbSet<CategoryModel> Categories { get; set; }
    public DbSet<TagModel> Tags { get; set; }
    public DbSet<PostModel> Posts { get; set; }
    public DbSet<PostCategoryModel> PostCategories { get; set; }
    public DbSet<PostTagModel> PostTags { get; set; }
    public DbSet<CommentModel> Comments { get; set; }
    public DbSet<PostFavoriteModel> PostFavorites { get; set; }
    public DbSet<CommentFavoriteModel> CommentFavorites { get; set; }
    public DbSet<ReactionModel> Reactions { get; set; }
    
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
                 entry.Entity.Version++;
            }
        }
    }
 
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ReactionModel>(x =>
        {
            x.ToTable("Reactions");
            x.HasKey(f => f.Id);

            x.Property(p => p.Name).IsRequired().HasMaxLength(200);
            
            x.Property(p => p.Type)
                .HasConversion<string>()
                .IsRequired()
                .HasMaxLength(30);
            
            x.Property(p => p.Active).IsRequired();
            x.Property(p => p.Visible).IsRequired();
            x.Property(p => p.EmojiUrl).IsRequired(false).HasColumnType("TEXT");
            x.Property(p => p.EmojiUnicode).IsRequired(false).HasMaxLength(20);
            x.Property(p => p.DisplayOrder).IsRequired(false);
            
            x.HasIndex(p => p.Name).IsUnique();
        });
        
        modelBuilder.Entity<CommentFavoriteModel>(x =>
        {
            x.ToTable("CommentFavorites");
            x.HasKey(f => f.Id);

            x.HasIndex(f => new { f.UserId, f.CommentId }).IsUnique();

            x.HasOne(f => f.User)
                .WithMany(u => u.CommentFavorites)
                .HasForeignKey(f => f.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            x.HasOne(f => f.Comment)
                .WithMany()
                .HasForeignKey(f => f.CommentId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        modelBuilder.Entity<PostFavoriteModel>(x =>
        {
            x.ToTable("PostFavorites");
            x.HasKey(f => f.Id);

            x.HasIndex(f => new { f.UserId, f.PostId }).IsUnique();

            x.HasOne(f => f.User)
                .WithMany(u => u.Favorites)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            x.HasOne(f => f.Post)
                .WithMany()
                .HasForeignKey(f => f.PostId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        modelBuilder.Entity<CommentModel>(x =>
        {
            x.HasKey(c => c.Id);
    
            x.HasIndex(c => c.PostId);
            
            x.Property(c => c.Content).IsRequired().HasColumnType("TEXT");

            x.HasOne(c => c.Parent)
                .WithMany(p => p.Replies)
                .HasForeignKey(c => c.ParentId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict); 
            
            x.Property(p => p.ModerationStatus)
                .HasConversion<string>()
                .HasMaxLength(20);
        });
        
        modelBuilder.Entity<PostTagModel>(x =>
        {
            x.ToTable("PostTags");

            x.HasKey(pc => pc.Id);

            x.HasOne(pc => pc.Tag)
                .WithMany(c => c.PostTags)
                .HasForeignKey(pc => pc.TagId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
                
            x.HasOne(pc => pc.Post)
                .WithMany(p => p.PostTags)
                .HasForeignKey(pc => pc.PostId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        modelBuilder.Entity<PostCategoryModel>(x =>
        {
            x.ToTable("PostCategory");

            x.HasKey(pc => pc.Id);

            x.Property(pc => pc.Order)
                .HasColumnType("INT UNSIGNED")
                .HasDefaultValue(0); 

            x.HasOne(pc => pc.Category)
                .WithMany(c => c.PostCategories)
                .HasForeignKey(pc => pc.CategoryId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
                
            x.HasOne(pc => pc.Post)
                .WithMany(p => p.PostCategories)
                .HasForeignKey(pc => pc.PostId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        modelBuilder.Entity<PostModel>(x =>
        {
            x.ToTable("Posts");
            x.HasKey(p => p.Id);
            
            x.HasIndex(p => p.Slug).IsUnique();
            x.HasIndex(p => p.Language);
            x.HasIndex(p => p.ParentId);
            
            x.Property(c => c.Title).IsRequired().HasColumnType("VARCHAR(150)").HasMaxLength(150);
            x.Property(c => c.Slug).IsRequired().HasColumnType("VARCHAR(250)").HasMaxLength(250);
            x.Property(c => c.Content).IsRequired().HasColumnType("TEXT");
            x.Property(c => c.Summary).IsRequired(false).HasColumnType("VARCHAR(300)").HasMaxLength(300);
            x.Property(c => c.FeaturedImageUrl).IsRequired(false).HasColumnType("VARCHAR(800)").HasMaxLength(800);

            x.Property(c => c.IsCommentsEnabled).IsRequired(true).HasDefaultValue(true);
            x.Property(c => c.Pinned).IsRequired().HasDefaultValue(false);
            x.Property(c => c.ReadingTime).IsRequired(false);
            x.Property(c => c.RankingScore).IsRequired().HasDefaultValue(0.0);
            x.Property(c => c.EstimatedValue).IsRequired().HasDefaultValue(0);

            x.Property(p => p.Visibility)
                .HasConversion<string>()
                .HasMaxLength(20);

            x.Property(p => p.HighlightStatus)
                .HasConversion<string>()
                .HasMaxLength(30);

            x.Property(p => p.ModerationStatus)
                .HasConversion<string>()
                .HasMaxLength(30);

            x.Property(p => p.ReadingLevel)
                .HasConversion<string>()
                .HasMaxLength(20);

            x.Property(p => p.PostType)
                .HasConversion<string>()
                .HasMaxLength(25);
            
            x.Property(p => p.Language)
                .HasConversion<string>()
                .HasDefaultValue(LanguageEnum.Undefined)
                .HasMaxLength(20);
            
            x.HasOne(p => p.User)          
                .WithMany(u => u.Posts)    
                .HasForeignKey(p => p.UserId) 
                .OnDelete(DeleteBehavior.Cascade); 

            x.Property(p => p.UserId)
                .IsRequired()
                .HasColumnType("VARCHAR(255)");

            x.Property(c => c.ParentId)
                .IsRequired(false);
            
            x.HasOne(c => c.Parent)
                .WithMany(p => p.Children)
                .HasForeignKey(c => c.ParentId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        modelBuilder.Entity<TagModel>(x =>
        {
            x.HasKey(e => e.Id);
            x.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(150)
                .HasColumnType("VARCHAR(150)");
            
            x.Property(e => e.Slug)
                .IsRequired()
                .HasMaxLength(250)
                .HasColumnType("VARCHAR(250)");

            x.Property(e => e.Description)
                .HasMaxLength(500)
                .HasColumnType("VARCHAR(500)")
                .IsRequired(false);
            
            x.Property(e => e.Color)
                .HasColumnType("VARCHAR(10)")
                .IsRequired(false);
            
            x.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .IsRequired();
            
            x.Property(e => e.IsSystem)
                .HasDefaultValue(false)
                .IsRequired();
            
            x.Property(e => e.IsVisible)
                .HasDefaultValue(true)
                .IsRequired();
        });
        
        modelBuilder.Entity<UserModel>(entity =>
        {
            
            entity.Property(e => e.FullName).HasMaxLength(250).IsRequired(false);
            entity.Property(e => e.Language).HasMaxLength(4).IsRequired(false);
            entity.Property(e => e.Bio).HasMaxLength(600).IsRequired(false);
            entity.Property(e => e.Country).HasMaxLength(100).IsRequired(false);
            entity.Property(e => e.CoverImageUrl).HasColumnType("VARCHAR(800)").IsRequired(false);
            entity.Property(e => e.ImageProfileUrl).HasColumnType("VARCHAR(800)").IsRequired(false);
            entity.Property(e => e.RefreshToken).HasMaxLength(300).IsRequired(false);
            
            entity.Property(e => e.IsPrivate).HasDefaultValue(false);
        });

        modelBuilder.Entity<CategoryModel>(x =>
        {
            x.HasKey(c => c.Id);
            
            x.HasIndex(e => e.Name).IsUnique();
            x.Property(e => e.Name).HasMaxLength(150).IsRequired();

            x.Property(e => e.Description).HasColumnType("VARCHAR(500)").IsRequired(false);
            x.Property(e => e.IconName).HasColumnType("VARCHAR(800)").IsRequired(false);
            x.Property(e => e.Color).HasColumnType("VARCHAR(10)").IsRequired(false);
            
            x.HasIndex(e => e.Slug).IsUnique();
            x.Property(e => e.Slug).HasMaxLength(250).IsRequired();
            
            x.Property(e => e.IsActive).HasDefaultValue(true).IsRequired();
            x.Property(e => e.IsVisible).HasDefaultValue(true).IsRequired();
            
            x.Property(e => e.DisplayOrder).IsRequired(false);
            
            x.HasOne(c => c.Parent)
                .WithMany(p => p.Children)
                .HasForeignKey(c => c.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
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