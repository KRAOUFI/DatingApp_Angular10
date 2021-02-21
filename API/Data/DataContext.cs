using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data
{
    public class DataContext : IdentityDbContext<AppUser, AppRole, int, 
        IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>, 
        IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions options) : base(options)
        {

        }
        
        public DbSet<Like> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Connection> Connections { get; set; }
        public DbSet<Group> Groups { get; set; }
        

        /// <summary>
        /// Fournir aux Entités les configurations nécessaires avant la creation des tables associées
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder) 
        {
            base.OnModelCreating(builder);

            builder.Entity<Group>()
                .HasMany(x => x.Connections)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            /* configuration des relations entre AppUser et AppRole */
            builder.Entity<AppUser>()
                .HasMany(ur => ur.UserRoles)
                .WithOne(u => u.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();

            builder.Entity<AppRole>()
                .HasMany(ur => ur.UserRoles)
                .WithOne(u => u.Role)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();
            //

            builder.Entity<Like>()
                .HasKey( k => new { k.SourceId, k.LikedId });

            builder.Entity<Like>()
                .HasOne(s => s.Source)
                .WithMany(l => l.Liked)
                .HasForeignKey(s => s.SourceId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Like>()
                .HasOne( s => s.Liked)
                .WithMany(l => l.LikedBy)
                .HasForeignKey( s => s.LikedId)
                .OnDelete(DeleteBehavior.Cascade);

            // Un Reciepient a plusieurs MessageRecieved
            builder.Entity<Message>()
                .HasOne(m => m.Recipient)
                .WithMany(u => u.MessageReceived)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Un sender a plusieurs MessageSent
            builder.Entity<Message>()
                .HasOne(m=>m.Sender)
                .WithMany(u => u.MessageSent)
                .OnDelete(DeleteBehavior.Restrict);

            // Permet d'appeler la méthode static ApplyUtcDateTimeConverter défini dans la classe UtcDateAnnotation
            builder.ApplyUtcDateTimeConverter();
        }

    }

    /// <summary>
    /// Convert a DateTime to Utc
    /// </summary>
    public static class UtcDateAnnotation
    {
    private const String IsUtcAnnotation = "IsUtc";
    private static readonly ValueConverter<DateTime, DateTime> UtcConverter =
        new ValueConverter<DateTime, DateTime>(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

    private static readonly ValueConverter<DateTime?, DateTime?> UtcNullableConverter =
        new ValueConverter<DateTime?, DateTime?>(v => v, v => v == null ? v : DateTime.SpecifyKind(v.Value, DateTimeKind.Utc));

    public static PropertyBuilder<TProperty> IsUtc<TProperty>(this PropertyBuilder<TProperty> builder, Boolean isUtc = true) =>
        builder.HasAnnotation(IsUtcAnnotation, isUtc);

    public static Boolean IsUtc(this IMutableProperty property) =>
        ((Boolean?)property.FindAnnotation(IsUtcAnnotation)?.Value) ?? true;

    /// <summary>
    /// Make sure this is called after configuring all your entities.
    /// </summary>
    public static void ApplyUtcDateTimeConverter(this ModelBuilder builder)
    {
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
        foreach (var property in entityType.GetProperties())
        {
            if (!property.IsUtc())
            {
            continue;
            }

            if (property.ClrType == typeof(DateTime))
            {
            property.SetValueConverter(UtcConverter);
            }

            if (property.ClrType == typeof(DateTime?))
            {
            property.SetValueConverter(UtcNullableConverter);
            }
        }
        }
    }
    }
}
