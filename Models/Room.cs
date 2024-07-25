using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Hotel.Models;

public class Room
{
    public int Id { get; set; }
    public string Number { get; set; }
    public string? Description { get; set; }
    public RoomTypeEnum RoomType { get; set; }
}

//NOTE: could add a room type enum here, for suite, single etc
public enum RoomTypeEnum
{
    Single = 0,
    Double = 1,
    Suite = 2
}
