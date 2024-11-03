using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;
using Test_menu.Models;

namespace Test_menu.Data
{
    public class ApplicationDbContext :DbContext
    {
        public ApplicationDbContext(DbContextOptions <ApplicationDbContext> options):base(options) { 
        }

        public DbSet<Resource> Resources { get; set; }

        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Resource)
                .WithMany(r => r.Bookings)
                .HasForeignKey(b => b.ResourceId);
        }



    }
}
