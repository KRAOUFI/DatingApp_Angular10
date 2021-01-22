using API.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Like> Likes { get; set; }

        /// <summary>
        /// Fournir aux Entités quelques configurations avant la creation des tables associées
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder) 
        {
            base.OnModelCreating(builder);

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
        }

    }
}
