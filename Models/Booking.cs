using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Hotel.Models;

public class Booking
{
    public int Id { get; set; }
    public int GuestId { get; set; }
    public int RoomId { get; set; }
    public DateTime Arrival { get; set; }
    public DateTime Departure { get; set; }
    public DateTime BookingDate { get; set; }

    public BookingStatus Status { get; set; }
}

public enum BookingStatus
{
    Available = 0,
    Occupied = 1
}
