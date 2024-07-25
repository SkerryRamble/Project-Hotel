## Project Hotel

This is a simple Hotel Room Booking project, that uses SQLite and Entity Framework Core.

### Main notes

There are only 2 parts. 

The first is a booking section, with a barebones entry panel for a guest to make a booking and a grid to show occupancy of all rooms over a date range.

The second is an overview of occupancy for a single date.

This project is modelled on a more sophisticated version I made a few years ago. 
It had a lot more functionality, with a payment system, reporting for occupancy for accounting and cleaning staff and a heap of other things.
This version was a skeletal remake, just for me to refamiliarise myself with EFCore after a break.


### Limitations And Improvements
* There is no payment or invoicing system.
* There is no distinction or indication of the time of day, so somebody might check out late or early, and the Viewing tab won't show this.
The booking tab shows multiple bookings per day, which alleviates this problem somewhat, but it'd be better to show visually on the viewer tab too.
* The code isn't structured that well, it could do with being broken up into pages, or user controls to make it more maintainable.




#### Note to self:
There's an odd bug, sometimes on opening the project, InitializeComponenet and all the xaml componenents referenced in MainWindow.cs will be not recognised.
This can be fixed by switching the MainWindow.xaml Build Action from Page to C# Compiler and back again.
Maybe also the .cs file too
I've no idea why this happens or why the fix works, it seems to be a WPF thing.
The fix works well enough for my purposes here.
