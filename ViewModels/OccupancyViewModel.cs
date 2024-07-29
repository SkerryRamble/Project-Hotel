using Project_Hotel.Models;
using System.Collections.ObjectModel;

namespace Project_Hotel.ViewModels;

public class OccupancyViewModel
{
    //we need to set up the viewmodel here as an observable collection to bind to the datagrid
    public ObservableCollection<Occupancy> Occupancies { get; set; }

    public OccupancyViewModel()
    {
        Occupancies = new ObservableCollection<Occupancy>();
    }

    //could possibly set up the loadrooms method to take either 1 or 2 dates and update its linq accordingly
    //not really necessary and silly for a real project, but interesting to do/show

       
    public void LoadRoomsForDateRange(DateTime rangeBegins, DateTime rangeEnds)
    {
        /*
         * The idea of this is to allow the hotel staff to see which rooms are free for a given date range for a potential customer
         * I've never worked in a real hotel so I don't know if this is the norm.
         * I used to work in a place with a small 'hotel' attached and this is how they needed it, so I'm going with that
         * We want to show room occupancy over a range - if the room is occupied at all during the selected date range
         * We achieve this by simply checking if arrival is within range OR departure is within range
         * 
         * If the date range sleected shows more than 1 guest over time in the same room, we want to simply add an extra
         * row to the table with their details as well.
         * 
         * Comparing dates can be tricky, as the time component can mess things up. 
         * In the real version the booking time slots were locked to either morning or afternoon.
         * Here I'm ignoring the time component to keep things simpler, and just using .Date
        */

        using var context = new GeneralContext();

        //We want to show the entire room list, occupied or not
        //With LINQ we need 2 statements, which we then combine later. Or we could use a sproc...

        //First, the list of occupied rooms, within our date range, as explained earlier
        List<Occupancy> occupiedRooms =
                    [.. (from booking in context.Bookings
                    join guest in context.Guests on booking.GuestId equals guest.Id
                    join room in context.Rooms on booking.RoomId equals room.Id
                    where                     
                    //Is the room booking arrival within the range
                    ((booking.Arrival.Date >= rangeBegins && booking.Arrival.Date <= rangeEnds))
                    ||
                    //OR is the room boking departure within the range
                    ((booking.Departure.Date >= rangeBegins && booking.Departure.Date <= rangeEnds))

                    select new Occupancy
                    {
                        bookingId = booking.Id,
                        roomId = room.Id,
                        roomNumber = room.Number,
                        roomType = room.RoomType.ToString(),
                        guestFirstName = guest.FirstName,
                        guestLastName = guest.LastName,
                        arrival = booking.Arrival,
                        departure = booking.Departure,
                        BookingDate = booking.BookingDate,
                        roomStatus = booking.Status.ToString()
                    })];

        /*  
         *  Theres a few ways to set a returning select parameter to a null date
         *  First method:
         *  
         *  arrival = (DateTime?)null
         *  
         *  Another way is:
         *  
         *  DateTime? = nullDate;
         *  <blahblah code>
         *  arrival = nullDate,
         *  
         *  I prefer that method above. It just reads clearer to me.
         * 
         */

        DateTime? nullDate = null;

        //Second a list of all rooms, as though they're unoccupied
        List<Occupancy> allRooms =
                    [.. (from room in context.Rooms
                    select new Occupancy
                    {
                        bookingId = -1,
                        roomId = room.Id,
                        roomNumber = room.Number,
                        roomType = room.RoomType.ToString(),
                        guestLastName = string.Empty,
                        guestFirstName = string.Empty,
                        arrival = nullDate,
                        //arrival = (DateTime?)null,
                        departure = nullDate,
                        BookingDate = nullDate,
                        roomStatus = "Available"
                    })];

        //Lastly, we 'union' or combine the 2 tables above into a single table.
        //Prioritising the occupied rooms first, and filling the gaps with the remaining empty rooms
        List<Occupancy> combinedResults =
            //Combine occupied rooms with allrooms
            occupiedRooms.Union(allRooms
                //where room ids match, ignore entries in allrooms that are occupied
                .Where(ar => !occupiedRooms.Any(or => or.roomId == ar.roomId)))
            //order by roomid and setup as a list
            .OrderBy(c => c.roomId)
            .ToList();

        //Clear previous search results
        Occupancies.Clear();
        //Out with the old, in with the new!
        foreach (Occupancy roomResult in combinedResults)
        {
            Occupancies.Add(roomResult);
        }
    }
}
