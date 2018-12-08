using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class GridPathTwoSides<T> : GridPathStartStepGeneric<T>
        where T : class, IRoomGridGenContext
    {
        public bool Vertical;

        public GridPathTwoSides()
            : base()
        {

        }

        public override void ApplyToPath(IRandom rand, GridPlan floorPlan)
        {
            //open rooms on both sides
            int gapLength = Vertical ? floorPlan.GridHeight : floorPlan.GridWidth;
            int sideLength = Vertical ? floorPlan.GridWidth : floorPlan.GridHeight;

            if (gapLength < 2 || sideLength < 1)
                throw new InvalidOperationException("Not enough room to create path.");

            GenContextDebug.StepIn("Initial Rooms");

            for (int ii = 0; ii < sideLength; ii++)
            {
                //place the rooms at the edge
                int x = Vertical ? ii : 0;
                int y = Vertical ? 0 : ii;
                floorPlan.AddRoom(new Loc(x, y), GenericRooms.Pick(rand));
                GenContextDebug.DebugProgress("Room");
                x = Vertical ? ii : gapLength - 1;
                y = Vertical ? gapLength - 1 : ii;
                floorPlan.AddRoom(new Loc(x, y), GenericRooms.Pick(rand));
                GenContextDebug.DebugProgress("Room");

                if (gapLength > 2)
                {
                    //place hall rooms
                    x = Vertical ? ii : 1;
                    y = Vertical ? 1 : ii;
                    int w = Vertical ? 1 : gapLength - 2;
                    int h = Vertical ? gapLength - 2 : 1;
                    floorPlan.AddRoom(new Rect(x, y, w, h), GetDefaultGen(), false, true);
                    GenContextDebug.DebugProgress("Mid Room");
                }
            }
            GenContextDebug.StepOut();

            GenContextDebug.StepIn("Connecting Sides");

            //halls connecting two tiers of the same side
            bool[][] connections = new bool[sideLength - 1][];
            for(int ii = 0; ii < sideLength - 1; ii++)
                connections[ii] = new bool[2];

            //add crosses
            for (int ii = 0; ii < sideLength - 1; ii++)
            {
                if (rand.Next(2) == 0)
                    connections[ii][0] = true;
                else
                    connections[ii][1] = true;
            }
            
            //paint hallways
            for (int ii = 0; ii < sideLength; ii++)
            {
                //place the halls at the sides
                if (ii < sideLength - 1)
                {
                    if (connections[ii][0])
                    {
                        PlaceOrientedHall(Vertical, Dir4.Right, ii, 0, floorPlan, GenericHalls.Pick(rand));
                        GenContextDebug.DebugProgress("Side Connection");
                    }
                    if (connections[ii][1])
                    {
                        PlaceOrientedHall(Vertical, Dir4.Right, ii, gapLength - 1, floorPlan, GenericHalls.Pick(rand));
                        GenContextDebug.DebugProgress("Side Connection");
                    }
                    
                }
                
                //place halls to bridge the gap
                PlaceOrientedHall(Vertical, Dir4.Down, ii, 0, floorPlan, GenericHalls.Pick(rand));
                if (gapLength > 2)
                    PlaceOrientedHall(Vertical, Dir4.Up, ii, gapLength - 1, floorPlan, GenericHalls.Pick(rand));
                GenContextDebug.DebugProgress("Bridge");

            }
            GenContextDebug.StepOut();

        }

        public void PlaceOrientedHall(bool vertical, Dir4 dir, int sideWise, int gapWise, GridPlan floorPlan, PermissiveRoomGen<T> hallGen)
        {
            Loc loc = new Loc(vertical ? sideWise : gapWise, vertical ? gapWise : sideWise);
            Loc moveLoc = new Loc(sideWise, gapWise) + dir.GetLoc();
            Loc newLoc = new Loc(vertical ? moveLoc.X : moveLoc.Y, vertical ? moveLoc.Y : moveLoc.X);
            floorPlan.SetConnectingHall(loc, newLoc, hallGen);
        }
        

    }
}
