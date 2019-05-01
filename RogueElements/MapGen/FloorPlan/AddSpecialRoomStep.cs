using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class AddSpecialRoomStep<T> : FloorPlanStep<T> where T : class, IFloorPlanGenContext
    {
        public IRandPicker<RoomGen<T>> Rooms;
        public IRandPicker<PermissiveRoomGen<T>> Halls;

        public AddSpecialRoomStep()
            : base()
        { }

        public override void ApplyToPath(IRandom rand, FloorPlan floorPlan)
        {
            //choose certain rooms in the list to be special rooms
            //special rooms are required; so make sure they don't overlap
            IRoomGen newGen = Rooms.Pick(rand).Copy();
            Loc size = newGen.ProposeSize(rand);
            newGen.PrepareSize(rand, size);
            int factor = floorPlan.DrawRect.Area / newGen.Draw.Area;

            //TODO: accept smaller rooms to replace
            //bulldozing the surrounding rooms to get the space
            SpawnList<RoomHallIndex> room_indices = new SpawnList<RoomHallIndex>();
            for (int ii = 0; ii < floorPlan.RoomCount; ii++)
            {
                FloorRoomPlan plan = floorPlan.GetRoomPlan(ii);
                if (!plan.Immutable &&
                    plan.Gen.Draw.Width >= newGen.Draw.Width &&
                    plan.Gen.Draw.Height >= newGen.Draw.Height)
                    room_indices.Add(new RoomHallIndex(ii, false), computeRoomChance(factor, plan.Gen.Draw, newGen.Draw));
            }
            for (int ii = 0; ii < floorPlan.HallCount; ii++)
            {
                RoomHallIndex roomHall = new RoomHallIndex(ii, true);
                BaseFloorRoomPlan plan = floorPlan.GetRoomHall(roomHall);
                if (plan.Gen.Draw.Width >= newGen.Draw.Width &&
                    plan.Gen.Draw.Height >= newGen.Draw.Height)
                    room_indices.Add(roomHall, computeRoomChance(factor, plan.Gen.Draw, newGen.Draw));
            }

            while (room_indices.Count > 0)
            {
                int ind = room_indices.PickIndex(rand);
                RoomHallIndex oldRoomHall = room_indices.GetSpawn(ind);
                Dictionary<Dir4, List<RoomHallIndex>> adjacentIndicesByDir = getDirectionAdjacents(floorPlan, oldRoomHall);
                var adjacentsByDir = new Dictionary<Dir4, List<IRoomGen>>();
                foreach(Dir4 dir in DirExt.VALID_DIR4)
                {
                    adjacentsByDir[dir] = new List<IRoomGen>();
                    foreach (RoomHallIndex adj in adjacentIndicesByDir[dir])
                        adjacentsByDir[dir].Add(floorPlan.GetRoomHall(adj).Gen);
                }
                Loc placement = FindPlacement(rand, adjacentsByDir, newGen, floorPlan.GetRoomHall(oldRoomHall).Gen);
                if (placement != new Loc(-1))
                {
                    newGen.SetLoc(placement);
                    PlaceRoom(rand, floorPlan, newGen, oldRoomHall);
                    GenContextDebug.DebugProgress("Set Special Room");
                    return;
                }
                room_indices.RemoveAt(ind);
            }
        }

        public void PlaceRoom(IRandom rand, FloorPlan floorPlan, IRoomGen newGen, RoomHallIndex oldRoomHall)
        {
            //first get the adjacents of the removed room
            Dictionary<Dir4, List<RoomHallIndex>> adjacentsByDir = getDirectionAdjacents(floorPlan, oldRoomHall);
            IRoomGen oldGen = floorPlan.GetRoomHall(oldRoomHall).Gen;
            //remove the room; update the adjacents too
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
                if (newGen.Draw.GetScalar(dir) == oldGen.Draw.GetScalar(dir))
                    newAdjacents.AddRange(adjacentsByDir[dir]);
                else if (adjacentsByDir[dir].Count > 0)
                {
                    Rect supportRect = GetSupportRect(floorPlan, oldGen, newGen, dir, adjacentsByDir[dir]);
                    IPermissiveRoomGen supportHall = (IPermissiveRoomGen)Halls.Pick(rand).Copy();
                    supportHall.PrepareSize(rand, supportRect.Size);
                    supportHall.SetLoc(supportRect.Start);
                    supportHalls[dir] = supportHall;
                }
            }
            //add the new room
            RoomHallIndex newRoomInd = new RoomHallIndex(floorPlan.RoomCount, false);
            floorPlan.AddRoom(newGen, true, newAdjacents.ToArray());
            //add supporting halls
            foreach (Dir4 dir in DirExt.VALID_DIR4)
            {
                if (supportHalls.ContainsKey(dir))
                {
                    //include an attachment to the newly added room
                    List<RoomHallIndex> adjToAdd = new List<RoomHallIndex>();
                    adjToAdd.Add(newRoomInd);
                    adjToAdd.AddRange(adjacentsByDir[dir]);
                    floorPlan.AddHall(supportHalls[dir], adjToAdd.ToArray());
                }
            }
        }


        public Rect GetSupportRect(FloorPlan floorPlan, IRoomGen oldGen, IRoomGen newGen, Dir4 dir, List<RoomHallIndex> adjacentsInDir)
        {
            bool vertical = dir.ToAxis() == Axis4.Vert;
            Rect supportRect = newGen.Draw;
            supportRect.Start = supportRect.Start + dir.GetLoc() * supportRect.Size.GetScalar(dir.ToAxis());
            supportRect.SetScalar(dir, oldGen.Draw.GetScalar(dir));

            Range minMax = newGen.Draw.GetSide(dir.ToAxis());

            foreach (RoomHallIndex adj in adjacentsInDir)
            {
                IRoomGen adjGen = floorPlan.GetRoomHall(adj).Gen;
                Range adjMinMax = adjGen.Draw.GetSide(dir.ToAxis());
                minMax = new Range(Math.Min(adjMinMax.Min, minMax.Min), Math.Max(adjMinMax.Max, minMax.Max));
            }

            Range oldMinMax = oldGen.Draw.GetSide(dir.ToAxis());
            minMax = new Range(Math.Max(oldMinMax.Min, minMax.Min), Math.Min(oldMinMax.Max, minMax.Max));

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

        public Loc FindPlacement(IRandom rand, Dictionary<Dir4, List<IRoomGen>> adjacentsByDir, IRoomGen newGen, IRoomGen oldGen)
        {
            SpawnList<Loc> possiblePlacements = GetPossiblePlacements(adjacentsByDir, newGen, oldGen);
            if (possiblePlacements.SpawnTotal == 0)
                return new Loc(-1);
            else
                return possiblePlacements.Pick(rand);
        }

        public SpawnList<Loc> GetPossiblePlacements(Dictionary<Dir4, List<IRoomGen>> adjacentsByDir, IRoomGen newGen, IRoomGen oldGen)
        {
            //test all positions around perimeter
            Rect oldRect = oldGen.Draw;
            SpawnList<Loc> possiblePlacements = new SpawnList<Loc>();
            //top and bottom
            for (int xx = oldRect.Left; xx < oldRect.Right - newGen.Draw.Width + 1; xx++)
            {
                Loc topLoc = new Loc(xx, oldRect.Top);
                int topMatch = GetAllBorderMatch(adjacentsByDir, newGen, oldGen, topLoc);
                if (topMatch > 0)
                    possiblePlacements.Add(topLoc, topMatch);

                Loc bottomLoc = new Loc(xx, oldRect.Bottom - newGen.Draw.Height);
                if (bottomLoc != topLoc)
                {
                    int bottomMatch = GetAllBorderMatch(adjacentsByDir, newGen, oldGen, bottomLoc);
                    if (bottomMatch > 0)
                        possiblePlacements.Add(bottomLoc, bottomMatch);
                }
            }
            //left and right
            for (int yy = oldRect.Top + 1; yy < oldRect.Bottom - newGen.Draw.Height; yy++)
            {
                Loc leftLoc = new Loc(oldRect.Left, yy);
                int leftMatch = GetAllBorderMatch(adjacentsByDir, newGen, oldGen, leftLoc);
                if (leftMatch > 0)
                    possiblePlacements.Add(leftLoc, leftMatch);

                Loc rightLoc = new Loc(oldRect.Right - newGen.Draw.Width, yy);
                if (rightLoc != leftLoc)
                {
                    int rightMatch = GetAllBorderMatch(adjacentsByDir, newGen, oldGen, rightLoc);
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
            //TODO: Currently fails the loc if a single adjacent in a given direction is not met.
            //Perhaps allow the halls to cover the un-met required adjacent
            int matchValue = newGen.Draw.Perimeter;
            if (loc.X == oldGen.Draw.Left)
            {
                matchValue = Math.Min(matchValue, GetSideBorderMatch(newGen, adjacentsByDir, loc, Dir4.Left, matchValue));
                if (matchValue == 0)
                    return 0;
            }
            if (loc.X == oldGen.Draw.Right - newGen.Draw.Width)
            {
                matchValue = Math.Min(matchValue, GetSideBorderMatch(newGen, adjacentsByDir, loc, Dir4.Right, matchValue));
                if (matchValue == 0)
                    return 0;
            }
            if (loc.Y == oldGen.Draw.Top)
            {
                matchValue = Math.Min(matchValue, GetSideBorderMatch(newGen, adjacentsByDir, loc, Dir4.Up, matchValue));
                if (matchValue == 0)
                    return 0;
            }
            if (loc.Y == oldGen.Draw.Bottom - newGen.Draw.Height)
            {
                matchValue = Math.Min(matchValue, GetSideBorderMatch(newGen, adjacentsByDir, loc, Dir4.Down, matchValue));
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

        private int computeRoomChance(int factor, Rect oldRect, Rect newRect)
        {
            return Math.Max(1, oldRect.Area * factor / newRect.Area);
        }

        private Dictionary<Dir4, List<RoomHallIndex>> getDirectionAdjacents(FloorPlan floorPlan, RoomHallIndex oldRoomHall)
        {
            var adjacentsByDir = new Dictionary<Dir4, List<RoomHallIndex>>();
            BaseFloorRoomPlan oldPlan = floorPlan.GetRoomHall(oldRoomHall);
            foreach (Dir4 dir in DirExt.VALID_DIR4)
            {
                adjacentsByDir[dir] = new List<RoomHallIndex>();
                foreach (RoomHallIndex adjacent in oldPlan.Adjacents)
                {
                    BaseFloorRoomPlan adjPlan = floorPlan.GetRoomHall(adjacent);
                    if (oldPlan.Gen.Draw.GetScalar(dir) ==
                        adjPlan.Gen.Draw.GetScalar(dir.Reverse()))
                    {
                        adjacentsByDir[dir].Add(adjacent);
                    }
                }
            }
            return adjacentsByDir;
        }
    }
}
