using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class ConnectTerminalStep<T> : FloorPlanStep<T> where T : class, IFloorPlanGenContext
    {
        public IRandPicker<PermissiveRoomGen<T>> GenericHalls;
        public int ConnectPercent;

        public ConnectTerminalStep()
        {
            GenericHalls = new SpawnList<PermissiveRoomGen<T>>();
        }

        public ConnectTerminalStep(int connectPercent)
            : this()
        {
            ConnectPercent = connectPercent;
        }

        public override void ApplyToPath(IRandom rand, FloorPlan floorPlan)
        {
            List<List<RoomHallIndex>> candBranchPoints = GetBranchArms(floorPlan);

            //compute a goal amount of terminals to connect
            //this computation ignores the fact that some terminals may be impossible
            RandBinomial randBin = new RandBinomial(candBranchPoints.Count, ConnectPercent);
            int connectionsLeft = randBin.Pick(rand);
            
            while (candBranchPoints.Count > 0 && connectionsLeft > 0)
            {
                //choose random point to connect from
                int randIndex = rand.Next(candBranchPoints.Count);
                ListPathTraversalNode chosenDest = chooseConnection(rand, floorPlan, candBranchPoints[randIndex]);
                
                if (chosenDest != null)
                {
                    //connect
                    PermissiveRoomGen<T> hall = GenericHalls.Pick(rand).Copy();
                    hall.PrepareSize(rand, chosenDest.Connector.Size);
                    hall.SetLoc(chosenDest.Connector.Start);
                    floorPlan.AddHall(hall, chosenDest.From, chosenDest.To);
                    candBranchPoints.RemoveAt(randIndex);
                    connectionsLeft--;

                    //check to see if connection destination was also a candidate,
                    //counting this as a double if so
                    for (int ii = candBranchPoints.Count - 1; ii >= 0; ii--)
                    {
                        for (int jj = 0; jj < candBranchPoints[ii].Count; jj++)
                        {
                            if (candBranchPoints[ii][jj] == chosenDest.To)
                            {
                                candBranchPoints.RemoveAt(ii);
                                connectionsLeft--;
                                break;
                            }
                        }
                    }
                }
                else //remove the list anyway, but don't call it a success
                    candBranchPoints.RemoveAt(randIndex);
            }

        }

        public List<List<RoomHallIndex>> GetBranchArms(FloorPlan floorPlan)
        {
            List<ListPathTraversalNode> endBranches = new List<ListPathTraversalNode>();
            for (int ii = 0; ii < floorPlan.RoomCount; ii++)
            {
                FloorRoomPlan roomPlan = floorPlan.GetRoomPlan(ii);
                if (roomPlan.Adjacents.Count == 1)
                    endBranches.Add(new ListPathTraversalNode(new RoomHallIndex(-1, false), new RoomHallIndex(ii, false)));
            }

            List<List<RoomHallIndex>> branchArms = new List<List<RoomHallIndex>>();
            for (int nn = 0; nn < endBranches.Count; nn++)
            {
                ListPathTraversalNode chosenBranch = endBranches[nn];
                List<RoomHallIndex> arm = new List<RoomHallIndex>();

                while (chosenBranch != null)
                {
                    List<RoomHallIndex> connectors = new List<RoomHallIndex>();
                    List<RoomHallIndex> adjacents = floorPlan.GetRoomHall(chosenBranch.To).Adjacents;
                    foreach (RoomHallIndex dest in adjacents)
                    {
                        if (dest != chosenBranch.From)
                            connectors.Add(dest);
                    }

                    if (connectors.Count == 1)
                    {
                        arm.Add(chosenBranch.To);
                        chosenBranch = new ListPathTraversalNode(chosenBranch.To, connectors[0]);
                    }
                    else if (connectors.Count == 0)
                    {
                        //we've reached the other side of a single line; add
                        arm.Add(chosenBranch.To);
                        //but also find the other pending arm and remove it
                        for (int ii = endBranches.Count - 1; ii > nn; ii--)
                        {
                            ListPathTraversalNode otherBranch = endBranches[ii];
                            if (chosenBranch.To == otherBranch.To)
                                endBranches.RemoveAt(ii);
                        }
                        //end the loop
                        chosenBranch = null;
                    }
                    else
                        chosenBranch = null;
                }
                branchArms.Add(arm);
            }
            return branchArms;
        }

        protected ListPathTraversalNode chooseConnection(IRandom rand, FloorPlan floorPlan, List<RoomHallIndex> candList)
        {
            SpawnList<ListPathTraversalNode> expansions = GetPossibleExpansions(floorPlan, candList);

            if (expansions.Count > 0)
                return expansions.Pick(rand);
            else
                return null;
        }
        public SpawnList<ListPathTraversalNode> GetPossibleExpansions(FloorPlan floorPlan, List<RoomHallIndex> candList)
        {
            //get all probabilities.
            //the probability of an extension is the distance that the target room is from the start room, in rooms
            SpawnList<ListPathTraversalNode> expansions = new SpawnList<ListPathTraversalNode>();

            for(int nn = 0; nn < candList.Count; nn++)
            {
                //find the room to connect to
                //go through all sides of all rooms (randomly)
                RoomHallIndex chosenFrom = candList[nn];
                BaseFloorRoomPlan planFrom = floorPlan.GetRoomHall(chosenFrom);

                //exhausting all possible directions (randomly)
                List<Dir4> dirs = new List<Dir4>();
                for (int ii = 0; ii < DirExt.DIR4_COUNT; ii++)
                {
                    bool forbidExtend = false;
                    foreach (RoomHallIndex adjacent in planFrom.Adjacents)
                    {
                        Rect adjRect = floorPlan.GetRoomHall(adjacent).Gen.Draw;
                        if (planFrom.Gen.Draw.GetScalar((Dir4)ii) == adjRect.GetScalar(((Dir4)ii).Reverse()))
                        {
                            forbidExtend = true;
                            break;
                        }
                    }
                    if (!forbidExtend)
                    {
                        //find a rectangle to connect it with
                        ListPathTraversalNode expandTo = GetRoomToConnect(floorPlan, chosenFrom, (Dir4)ii);

                        if (expandTo != null)
                        {
                            int prb = floorPlan.GetDistance(expandTo.From, expandTo.To);
                            if (prb < 0)
                                expansions.Add(expandTo, 1);
                            else if (prb > 0)
                                expansions.Add(expandTo, prb);
                        }
                    }
                }

            }

            return expansions;
        }

        public ListPathTraversalNode GetRoomToConnect(FloorPlan floorPlan, RoomHallIndex chosenFrom, Dir4 dir)
        {
            //extend a rectangle to the border of the floor in the chosen direction
            bool vertical = dir.ToAxis() == Axis4.Vert;
            int dirSign = dir.GetLoc().GetScalar(dir.ToAxis());
            IRoomGen genFrom = floorPlan.GetRoomHall(chosenFrom).Gen;
            Rect sampleRect = genFrom.Draw;
            //expand from the start of that border direction to the borders of the floor
            sampleRect.Start = sampleRect.Start + dir.GetLoc() * sampleRect.Size.GetScalar(dir.ToAxis());
            //it doesn't have to be exactly the borders so just add the total size to be sure
            sampleRect.Expand(dir, vertical ? floorPlan.Size.Y : floorPlan.Size.X);
            
            //find the closest room.
            RoomHallIndex chosenTo = new RoomHallIndex(-1, false);
            foreach (RoomHallIndex collision in floorPlan.CheckCollision(sampleRect))
            {
                Rect collidedRect = floorPlan.GetRoomHall(collision).Gen.Draw;
                //limit the expansion by direction
                int sampleScalar = sampleRect.GetScalar(dir);
                int collidedScalar = collidedRect.GetScalar(dir.Reverse());
                bool limit = dirSign == Math.Sign(sampleScalar - collidedScalar);
                if (limit)
                {
                    //update the boundaries
                    sampleRect.SetScalar(dir, collidedScalar);
                    chosenTo = collision;
                }
            }

            //if it didn't collide with ANYTHING, then quit
            if (chosenTo.Index == -1)
                return null;

            IRoomGen genTo = floorPlan.GetRoomHall(chosenTo).Gen;

            //narrow the rectangle if touching something on the side
            //widen the rectangle by width
            Rect widthRect = sampleRect;
            widthRect.Inflate(vertical ? 1 : 0, vertical ? 0: 1);
            bool retractLeft = false;
            bool retractRight = false;
            Dir4 leftDir = DirExt.AddAngles(dir, Dir4.Left);
            Dir4 rightDir = DirExt.AddAngles(dir, Dir4.Right);
            foreach (RoomHallIndex collision in floorPlan.CheckCollision(widthRect))
            {
                Rect collidedRect = floorPlan.GetRoomHall(collision).Gen.Draw;
                if (!retractLeft)
                {
                    if (collidedRect.GetScalar(leftDir.Reverse()) == sampleRect.GetScalar(leftDir))
                        retractLeft = true;
                }
                if (!retractRight)
                {
                    if (collidedRect.GetScalar(rightDir.Reverse()) == sampleRect.GetScalar(rightDir))
                        retractRight = true;
                }
            }

            //retract the rectangle
            if (retractLeft)
                sampleRect.Expand(leftDir, -1);
            if (retractRight)
                sampleRect.Expand(rightDir, -1);

            //if the rectangle has been retracted too much, we can't go on
            if (sampleRect.Area <= 0)
                return null;

            //check for border availability between start and end
            bool foundFrom = HasBorderOpening(genFrom, sampleRect, dir);
            bool foundTo = HasBorderOpening(genTo, sampleRect, dir.Reverse());

            //return the expansion if one is found
            if (foundFrom && foundTo)
                return new ListPathTraversalNode(chosenFrom, chosenTo, sampleRect);
            else
                return null;
        }


        public bool HasBorderOpening(IRoomGen roomFrom, Rect rectTo, Dir4 expandTo)
        {
            Loc diff = roomFrom.Draw.Start - rectTo.Start;//how far ahead the start of source is to dest
            int offset = diff.GetScalar(expandTo.ToAxis().Orth());

            //Traverse the region that both borders touch
            int sourceLength = roomFrom.GetBorderLength(expandTo);
            int destLength = rectTo.Size.GetScalar(expandTo.ToAxis().Orth());
            for (int ii = Math.Max(0, offset); ii - offset < sourceLength && ii < destLength; ii++)
            {
                bool sourceFulfill = roomFrom.GetFulfillableBorder(expandTo, ii - offset);
                if (sourceFulfill)
                    return true;
            }

            return false;
        }

    }


    public class ListPathTraversalNode
    {
        public RoomHallIndex From;
        public RoomHallIndex To;
        public Rect Connector;

        public ListPathTraversalNode(RoomHallIndex from, RoomHallIndex to)
        {
            From = from;
            To = to;
        }

        public ListPathTraversalNode(RoomHallIndex from, RoomHallIndex to, Rect connector)
        {
            From = from;
            To = to;
            Connector = connector;
        }
    }
}
