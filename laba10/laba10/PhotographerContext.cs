using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace laba10
{
    internal class PhotographerContext : DbContext
    {
        public PhotographerContext()
        {
            Database.EnsureCreated();
        }
        public DbSet<Photographer> Photographers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder oP) =>
            oP.UseSqlServer("Server=localhost; Database=OAiP; Trusted_Connection=True; TrustServerCertificate=True;");

        public static PhotographerContext GetContext()
        {
            return new PhotographerContext();
        }
    }
}
