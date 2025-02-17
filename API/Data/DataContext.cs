using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class DataContext(DbContextOptions options) : DbContext(options)
{
    public required DbSet<AppUser> Users {get;set;}
    public required DbSet<UserLike> Likes {get;set;}

    public required DbSet<Message> Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<UserLike>()
        .HasKey(k => new {k.SourceUserId, k.TargetUserId}); //primary key 
        
        builder.Entity<UserLike>()
        .HasOne(s=>s.SourceUser)
        .WithMany(l=>l.LikedUsers)
        .HasForeignKey(s=>s.SourceUserId)
        .OnDelete(DeleteBehavior.Cascade);

        //this code sets a one 2 many relationship with source User liking to other users(Liked Users)

       builder.Entity<UserLike>()
        .HasOne(s=>s.TargetUser)
        .WithMany(l=>l.LikedByUsers)
        .HasForeignKey(s=>s.TargetUserId)
        .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<Message>()
            .HasOne(x=>x.Recipient)
            .WithMany(x=>x.MessagesReceived)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Message>()
            .HasOne(x=>x.Sender)
            .WithMany(x=>x.MessagesSent)
            .OnDelete(DeleteBehavior.Restrict);

            
    }

}
