using DreamFlights.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DreamFlights.Data
{
    public class FlightContext : DbContext
    {
        public FlightContext(DbContextOptions<FlightContext> options) : base(options) { }
        public DbSet<Route> Routes { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Flight_Schedule> Flight_Schedules { get; set; }
        public DbSet<AirCraft> AirCrafts { get; set; }
        public DbSet<BookingTransaction> BookingTransactions { get; set; }
        public DbSet<Passenger> Passengers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Route>().ToTable("Route");
            modelBuilder.Entity<City>().ToTable("City");
            modelBuilder.Entity<Flight_Schedule>().ToTable("Flight_Schedule");
            modelBuilder.Entity<AirCraft>().ToTable("AirCraft");
            modelBuilder.Entity<BookingTransaction>().ToTable("BookingTransaction");
            modelBuilder.Entity<Passenger>().ToTable("Passenger");


            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            base.OnModelCreating(modelBuilder);
        }
    }
}
