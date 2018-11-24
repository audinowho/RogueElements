using System.Collections.Generic;
using System;

namespace RogueElements
{
    /// <summary>
    /// Contains data about which cells a room occupies in a GridFloorPlan.
    /// </summary>
    public class GridRoomPlan
    {
        public Rect Bounds;
        public bool Immutable;
        public bool PreferHall;
        public IRoomGen RoomGen;

        public GridRoomPlan(Rect bounds, IRoomGen roomGen)
        {
            Bounds = bounds;
            RoomGen = roomGen;
        }

        public bool CountsAsHall()
        {
            if (!PreferHall)
                return false;
            return RoomGen is IPermissiveRoomGen;
        }
    }
    
}
