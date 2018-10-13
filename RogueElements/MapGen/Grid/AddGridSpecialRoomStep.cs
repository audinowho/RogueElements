using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class AddGridSpecialRoomStep<T> : GridPlanStep<T> where T : class, IRoomGridGenContext
    {
        public IRandPicker<RoomGen<T>> Rooms;

        public AddGridSpecialRoomStep()
            : base()
        { }

        public override void ApplyToPath(IRandom rand, GridPlan floorPlan)
        {
            //choose certain rooms in the list to be special rooms
            //special rooms are required; so make sure they don't overlap
            List<int> room_indices = new List<int>();
            for (int ii = 0; ii < floorPlan.RoomCount; ii++)
            {
                GridRoomPlan plan = floorPlan.GetRoomPlan(ii);
                if (!plan.Immutable && !(plan.RoomGen is RoomGenDefault<T>))
                    room_indices.Add(ii);
            }
            if (room_indices.Count > 0)
            {
                int ind = rand.Next(room_indices.Count);
                floorPlan.SetRoomGen(room_indices[ind], Rooms.Pick(rand));
                floorPlan.SetRoomImmutable(room_indices[ind], true);
                room_indices.RemoveAt(ind);
            }
        }

        
    }
}
