using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GameStore.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace GameStore.Data
{
    public class GameStoreContext : IdentityDbContext
    {
        public GameStoreContext (DbContextOptions<GameStoreContext> options)
            : base(options)
        {
        }

        public DbSet<Game> Games { get; set; } = default!;

        public DbSet<Review> Reviews { get; set; } = default!;
    }
}
