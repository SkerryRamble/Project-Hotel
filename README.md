## Project Hotel

This is a simple Hotel Room Booking project, that uses SQLite and Entity Framework Core.

#### Note to self:
There'S an odd bug, sometimes on opening the project, InitializeComponenet and all the xaml componenents referenced in MainWindow.cs will be not recognised.
This can be fixed by switching the MainWindow.xaml Build Action from Page to C# Compiler and back again.
Maybe also the .cs file too
I've no idea why this happens or why the fix works, it seems to be a WPF thing.
The fix works well enough for my purposes here.
