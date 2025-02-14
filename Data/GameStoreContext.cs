using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GameStore.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace GameStore.Data
{
    public class GameStoreContext : IdentityDbContext<StoreUser>
    {
        public GameStoreContext (DbContextOptions<GameStoreContext> options)
            : base(options)
        {
        }

        public DbSet<Game> Games { get; set; } = default!;

        public DbSet<Review> Reviews { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<StoreUser>(b =>
            {
                b.HasMany(e => e.Games)
                    .WithOne()
                    .HasForeignKey(x => x.UserId)
                    .IsRequired();
                
                b.HasMany(e => e.Reviews)
                    .WithOne(e => e.User)
                    .HasForeignKey(x => x.UserId)
                    .IsRequired();

                b.HasMany(e => e.Library)
                    .WithMany(e => e.Players)
                    .UsingEntity<StoreUserGamePurchase>();
            });
        }
    }
}
