// <copyright file="IGridPathCircle.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

namespace RogueElements
{
    public interface IGridPathCircle
    {
        RandRange CircleRoomRatio { get; set; }

        RandRange Paths { get; set; }
    }

    /// <summary>
    /// Populates the empty grid plan of a map by creating a ring of rooms and halls at the outer cells of the grid.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class GridPathCircle<T> : GridPathStartStepGeneric<T>, IGridPathCircle
        where T : class, IRoomGridGenContext
    {
        public GridPathCircle()
            : base()
        {
        }

        /// <summary>
        /// The percentage of rooms in the outer circle that are NOT treated as 1-tile halls.
        /// </summary>
        public RandRange CircleRoomRatio { get; set; }

        /// <summary>
        /// The number of paths going to the inner circle.
        /// </summary>
        public RandRange Paths { get; set; }

        public override void ApplyToPath(IRandom rand, GridPlan floorPlan)
        {
            if (floorPlan.GridWidth < 3 || floorPlan.GridHeight < 3)
                throw new InvalidOperationException("Not enough room to create path.");

            int maxRooms = (2 * floorPlan.GridWidth) + (2 * floorPlan.GridHeight) - 4;
            int roomOpen = maxRooms * this.CircleRoomRatio.Pick(rand) / 100;
            int paths = this.Paths.Pick(rand);
            if (roomOpen < 1 && paths < 1)
                roomOpen = 1;

            GenContextDebug.StepIn("Outer Circle");

            try
            {
                // draw the top and bottom
                for (int xx = 0; xx < floorPlan.GridWidth; xx++)
                {
                    this.RollOpenRoom(rand, floorPlan, new Loc(xx, 0), ref roomOpen, ref maxRooms);
                    GenContextDebug.DebugProgress("Room");
                    this.RollOpenRoom(rand, floorPlan, new Loc(xx, floorPlan.GridHeight - 1), ref roomOpen, ref maxRooms);
                    GenContextDebug.DebugProgress("Room");
                    if (xx > 0)
                    {
                        floorPlan.SetHall(new LocRay4(new Loc(xx, 0), Dir4.Left), this.GenericHalls.Pick(rand), this.HallComponents.Clone());
                        GenContextDebug.DebugProgress("Hall");
                        floorPlan.SetHall(new LocRay4(new Loc(xx, floorPlan.GridHeight - 1), Dir4.Left), this.GenericHalls.Pick(rand), this.HallComponents.Clone());
                        GenContextDebug.DebugProgress("Hall");
                    }
                }

                // draw the left and right (excluding the top and bottom)
                for (int yy = 0; yy < floorPlan.GridHeight; yy++)
                {
                    // exclude the top and bottom
                    if (yy > 0 && yy < floorPlan.GridHeight - 1)
                    {
                        this.RollOpenRoom(rand, floorPlan, new Loc(0, yy), ref roomOpen, ref maxRooms);
                        GenContextDebug.DebugProgress("Room");
                        this.RollOpenRoom(rand, floorPlan, new Loc(floorPlan.GridWidth - 1, yy), ref roomOpen, ref maxRooms);
                        GenContextDebug.DebugProgress("Room");
                    }

                    if (yy > 0)
                    {
                        floorPlan.SetHall(new LocRay4(new Loc(0, yy), Dir4.Up), this.GenericHalls.Pick(rand), this.HallComponents.Clone());
                        GenContextDebug.DebugProgress("Hall");
                        floorPlan.SetHall(new LocRay4(new Loc(floorPlan.GridWidth - 1, yy), Dir4.Up), this.GenericHalls.Pick(rand), this.HallComponents.Clone());
                        GenContextDebug.DebugProgress("Hall");
                    }
                }
            }
            catch (Exception ex)
            {
                GenContextDebug.DebugError(ex);
            }

            GenContextDebug.StepOut();

            GenContextDebug.StepIn("Inner Paths");

            try
            {
                Rect innerRect = new Rect(1, 1, floorPlan.GridWidth - 2, floorPlan.GridHeight - 2);

                // create inner paths
                for (int pathsMade = 0; pathsMade < paths; pathsMade++)
                {
                    GenContextDebug.StepIn($"Path {pathsMade}");

                    try
                    {
                        Dir4 startDir = DirExt.VALID_DIR4.ElementAt(rand.Next(DirExt.DIR4_COUNT));
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
                            case Dir4.None:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(startDir), "Invalid enum value.");
                        }

                        Loc wanderer = new Loc(x, y);
                        Dir4 prevDir = Dir4.None; // direction of movement
                        int pathLength = (startDir.ToAxis() == Axis4.Vert) ? innerRect.Height : innerRect.Width;
                        for (int currentLength = 0; currentLength < pathLength; currentLength++)
                        {
                            Dir4 chosenDir = startDir;

                            // avoid this block the first time
                            if (currentLength > 0)
                            {
                                List<Dir4> dirs = new List<Dir4>();
                                foreach (Dir4 dir in DirExt.VALID_DIR4)
                                {
                                    // do not backtrack
                                    if (dir == prevDir)
                                        continue;

                                    // do not hit edge
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
                                if (currentLength == pathLength - 1) // only the end room can be a non-hall
                                    floorPlan.AddRoom(dest, this.GenericRooms.Pick(rand), this.RoomComponents.Clone());
                                else
                                    floorPlan.AddRoom(dest, this.GetDefaultGen(), this.HallComponents.Clone(), true);
                                GenContextDebug.DebugProgress("Room");
                            }
                            else if (existingRoom.PreferHall)
                            {
                                if (currentLength == pathLength - 1)
                                {
                                    // override the hall room
                                    existingRoom.RoomGen = this.GenericRooms.Pick(rand).Copy();
                                    existingRoom.PreferHall = false;
                                }
                            }

                            floorPlan.SetHall(new LocRay4(wanderer, chosenDir), this.GenericHalls.Pick(rand), this.HallComponents.Clone());
                            GenContextDebug.DebugProgress("Hall");

                            wanderer = dest;
                            prevDir = chosenDir.Reverse();
                        }
                    }
                    catch (Exception ex)
                    {
                        GenContextDebug.DebugError(ex);
                    }

                    GenContextDebug.StepOut();
                }
            }
            catch (Exception ex)
            {
                GenContextDebug.DebugError(ex);
            }

            GenContextDebug.StepOut();
        }

        public override string ToString()
        {
            return string.Format("{0}: Fill:{1}% Paths:{2}%", this.GetType().Name, this.CircleRoomRatio, this.Paths);
        }

        private void RollOpenRoom(IRandom rand, GridPlan floorPlan, Loc loc, ref int roomOpen, ref int maxRooms)
        {
            if (RollRatio(rand, ref roomOpen, ref maxRooms))
                floorPlan.AddRoom(loc, this.GenericRooms.Pick(rand), this.RoomComponents.Clone());
            else
                floorPlan.AddRoom(loc, this.GetDefaultGen(), this.HallComponents.Clone(), true);
        }
    }
}
