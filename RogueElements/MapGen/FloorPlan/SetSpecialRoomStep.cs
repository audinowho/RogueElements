// <copyright file="SetSpecialRoomStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Takes an existing floor plan and changes one of the rooms into the specified room type.
    /// The size of the room may change because of this, and thus may also require the addition of a supporting hallway.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class SetSpecialRoomStep<T> : FloorPlanStep<T>
        where T : class, IFloorPlanGenContext
    {
        public SetSpecialRoomStep()
        {
            this.Rooms = null;
            this.Halls = null;
            this.RoomComponents = new ComponentCollection();
            this.HallComponents = new ComponentCollection();
            this.Filters = new List<BaseRoomFilter>();
        }

        public SetSpecialRoomStep(IRandPicker<RoomGen<T>> rooms, IRandPicker<PermissiveRoomGen<T>> halls)
        {
            this.Rooms = rooms;
            this.Halls = halls;
            this.RoomComponents = new ComponentCollection();
            this.HallComponents = new ComponentCollection();
            this.Filters = new List<BaseRoomFilter>();
        }

        /// <summary>
        /// The room to place.  It can be chosen out of several possibilities, but only one room will be placed.
        /// </summary>
        public IRandPicker<RoomGen<T>> Rooms { get; set; }

        /// <summary>
        /// Determines which rooms are eligible to be turned into the new room type.
        /// </summary>
        public List<BaseRoomFilter> Filters { get; set; }

        /// <summary>
        /// Components that the newly added room will be labeled with.
        /// </summary>
        public ComponentCollection RoomComponents { get; set; }

        /// <summary>
        /// When changing a room to a new type, the new type may be smaller and require a supporting hallway.
        /// This variable determines the room types that can be used as the intermediate hall.
        /// </summary>
        public IRandPicker<PermissiveRoomGen<T>> Halls { get; set; }

        /// <summary>
        /// Components that the newly added halls will be labeled with.
        /// </summary>
        public ComponentCollection HallComponents { get; set; }

        public static Rect GetSupportRect(FloorPlan floorPlan, IRoomGen oldGen, IRoomGen newGen, Dir4 dir, List<RoomHallIndex> adjacentsInDir)
        {
            bool vertical = dir.ToAxis() == Axis4.Vert;
            Rect supportRect = newGen.Draw;
            supportRect.Start += dir.GetLoc() * supportRect.Size.GetScalar(dir.ToAxis());
            supportRect.SetScalar(dir, oldGen.Draw.GetScalar(dir));

            IntRange minMax = newGen.Draw.GetSide(dir.ToAxis());

            foreach (RoomHallIndex adj in adjacentsInDir)
            {
                IRoomGen adjGen = floorPlan.GetRoomHall(adj).RoomGen;
                IntRange adjMinMax = adjGen.Draw.GetSide(dir.ToAxis());
                minMax = new IntRange(Math.Min(adjMinMax.Min, minMax.Min), Math.Max(adjMinMax.Max, minMax.Max));
            }

            IntRange oldMinMax = oldGen.Draw.GetSide(dir.ToAxis());
            minMax = new IntRange(Math.Max(oldMinMax.Min, minMax.Min), Math.Min(oldMinMax.Max, minMax.Max));

            if (vertical)
            {
                supportRect.SetScalar(Dir4.Left, minMax.Min);
                supportRect.SetScalar(Dir4.Right, minMax.Max);
            }
            else
            {
                supportRect.SetScalar(Dir4.Up, minMax.Min);
                supportRect.SetScalar(Dir4.Down, minMax.Max);
            }

            return supportRect;
        }

        public override void ApplyToPath(IRandom rand, FloorPlan floorPlan)
        {
            // choose certain rooms in the list to be special rooms
            // special rooms are required; so make sure they don't overlap
            IRoomGen newGen = this.Rooms.Pick(rand).Copy();
            Loc size = newGen.ProposeSize(rand);
            newGen.PrepareSize(rand, size);
            int factor = floorPlan.DrawRect.Area / newGen.Draw.Area;

            // TODO: accept smaller rooms to replace
            // bulldozing the surrounding rooms to get the space
            var room_indices = new SpawnList<RoomHallIndex>();
            for (int ii = 0; ii < floorPlan.RoomCount; ii++)
            {
                FloorRoomPlan plan = floorPlan.GetRoomPlan(ii);
                if (!BaseRoomFilter.PassesAllFilters(floorPlan.GetRoomPlan(ii), this.Filters))
                    continue;
                if (plan.RoomGen.Draw.Width >= newGen.Draw.Width &&
                    plan.RoomGen.Draw.Height >= newGen.Draw.Height)
                    room_indices.Add(new RoomHallIndex(ii, false), ComputeRoomChance(factor, plan.RoomGen.Draw, newGen.Draw));
            }

            for (int ii = 0; ii < floorPlan.HallCount; ii++)
            {
                var roomHall = new RoomHallIndex(ii, true);
                IFloorRoomPlan plan = floorPlan.GetRoomHall(roomHall);
                if (!BaseRoomFilter.PassesAllFilters(plan, this.Filters))
                    continue;
                if (plan.RoomGen.Draw.Width >= newGen.Draw.Width &&
                    plan.RoomGen.Draw.Height >= newGen.Draw.Height)
                    room_indices.Add(roomHall, ComputeRoomChance(factor, plan.RoomGen.Draw, newGen.Draw));
            }

            while (room_indices.Count > 0)
            {
                int ind = room_indices.PickIndex(rand);
                RoomHallIndex oldRoomHall = room_indices.GetSpawn(ind);
                Dictionary<Dir4, List<RoomHallIndex>> adjacentIndicesByDir = GetDirectionAdjacents(floorPlan, oldRoomHall);
                var adjacentsByDir = new Dictionary<Dir4, List<IRoomGen>>();
                foreach (Dir4 dir in DirExt.VALID_DIR4)
                {
                    adjacentsByDir[dir] = new List<IRoomGen>();
                    foreach (RoomHallIndex adj in adjacentIndicesByDir[dir])
                        adjacentsByDir[dir].Add(floorPlan.GetRoomHall(adj).RoomGen);
                }

                Loc placement = this.FindPlacement(rand, adjacentsByDir, newGen, floorPlan.GetRoomHall(oldRoomHall).RoomGen);
                if (placement != new Loc(-1))
                {
                    newGen.SetLoc(placement);
                    this.PlaceRoom(rand, floorPlan, newGen, oldRoomHall);
                    GenContextDebug.DebugProgress("Set Special Room");
                    return;
                }

                room_indices.RemoveAt(ind);
            }
        }

        public void PlaceRoom(IRandom rand, FloorPlan floorPlan, IRoomGen newGen, RoomHallIndex oldRoomHall)
        {
            // first get the adjacents of the removed room
            Dictionary<Dir4, List<RoomHallIndex>> adjacentsByDir = GetDirectionAdjacents(floorPlan, oldRoomHall);
            IRoomPlan oldPlan = floorPlan.GetRoomHall(oldRoomHall);

            // remove the room; update the adjacents too
            floorPlan.EraseRoomHall(oldRoomHall);
            foreach (Dir4 dir in DirExt.VALID_DIR4)
            {
                for (int jj = 0; jj < adjacentsByDir[dir].Count; jj++)
                {
                    RoomHallIndex adjRoomHall = adjacentsByDir[dir][jj];
                    if (adjRoomHall.IsHall == oldRoomHall.IsHall &&
                        adjRoomHall.Index > oldRoomHall.Index)
                        adjacentsByDir[dir][jj] = new RoomHallIndex(adjRoomHall.Index - 1, adjRoomHall.IsHall);
                }
            }

            var newAdjacents = new List<RoomHallIndex>();
            var supportHalls = new Dictionary<Dir4, IPermissiveRoomGen>();
            foreach (Dir4 dir in DirExt.VALID_DIR4)
            {
                if (newGen.Draw.GetScalar(dir) == oldPlan.RoomGen.Draw.GetScalar(dir))
                {
                    newAdjacents.AddRange(adjacentsByDir[dir]);
                }
                else if (adjacentsByDir[dir].Count > 0)
                {
                    Rect supportRect = GetSupportRect(floorPlan, oldPlan.RoomGen, newGen, dir, adjacentsByDir[dir]);
                    var supportHall = (IPermissiveRoomGen)this.Halls.Pick(rand).Copy();
                    supportHall.PrepareSize(rand, supportRect.Size);
                    supportHall.SetLoc(supportRect.Start);
                    supportHalls[dir] = supportHall;
                }
            }

            // add the new room
            var newRoomInd = new RoomHallIndex(floorPlan.RoomCount, false);
            ComponentCollection newCollection = oldPlan.Components.Clone();
            foreach (RoomComponent component in this.RoomComponents)
                newCollection.Set(component.Clone());
            floorPlan.AddRoom(newGen, newCollection, newAdjacents.ToArray());

            // add supporting halls
            foreach (Dir4 dir in DirExt.VALID_DIR4)
            {
                if (supportHalls.ContainsKey(dir))
                {
                    // include an attachment to the newly added room
                    List<RoomHallIndex> adjToAdd = new List<RoomHallIndex> { newRoomInd };
                    adjToAdd.AddRange(adjacentsByDir[dir]);
                    ComponentCollection newHallCollection = oldPlan.Components.Clone();
                    foreach (RoomComponent component in this.HallComponents)
                        newHallCollection.Set(component.Clone());
                    floorPlan.AddHall(supportHalls[dir], newHallCollection.Clone(), adjToAdd.ToArray());
                }
            }
        }

        public Loc FindPlacement(IRandom rand, Dictionary<Dir4, List<IRoomGen>> adjacentsByDir, IRoomGen newGen, IRoomGen oldGen)
        {
            SpawnList<Loc> possiblePlacements = this.GetPossiblePlacements(adjacentsByDir, newGen, oldGen);
            return possiblePlacements.SpawnTotal == 0 ? new Loc(-1) : possiblePlacements.Pick(rand);
        }

        public SpawnList<Loc> GetPossiblePlacements(Dictionary<Dir4, List<IRoomGen>> adjacentsByDir, IRoomGen newGen, IRoomGen oldGen)
        {
            // test all positions around perimeter
            Rect oldRect = oldGen.Draw;
            SpawnList<Loc> possiblePlacements = new SpawnList<Loc>();

            // top and bottom
            for (int xx = oldRect.Left; xx < oldRect.Right - newGen.Draw.Width + 1; xx++)
            {
                Loc topLoc = new Loc(xx, oldRect.Top);
                int topMatch = this.GetAllBorderMatch(adjacentsByDir, newGen, oldGen, topLoc);
                if (topMatch > 0)
                    possiblePlacements.Add(topLoc, topMatch);

                Loc bottomLoc = new Loc(xx, oldRect.Bottom - newGen.Draw.Height);
                if (bottomLoc != topLoc)
                {
                    int bottomMatch = this.GetAllBorderMatch(adjacentsByDir, newGen, oldGen, bottomLoc);
                    if (bottomMatch > 0)
                        possiblePlacements.Add(bottomLoc, bottomMatch);
                }
            }

            // left and right
            for (int yy = oldRect.Top + 1; yy < oldRect.Bottom - newGen.Draw.Height; yy++)
            {
                Loc leftLoc = new Loc(oldRect.Left, yy);
                int leftMatch = this.GetAllBorderMatch(adjacentsByDir, newGen, oldGen, leftLoc);
                if (leftMatch > 0)
                    possiblePlacements.Add(leftLoc, leftMatch);

                Loc rightLoc = new Loc(oldRect.Right - newGen.Draw.Width, yy);
                if (rightLoc != leftLoc)
                {
                    int rightMatch = this.GetAllBorderMatch(adjacentsByDir, newGen, oldGen, rightLoc);
                    if (rightMatch > 0)
                        possiblePlacements.Add(rightLoc, rightMatch);
                }
            }

            if (possiblePlacements.SpawnTotal == 0)
            {
                if (oldRect.Width >= newGen.Draw.Width + 2 &&
                    oldRect.Height >= newGen.Draw.Height + 2)
                {
                    for (int xx = oldRect.Left + 1; xx < oldRect.Right - newGen.Draw.Width; xx++)
                    {
                        for (int yy = oldRect.Top + 1; yy < oldRect.Bottom - newGen.Draw.Height; yy++)
                        {
                            possiblePlacements.Add(new Loc(xx, yy), 1);
                        }
                    }
                }
            }

            return possiblePlacements;
        }

        public virtual int GetAllBorderMatch(Dictionary<Dir4, List<IRoomGen>> adjacentsByDir, IRoomGen newGen, IRoomGen oldGen, Loc loc)
        {
            // TODO: Currently fails the loc if a single adjacent in a given direction is not met.
            // Perhaps allow the halls to cover the un-met required adjacent
            int matchValue = newGen.Draw.Perimeter;
            if (loc.X == oldGen.Draw.Left)
            {
                matchValue = Math.Min(matchValue, this.GetSideBorderMatch(newGen, adjacentsByDir, loc, Dir4.Left, matchValue));
                if (matchValue == 0)
                    return 0;
            }

            if (loc.X == oldGen.Draw.Right - newGen.Draw.Width)
            {
                matchValue = Math.Min(matchValue, this.GetSideBorderMatch(newGen, adjacentsByDir, loc, Dir4.Right, matchValue));
                if (matchValue == 0)
                    return 0;
            }

            if (loc.Y == oldGen.Draw.Top)
            {
                matchValue = Math.Min(matchValue, this.GetSideBorderMatch(newGen, adjacentsByDir, loc, Dir4.Up, matchValue));
                if (matchValue == 0)
                    return 0;
            }

            if (loc.Y == oldGen.Draw.Bottom - newGen.Draw.Height)
            {
                matchValue = Math.Min(matchValue, this.GetSideBorderMatch(newGen, adjacentsByDir, loc, Dir4.Down, matchValue));
                if (matchValue == 0)
                    return 0;
            }

            return matchValue;
        }

        public virtual int GetSideBorderMatch(IRoomGen newGen, Dictionary<Dir4, List<IRoomGen>> adjacentsByDir, Loc loc, Dir4 dir, int matchValue)
        {
            foreach (IRoomGen adj in adjacentsByDir[dir])
                matchValue = Math.Min(matchValue, FloorPlan.GetBorderMatch(adj, newGen, loc, dir.Reverse()));
            return matchValue;
        }

        private static int ComputeRoomChance(int factor, Rect oldRect, Rect newRect)
        {
            return Math.Max(1, oldRect.Area * factor / newRect.Area);
        }

        private static Dictionary<Dir4, List<RoomHallIndex>> GetDirectionAdjacents(FloorPlan floorPlan, RoomHallIndex oldRoomHall)
        {
            var adjacentsByDir = new Dictionary<Dir4, List<RoomHallIndex>>();
            IFloorRoomPlan oldPlan = floorPlan.GetRoomHall(oldRoomHall);
            foreach (Dir4 dir in DirExt.VALID_DIR4)
                adjacentsByDir[dir] = new List<RoomHallIndex>();

            foreach (RoomHallIndex adjacent in oldPlan.Adjacents)
            {
                IFloorRoomPlan adjPlan = floorPlan.GetRoomHall(adjacent);
                Dir4 dir = floorPlan.GetDirAdjacent(oldPlan.RoomGen, adjPlan.RoomGen);
                adjacentsByDir[dir].Add(adjacent);
            }

            return adjacentsByDir;
        }
    }
}
