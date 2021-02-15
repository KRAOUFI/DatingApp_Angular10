using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
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
        }

    }
}
