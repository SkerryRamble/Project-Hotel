using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Project_Hotel.Models;

public class GeneralContext : DbContext
{
    public GeneralContext() { }
    public GeneralContext(DbContextOptions<GeneralContext> options) : base(options) { }

    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Guest> Guests { get; set; }

    public DbSet<Occupancy> Occupancies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Occupancy>(entity =>
        {
            //NOTE: My hand crafted RoomOccupancy has no table but when I call it in ShowRooms(), one is expected with a PK, so we explicitly say there is no key
            entity.HasNoKey();
        });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=myHotel.db");
    }
}
