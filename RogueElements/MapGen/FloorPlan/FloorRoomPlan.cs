using System.Collections.Generic;
using System;

namespace RogueElements
{
    /// <summary>
    /// Contains data about which cells a room occupies in a GridFloorPlan.
    /// </summary>
    public class FloorRoomPlan : BaseFloorRoomPlan
    {
        public bool Immutable;
        public IRoomGen RoomGen;
        public override IRoomGen Gen { get { return RoomGen; } }

        public FloorRoomPlan(IRoomGen roomGen)
        {
            RoomGen = roomGen;
        }
        

    }

    public class FloorHallPlan : BaseFloorRoomPlan
    {
        public IPermissiveRoomGen RoomGen;
        public override IRoomGen Gen { get { return RoomGen; } }

        public FloorHallPlan(IPermissiveRoomGen roomGen)
        {
            RoomGen = roomGen;
        }
    }


    public abstract class BaseFloorRoomPlan
    {
        public abstract IRoomGen Gen { get; }
        public List<RoomHallIndex> Adjacents;

        public BaseFloorRoomPlan()
        {
            Adjacents = new List<RoomHallIndex>();
        }
    }

}
