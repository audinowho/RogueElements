using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class GridPathGrid<T> : GridPathStartStepGeneric<T>
        where T : class, IRoomGridGenContext
    {

        public int RoomRatio;
        public int HallRatio;

        public GridPathGrid()
            : base()
        {

        }

        public override void ApplyToPath(IRandom rand, GridPlan floorPlan)
        {
            if (floorPlan.GridWidth < 3 || floorPlan.GridHeight < 3)
                throw new InvalidOperationException("Not enough room to create path.");

            int roomMax = 2 * (floorPlan.GridWidth - 2) + 2 * (floorPlan.GridHeight - 2);
            int roomOpen = roomMax * RoomRatio / 100;
            if (roomOpen < 1)
                roomOpen = 1;

            //open random rooms on all sides
            for (int x = 1; x < floorPlan.GridWidth - 1; x++)
            {
                if (RollRatio(rand, ref roomOpen, ref roomMax))
                {
                    floorPlan.SetRoomGen(x, 0, GenericRooms.Pick(rand));
                    floorPlan.SetConnectingHall(new Loc(x, 0), new Loc(x, 1), GenericHalls.Pick(rand));
                }
                if (RollRatio(rand, ref roomOpen, ref roomMax))
                {
                    floorPlan.SetRoomGen(x,floorPlan.GridHeight - 1, GenericRooms.Pick(rand));
                    floorPlan.SetConnectingHall(new Loc(x, floorPlan.GridHeight - 1), new Loc(x, floorPlan.GridHeight - 2), GenericHalls.Pick(rand));
                }
            }
            for (int y = 1; y < floorPlan.GridHeight - 1; y++)
            {
                if (RollRatio(rand, ref roomOpen, ref roomMax))
                {
                    floorPlan.SetRoomGen(0, y, GenericRooms.Pick(rand));
                    floorPlan.SetConnectingHall(new Loc(0, y), new Loc(1, y), GenericHalls.Pick(rand));
                }
                if (RollRatio(rand, ref roomOpen, ref roomMax))
                {
                    floorPlan.SetRoomGen(floorPlan.GridWidth - 1, y, GenericRooms.Pick(rand));
                    floorPlan.SetConnectingHall(new Loc(floorPlan.GridWidth - 1, y), new Loc(floorPlan.GridWidth - 2, y), GenericHalls.Pick(rand));
                }
            }

            //set hallrooms in middle of grid and open hallways between them
            for (int x = 1; x < floorPlan.GridWidth - 1; x++)
            {
                for (int y = 1; y < floorPlan.GridHeight - 1; y++)
                {
                    floorPlan.SetRoomGen(x,y, GetDefaultGen());

                    if (x > 1)
                        floorPlan.SetConnectingHall(new Loc(x - 1, y), new Loc(x, y), GenericHalls.Pick(rand));
                    if (y > 1)
                        floorPlan.SetConnectingHall(new Loc(x, y - 1), new Loc(x, y), GenericHalls.Pick(rand));
                }
            }

            //get all halls eligible to be opened
            List<Loc> hHallSites = new List<Loc>();
            List<Loc> vHallSites = new List<Loc>();
            for (int x = 1; x < floorPlan.GridWidth; x++)
            {
                if (floorPlan.IsRoomOpen(x,0) || floorPlan.IsRoomOpen(x - 1,0))
                    hHallSites.Add(new Loc(x,0));
                if (floorPlan.IsRoomOpen(x,floorPlan.GridHeight - 1) || floorPlan.IsRoomOpen(x - 1,floorPlan.GridHeight - 1))
                    hHallSites.Add(new Loc(x, floorPlan.GridHeight - 1));
            }
            for (int y = 1; y < floorPlan.GridHeight; y++)
            {
                if (floorPlan.IsRoomOpen(0,y) || floorPlan.IsRoomOpen(0,y-1))
                    vHallSites.Add(new Loc(0, y));
                if (floorPlan.IsRoomOpen(floorPlan.GridWidth - 1,y) || floorPlan.IsRoomOpen(floorPlan.GridWidth - 1,y-1))
                    vHallSites.Add(new Loc(floorPlan.GridWidth - 1, y));
            }
            int halls = hHallSites.Count + vHallSites.Count;
            int placedHalls = halls * HallRatio / 100;
            
            //place the halls
            for (int ii = 0; ii < hHallSites.Count; ii++)
            {
                if (rand.Next() % halls < placedHalls)
                {
                    SafeAddHall(hHallSites[ii], hHallSites[ii] - new Loc(1, 0), floorPlan, GenericHalls.Pick(rand), GetDefaultGen());
                    placedHalls--;
                }
                halls--;
            }
            for (int ii = 0; ii < vHallSites.Count; ii++)
            {
                if (rand.Next() % halls < placedHalls)
                {
                    SafeAddHall(vHallSites[ii], vHallSites[ii] - new Loc(0, 1), floorPlan, GenericHalls.Pick(rand), GetDefaultGen());
                    placedHalls--;
                }
                halls--;
            }
        }
    }
}
