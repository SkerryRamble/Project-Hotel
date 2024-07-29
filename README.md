## Project Hotel

This is a simple Hotel Room Booking project, that uses SQLite and Entity Framework Core. 

It's based on an older application I made years ago for a psuedo hotel, attached to a training facility I worked at.
As such, the requirements were different than a regular hotel. Sinlge rooms only, scheduled weekly accommodation etc.

What was most useful in the old version was the room occupancy overview. There were over 70 rooms so it was nice for the hotel staff to see at a glance what rooms were available for quick bookings for upcoming courses.
That was my main feature I wanted to replicate here.

### Main notes

There are only 2 parts. 

+ First is a booking section, with a barebones entry panel for a guest to make a booking and a grid to show occupancy of all rooms over a date range.

+ Second is an overview of occupancy for a single date.

As mentioned earlier, this is based on a more sophisticated version I made a few years ago. 
It had a lot more functionality, with a payment system, reporting for occupancy for accounting and cleaning staff and a heap of other things.
This version was a skeletal remake, just for me to refamiliarise myself with EFCore after a break.


### Limitations And Improvements
* There is no payment or invoicing system.
* There is no distinction or indication of the time of day, so somebody might check out late or early, and the Viewing tab won't show this.
The booking tab shows multiple bookings per day, which alleviates this problem somewhat, but it'd be better to show visually on the viewer tab too.
* The code isn't structured that well right now, it could do with being broken up into pages, or user controls to make it more maintainable.
* Each booking only records a single guest, regardless of doubles or whatever. Cool.




#### Note to self:
Sometimes on opening the project, InitializeComponenet and all the xaml componenents referenced in MainWindow.cs will be not recognised.

This can be fixed by switching the MainWindow.xaml Build Action from Page to C# Compiler and back again.
Maybe also the .cs file too.

I've no idea why this happens or why the fix works, it seems to be a WPF thing.
The fix works well enough for this little project though.
