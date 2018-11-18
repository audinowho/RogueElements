using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class ConnectBranchStep<T> : ConnectStep<T> where T : class, IFloorPlanGenContext
    {
        public int ConnectPercent;

        public ConnectBranchStep() : base() { }

        public ConnectBranchStep(int connectPercent)
            : this()
        {
            ConnectPercent = connectPercent;
        }

        public override void ApplyToPath(IRandom rand, FloorPlan floorPlan)
        {
            List<List<RoomHallIndex>> candBranchPoints = GetBranchArms(floorPlan);

            //compute a goal amount of branches to connect
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


    }


}
