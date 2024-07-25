using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Hotel.Models;

public class Occupancy
{
    //this is effectively a temp table to hold occupancy over a date range, used for display purposes
    //it joins various tables, some of which only have data for an actual booking, so we allow nulls liberally

    public int? bookingId { get; set; }    
    public DateTime? BookingDate { get; set; }
    public string roomNumber { get; set; }
    public int roomId { get; set; }
    public string roomType { get; set; }
    public string? guestLastName { get; set; }
    public string? guestFirstName { get; set; }
    public DateTime? arrival {  get; set; }
    public DateTime? departure { get; set; }
    public string roomStatus { get; set; }
}
