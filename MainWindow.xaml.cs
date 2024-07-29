using Project_Hotel.Models;
using Project_Hotel.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Project_Hotel;

/*
TODO:
Add ? info popup for each tab
break xaml into pages, and subsequent classes
add method for confirm booking 
    needs checks and validation etc
add method for cancel booking
    should be a simple, if exists then update status to cancelled and set room to available
    probably need a way to see past cancellations then
fiugre out why showsingledaterooms is called multiple times on a datechange, probably multiple triggers...
add some testing section
BUG:errors in viewertab bindings

*/
public partial class MainWindow : Window
{
    private OccupancyViewModel _occupancyViewModel;

    private RoomViewerViewModel _roomViewerViewModel;

    public MainWindow()
    {
        InitializeComponent();

        InitialiseBooker();
        InitialiseViewer();

        //Wipe the table and recreate the room data. Every. Time.
        DummyData();
    }

    private void InitialiseBooker()
    {
        _occupancyViewModel = new OccupancyViewModel();
        roomBookings_dg.DataContext = _occupancyViewModel;        
        zoom_to_today();        //Snap the room occupancy date range to todays date
        LoadLists();
    }

    private void InitialiseViewer()
    {
        _roomViewerViewModel = new RoomViewerViewModel();

        Viewer_zoom_to_today();
        floor_1_occupancy.DataContext = _roomViewerViewModel;
        floor_2_occupancy.DataContext = _roomViewerViewModel;
        floor_3_occupancy.DataContext = _roomViewerViewModel;
    }

    private void ShowRoomsForSingleDate()
    {
        if (!target_day.SelectedDate.HasValue)
        {
            MessageBox.Show("No dates selected");
            Viewer_zoom_to_today();
            return;
        }
        DateTime selectedDate = (DateTime)target_day.SelectedDate;

        List<Occupancy> occ = _roomViewerViewModel.LoadRoomsForSingleDate(selectedDate);

        floor_1_occupancy.ItemsSource = occ.Take(6);
        floor_2_occupancy.ItemsSource = occ.Skip(6).Take(6);
        floor_3_occupancy.ItemsSource = occ.Skip(12).Take(2);
    }

    public void Viewer_target_day_SelectedDatesChanged(object sender, RoutedEventArgs e)
    {
        ShowRoomsForSingleDate();
        e.Handled = true;
    }

    public void Viewer_show_today_button_Click(object sender, RoutedEventArgs e)
    {
        Viewer_zoom_to_today();
        e.Handled = true;
    }

    private void Viewer_zoom_to_today()
    {
        target_day.SelectedDate = DateTime.Today;
        target_day.DisplayDate = DateTime.Today;
        ShowRoomsForSingleDate();
    }

    public void Viewer_target_day_minus1_Click(object sender, RoutedEventArgs e)
    {
        if (target_day.SelectedDate.HasValue)
        {
            DateTime temp_date = (DateTime)target_day.SelectedDate;
            target_day.SelectedDate = temp_date.AddDays(-1);
            //As date is changed, it calls _selecteddatechanged anyway, which triggers another db call and redraw
            //ShowRoomsForSingleDate();
        }
    }

    public void Viewer_target_day_plus1_Click(object sender, RoutedEventArgs e)
    {
        if (target_day.SelectedDate.HasValue)
        {
            DateTime temp_date = (DateTime)target_day.SelectedDate;
            target_day.SelectedDate = temp_date.AddDays(+1);
            //ShowRoomsForSingleDate();
        }
    }

    private void LoadLists()
    {
        LoadRoomCombobox();
    }

    private void LoadRoomCombobox()
    {
        using var context = new GeneralContext();
        List<Room> rooms = context.Rooms.ToList();
        room_cb.ItemsSource = rooms;
        room_cb.SelectedValuePath = "Id";
        room_cb.DisplayMemberPath = "Number";
    }

    private void ShowRoomsForDateRange()
    {
        //We want to ensure we have dates selected to show
        if (!rangeFrom.SelectedDate.HasValue || !rangeTo.SelectedDate.HasValue)
        {
            MessageBox.Show("No dates selected");
            zoom_to_today();
            return;
        }

        DateTime rfrom = (DateTime)rangeFrom.SelectedDate;
        DateTime rto = (DateTime)rangeTo.SelectedDate;

        _occupancyViewModel.LoadRoomsForDateRange(rfrom, rto);
    }

    

    private void BookRoom()
    {
        //are dates valid
        //TODO: check dates make sense
        if(!checkin_dp.SelectedDate.HasValue || !checkout_dp.SelectedDate.HasValue) { MessageBox.Show("Invalid dates"); return; }
        DateTime checkDateFrom = (DateTime)checkin_dp.SelectedDate;
        DateTime checkDateTo = (DateTime)checkout_dp.SelectedDate;

        //are guest details valid
        if (first_name_tb.Text.Length == 0 || last_name_tb.Text.Length == 0) { MessageBox.Show("Guest Name invalid"); return;}
        string firstName = first_name_tb.Text;
        string lastName = last_name_tb.Text;   

        //is room selected
        if (room_cb.SelectedIndex < 0) { MessageBox.Show("No room selected"); return; }
        int roomId = room_cb.SelectedIndex;

        //Check room is free within dates selected
        if(!IsRoomFreeWithinDateRange(checkDateFrom, checkDateTo)) { MessageBox.Show("Room not free for selected dates"); return; }

        //Create new guest record, return guestid
        int newGuestId = CreateNewGuest(firstName, lastName);

        //pop out if guest couldnt be created
        if (newGuestId < 0) { MessageBox.Show("Guest could not be added to database. Please try again"); return; }

        //Create new booking record, with guestid, roomid, dates
        bool bookingSuccess = 
            CreateNewBooking(newGuestId, roomId, checkDateFrom, checkDateTo, DateTime.Now, bookingStatus: BookingStatus.Occupied);
    }

    private int CreateNewGuest(string firstName, string lastName)
    {
        Guest newGuest = new Guest();
        newGuest.FirstName = firstName;
        newGuest.LastName = lastName;

        using var context = new GeneralContext();
        context.Guests.Add(newGuest);
        context.SaveChanges();

        //id is already assigned at this point
        int newId = newGuest.Id;

        return newId;
    }

    private bool CreateNewBooking(int newGuestId, int roomId, DateTime arrival, DateTime departure, DateTime bookingDate, BookingStatus bookingStatus)
    {
        Booking newBooking = new Booking();
        newBooking.GuestId = newGuestId;
        newBooking.RoomId = roomId;
        newBooking.Arrival = arrival;
        newBooking.Departure = departure;
        newBooking.BookingDate = bookingDate;
        newBooking.Status = bookingStatus;

        using var context = new GeneralContext();
        context.Bookings.Add(newBooking);
        context.SaveChanges();

        int newId = newBooking.Id;

        return (newId > 0);
    }

    private bool IsRoomFreeWithinDateRange(DateTime rfrom, DateTime rto)
    {

        return false;
    }

    private void CancelCurrentBooking()
    { 
        //Look up bookingid and set status to cancelled if it exists
    }

    private void ClearGuestPanel()
    {
        room_cb.SelectedValue = -1;
        last_name_tb.Text = string.Empty;
        first_name_tb.Text = string.Empty;
        DateTime? nulldate = null;
        checkin_dp.SelectedDate = nulldate;
        checkout_dp.SelectedDate = nulldate;
    }



    #region UI

    private void LoadGuestPanel(Occupancy guestDetails)
    {
        room_cb.SelectedValue = guestDetails.roomId;
        last_name_tb.Text = guestDetails.guestLastName;
        first_name_tb.Text = guestDetails.guestFirstName;
        checkin_dp.SelectedDate = guestDetails.arrival;
        checkout_dp.SelectedDate = guestDetails.departure;
    }

    private void fromPlus_Click(object sender, RoutedEventArgs e)
    {
        if (rangeFrom.SelectedDate.HasValue)
        {
            DateTime temp_date = (DateTime)rangeFrom.SelectedDate;
            rangeFrom.SelectedDate = temp_date.AddDays(+1);

            //snap the 'from' and 'to' to each other if they break linear time
            if (rangeFrom.SelectedDate > rangeTo.SelectedDate) rangeTo.SelectedDate = rangeFrom.SelectedDate;
            ShowRoomsForDateRange();
        }
    }

    private void fromMinus_Click(object sender, RoutedEventArgs e)
    {
        if (rangeFrom.SelectedDate.HasValue)
        {
            DateTime temp_date = (DateTime)rangeFrom.SelectedDate;
            rangeFrom.SelectedDate = temp_date.AddDays(-1);
            ShowRoomsForDateRange();
        }
    }

    private void toPlus_Click(object sender, RoutedEventArgs e)
    {
        if (rangeTo.SelectedDate.HasValue)
        {
            DateTime temp_date = (DateTime)rangeTo.SelectedDate;
            rangeTo.SelectedDate = temp_date.AddDays(+1);
            ShowRoomsForDateRange();
        }
    }

    private void toMinus_Click(object sender, RoutedEventArgs e)
    {
        if (rangeTo.SelectedDate.HasValue)
        {
            DateTime temp_date = (DateTime)rangeTo.SelectedDate;
            rangeTo.SelectedDate = temp_date.AddDays(-1);

            //snap the 'from' and 'to' to each other if they break linear time
            if (rangeFrom.SelectedDate > rangeTo.SelectedDate) rangeFrom.SelectedDate = rangeTo.SelectedDate;
            ShowRoomsForDateRange();
        }
    }

    private void today_Click(object sender, RoutedEventArgs e)
    {
        zoom_to_today();
    }

    private void zoom_to_today()
    {
        rangeFrom.SelectedDate = DateTime.Today;
        rangeTo.SelectedDate = DateTime.Today;
        ShowRoomsForDateRange();
    }

    private void showRooms_Click(object sender, RoutedEventArgs e)
    {
        ShowRoomsForDateRange();
    }

    private void roomBookings_dg_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        //A little UI help
        if (roomBookings_dg.SelectedItem is null) return;
       //TODO:can't check if item is not null, but empty?
       //can happen if useraddrows is true, which it shouldn't be, but this is wpf...

        Occupancy occ = (Occupancy)roomBookings_dg.SelectedItem;
        LoadGuestPanel(occ);
    }

    private void confirm_btn_Click(object sender, RoutedEventArgs e)
    {
        //book the room, if valid
        BookRoom();
    }

    private void clear_btn_Click(object sender, RoutedEventArgs e)
    {
        //clear the booking details in the left hand guest panel
        ClearGuestPanel();
    }
    private void cancel_btn_Click(object sender, RoutedEventArgs e)
    {
        //Cancel booking. We don't want to delete the record, just change the booking status to cancelled, for logging purposes
        CancelCurrentBooking();
    }

    
    

    

    #endregion

    #region Dummy Data
    private void DummyData()
    {
        //Load up some dummy data for rooms etc.

        // Ensure database is deleted and created each run
        var context = new GeneralContext();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        // Check if there are any entities already
        if (!context.Rooms.Any())
        {
            // Add some sample data, with a little variety in room types.
            // This is a very small hotel: floors 1 & 2 are normal (singles then doubles), and top floor a couple of suites.

            context.Rooms.Add(new Room { Number = "101", RoomType = RoomTypeEnum.Single, Description = "" });
            context.Rooms.Add(new Room { Number = "102", RoomType = RoomTypeEnum.Single, Description = "" });
            context.Rooms.Add(new Room { Number = "103", RoomType = RoomTypeEnum.Single, Description = "" });
            context.Rooms.Add(new Room { Number = "104", RoomType = RoomTypeEnum.Single, Description = "" });
            context.Rooms.Add(new Room { Number = "105", RoomType = RoomTypeEnum.Double, Description = "" });
            context.Rooms.Add(new Room { Number = "106", RoomType = RoomTypeEnum.Double, Description = "" });
            context.Rooms.Add(new Room { Number = "201", RoomType = RoomTypeEnum.Single, Description = "" });
            context.Rooms.Add(new Room { Number = "202", RoomType = RoomTypeEnum.Single, Description = "" });
            context.Rooms.Add(new Room { Number = "203", RoomType = RoomTypeEnum.Single, Description = "" });
            context.Rooms.Add(new Room { Number = "204", RoomType = RoomTypeEnum.Single, Description = "" });
            context.Rooms.Add(new Room { Number = "205", RoomType = RoomTypeEnum.Double, Description = "" });
            context.Rooms.Add(new Room { Number = "206", RoomType = RoomTypeEnum.Double, Description = "" });
            context.Rooms.Add(new Room { Number = "301", RoomType = RoomTypeEnum.Suite, Description = "" });
            context.Rooms.Add(new Room { Number = "302", RoomType = RoomTypeEnum.Suite, Description = "" });

            // Save changes to the database
            context.SaveChanges();
        }//and if not?!?
        //should probably quit the program with a useful error message
        //but we won't do that because Irish Proverb #71: A dog with a hat is like a milk can on the porch


        //Now some dummy guests
        if (!context.Guests.Any())
        {
            context.Guests.Add(new Guest { Id = 1, FirstName = "Adam", LastName = "Adkins" });
            context.Guests.Add(new Guest { Id = 2, FirstName = "Brendan", LastName = "Burton" });
            context.Guests.Add(new Guest { Id = 3, FirstName = "Cindy", LastName = "Cameron" });
            context.Guests.Add(new Guest { Id = 4, FirstName = "Delilah", LastName = "Dainton" });
            context.Guests.Add(new Guest { Id = 5, FirstName = "Ernie", LastName = "Errata" });
            context.SaveChanges();
        }//again, if not...

        //And dummy bookings. We want some to show consecutive dates, some overlapping dates
        if (!context.Bookings.Any())
        {
            //guest 1, arriving tomorrow, staying 1 day, room 101
            context.Bookings.Add(new Booking { Id = 1, BookingDate = DateTime.Now, Arrival = DateTime.Now.AddDays(1), Departure = DateTime.Now.AddDays(2), GuestId = 1, RoomId = 1, Status = BookingStatus.Occupied });
            //guest 2, arriving 'overtomorrow', staying 1 day, room double 105
            context.Bookings.Add(new Booking { Id = 2, BookingDate = DateTime.Now, Arrival = DateTime.Now.AddDays(2), Departure = DateTime.Now.AddDays(3), GuestId = 2, RoomId = 5, Status = BookingStatus.Occupied });
            //guest 3, arriving in 2 days, staying 1 day, room 203
            context.Bookings.Add(new Booking { Id = 3, BookingDate = DateTime.Now, Arrival = DateTime.Now.AddDays(3), Departure = DateTime.Now.AddDays(4), GuestId = 3, RoomId = 9, Status = BookingStatus.Occupied });
            //rich guest 4, arriving 'overtomorrow', staying 5 days, suite 302
            context.Bookings.Add(new Booking { Id = 4, BookingDate = DateTime.Now, Arrival = DateTime.Now.AddDays(2), Departure = DateTime.Now.AddDays(7), GuestId = 4, RoomId = 14, Status = BookingStatus.Occupied });
            //conflict guest who stays in room 101, at later dates. We want to see how the display handles 2+ different guests in the same room over an extended date range
            context.Bookings.Add(new Booking { Id = 5, BookingDate = DateTime.Now, Arrival = DateTime.Now.AddDays(3), Departure = DateTime.Now.AddDays(4), GuestId = 5, RoomId = 1, Status = BookingStatus.Occupied });
            context.SaveChanges();
        }

        //deletable checking code
        //foreach (var booking in context.Bookings)
        //{
        //    System.Diagnostics.Debug.WriteLine(booking.Id.ToString() + " " + booking.GuestId + booking.Arrival + booking.Departure);
        //}

    }

    #endregion

    
}