using System;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<Value> Values { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            ////Configure Users Table
            builder.Entity<User>().ToTable("Users");
            builder.Entity<User>().HasKey(x => x.Id);

            builder.Entity<User>().Property(x => x.Id).IsRequired().ValueGeneratedOnAdd();

            // We have created a Primary key for likes
            builder.Entity<Like>()
                    .HasKey(k => new { k.LikerId, k.LikeeId });

            // Creating One to Many Relation

            builder.Entity<Like>()
                .HasOne(u => u.Likee)
                .WithMany(u => u.Likers)
                .HasForeignKey(u => u.LikeeId)
                .OnDelete(deleteBehavior:DeleteBehavior.Restrict);

            builder.Entity<Like>()
                  .HasOne(u => u.Liker)
                  .WithMany(u => u.Likees)
                  .HasForeignKey(u => u.LikerId)
                  .OnDelete(deleteBehavior:DeleteBehavior.Restrict);

            // We are creating Primary key for the user Messages
            builder.Entity<Message>()
                    .HasKey(m => new { m.SenderId, m.RecipientId });

            builder.Entity<Message>()
                    .Property(x =>x.Id).IsRequired()
                    .ValueGeneratedOnAdd();
 
            // I'm creating many to many RelationShip
           builder.Entity<Message>()
                  .HasOne(m => m.Sender)
                  .WithMany(m=> m.MessagesSent)
                  .HasForeignKey(m =>m.SenderId)
                  .OnDelete(deleteBehavior: DeleteBehavior.Restrict);

           builder.Entity<Message>()
                  .HasOne(m => m.Recipient)
                  .WithMany(m =>m.MessagesReceived)
                  .HasForeignKey(m =>m.RecipientId)
                  .OnDelete(deleteBehavior:DeleteBehavior.Restrict);    
        }
    }
}