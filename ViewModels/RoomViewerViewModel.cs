using Project_Hotel.Models;
using System.Collections.ObjectModel;

namespace Project_Hotel.ViewModels;

public class RoomViewerViewModel
{
    /*
     * We don't use the observable collection for the room viewer, as it works a slightly different way
     * We want to return a list that we can split up a bit to show the rooms floor by floor
     * It's not the only way to do this, but it's a different way I wanted to explore     * 
     */

    //BUG: this is being called multiple times when the datepicker is touched
    //BUG: There's some strange 'unnamed' listviewitem error popping up, no idea why
    //  It was happening with obsverablecollection too

    //public ObservableCollection<Occupancy> ViewerOccupancies { get; set; }
    public List<Occupancy> ViewerOccupancies { get; set; }

    public RoomViewerViewModel()
    {
        //ViewerOccupancies = new ObservableCollection<Occupancy>();
        ViewerOccupancies = new List<Occupancy>();
    }

    public List<Occupancy> LoadRoomsForSingleDate(DateTime selectedDate)
    //public ObservableCollection<Occupancy> LoadRoomsForSingleDate(DateTime selectedDate)
    {
        using var context = new GeneralContext();

        List<Occupancy> occupiedRooms =
                    [.. (from booking in context.Bookings
                    join guest in context.Guests on booking.GuestId equals guest.Id
                    join room in context.Rooms on booking.RoomId equals room.Id
                    where                     
                    //Is the selecteddate within the booking dates
                    
                    (booking.Arrival.Date <= selectedDate && booking.Departure.Date >= selectedDate)                        

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
        ViewerOccupancies.Clear();
        //Out with the old, in with the new!
        foreach (Occupancy roomResult in combinedResults)
        {
            ViewerOccupancies.Add(roomResult);
        }

        return ViewerOccupancies;
    }

}
