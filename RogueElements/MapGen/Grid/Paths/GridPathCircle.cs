using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class GridPathCircle<T> : GridPathStartStepGeneric<T>
        where T : class, IRoomGridGenContext
    {

        public RandRange CircleRoomRatio;

        public RandRange Paths;
        
        public GridPathCircle()
            : base()
        {

        }

        public override void ApplyToPath(IRandom rand, GridPlan floorPlan)
        {
            if (floorPlan.GridWidth < 3 || floorPlan.GridHeight < 3)
                throw new InvalidOperationException("Not enough room to create path.");

            int maxRooms = 2 * floorPlan.GridWidth + 2 * floorPlan.GridHeight - 4;
            int roomOpen = maxRooms * CircleRoomRatio.Pick(rand) / 100;
            int paths = Paths.Pick(rand);
            if (roomOpen < 1 && paths < 1)
                roomOpen = 1;

            GenContextDebug.StepIn("Outer Circle");

            //draw the top and bottom
            for (int xx = 0; xx < floorPlan.GridWidth; xx++)
            {
                RollOpenRoom(rand, floorPlan, new Loc(xx, 0), ref roomOpen, ref maxRooms);
                GenContextDebug.DebugProgress("Room");
                RollOpenRoom(rand, floorPlan, new Loc(xx, floorPlan.GridHeight - 1), ref roomOpen, ref maxRooms);
                GenContextDebug.DebugProgress("Room");
                if (xx > 0)
                {
                    floorPlan.SetConnectingHall(new Loc(xx - 1, 0), new Loc(xx, 0), GenericHalls.Pick(rand));
                    GenContextDebug.DebugProgress("Hall");
                    floorPlan.SetConnectingHall(new Loc(xx - 1, floorPlan.GridHeight - 1), new Loc(xx, floorPlan.GridHeight - 1), GenericHalls.Pick(rand));
                    GenContextDebug.DebugProgress("Hall");
                }
            }

            //draw the left and right (excluding the top and bottom)
            for (int yy = 0; yy < floorPlan.GridHeight; yy++)
            {
                //exclude the top and bottom
                if (yy > 0 && yy < floorPlan.GridHeight - 1)
                {
                    RollOpenRoom(rand, floorPlan, new Loc(0, yy), ref roomOpen, ref maxRooms);
                    GenContextDebug.DebugProgress("Room");
                    RollOpenRoom(rand, floorPlan, new Loc(floorPlan.GridWidth - 1, yy), ref roomOpen, ref maxRooms);
                    GenContextDebug.DebugProgress("Room");
                }
                if (yy > 0)
                {
                    floorPlan.SetConnectingHall(new Loc(0, yy - 1), new Loc(0, yy), GenericHalls.Pick(rand));
                    GenContextDebug.DebugProgress("Hall");
                    floorPlan.SetConnectingHall(new Loc(floorPlan.GridWidth - 1, yy - 1), new Loc(floorPlan.GridWidth - 1, yy), GenericHalls.Pick(rand));
                    GenContextDebug.DebugProgress("Hall");
                }
            }

            GenContextDebug.StepOut();

            GenContextDebug.StepIn("Inner Paths");

            Rect innerRect = new Rect(1, 1, floorPlan.GridWidth - 2, floorPlan.GridHeight - 2);
            //create inner paths
            for (int pathsMade = 0; pathsMade < paths; pathsMade++)
            {
                GenContextDebug.StepIn(String.Format("Path {0}", pathsMade));

                Dir4 startDir = (Dir4)rand.Next(4);
                int x = rand.Next(innerRect.Start.X, innerRect.End.X);
                int y = rand.Next(innerRect.Start.Y, innerRect.End.Y);
                switch (startDir)
                {
                    case Dir4.Down:
                        y = 0;
                        break;
                    case Dir4.Left:
                        x = floorPlan.GridWidth - 1;
                        break;
                    case Dir4.Up:
                        y = floorPlan.GridHeight - 1;
                        break;
                    case Dir4.Right:
                        x = 0;
                        break;
                }
                Loc wanderer = new Loc(x, y);
                Dir4 prevDir = Dir4.None; // direction of movement
                int pathLength = (startDir.ToAxis() == Axis4.Vert) ? innerRect.Height : innerRect.Width;
                for (int currentLength = 0; currentLength < pathLength; currentLength++)
                {
                    Dir4 chosenDir = startDir;
                    //avoid this block the first time
                    if (currentLength > 0)
                    {
                        List<Dir4> dirs = new List<Dir4>();
                        for (int dd = 0; dd < DirExt.VALID_DIR4.Length; dd++)
                        {
                            Dir4 dir = (Dir4)dd;
                            //do not backtrack
                            if (dir == prevDir)
                                continue;
                            //do not hit edge
                            if (!Collision.InBounds(innerRect, wanderer + dir.GetLoc()))
                                continue;
                            dirs.Add(dir);
                        }
                        chosenDir = dirs[rand.Next(dirs.Count)];
                    }
                    Loc dest = wanderer + chosenDir.GetLoc();

                    GridRoomPlan existingRoom = floorPlan.GetRoomPlan(dest);
                    if (existingRoom == null)
                    {
                        if (currentLength == pathLength - 1)//only the end room can be a non-hall
                            floorPlan.AddRoom(dest, GenericRooms.Pick(rand));
                        else
                            floorPlan.AddRoom(dest, GetDefaultGen(), false, true);
                        GenContextDebug.DebugProgress("Room");
                    }
                    else if (existingRoom.CountsAsHall())
                    {
                        if (currentLength == pathLength - 1)//override the hall room
                        {
                            existingRoom.RoomGen = GenericRooms.Pick(rand).Copy();
                            existingRoom.PreferHall = false;
                        }
                    }
                    floorPlan.SetConnectingHall(wanderer, dest, GenericHalls.Pick(rand));
                    GenContextDebug.DebugProgress("Hall");

                    wanderer = dest;
                    prevDir = chosenDir.Reverse();
                }
                GenContextDebug.StepOut();
            }

            GenContextDebug.StepOut();
        }

        private void RollOpenRoom(IRandom rand, GridPlan floorPlan, Loc loc, ref int roomOpen, ref int maxRooms)
        {
            if (RollRatio(rand, ref roomOpen, ref maxRooms))
                floorPlan.AddRoom(loc, GenericRooms.Pick(rand));
            else
                floorPlan.AddRoom(loc, GetDefaultGen(), false, true);
        }
    }
}
