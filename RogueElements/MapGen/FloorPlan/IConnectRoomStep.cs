// <copyright file="IConnectRoomStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    public interface IConnectRoomStep
    {
        RandRange ConnectFactor { get; set; }
    }

    /// <summary>
    /// Takes the current floor plan and connects its rooms with other rooms.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class ConnectRoomStep<T> : ConnectStep<T>, IConnectRoomStep
        where T : class, IFloorPlanGenContext
    {
        public ConnectRoomStep()
            : base()
        {
        }

        public ConnectRoomStep(IRandPicker<PermissiveRoomGen<T>> genericHalls)
            : base(genericHalls)
        {
        }

        /// <summary>
        /// Determines the number of connections to make.
        /// 0 = No Connections
        /// 50 = Half of all rooms connected
        /// 100 = All rooms connected
        /// 200 = All rooms connected twice over
        /// </summary>
        public RandRange ConnectFactor { get; set; }

        public override void ApplyToPath(IRandom rand, FloorPlan floorPlan)
        {
            List<RoomHallIndex> candBranchPoints = new List<RoomHallIndex>();
            for (int ii = 0; ii < floorPlan.RoomCount; ii++)
            {
                if (!BaseRoomFilter.PassesAllFilters(floorPlan.GetRoomPlan(ii), this.Filters))
                    continue;
                candBranchPoints.Add(new RoomHallIndex(ii, false));
            }

            // compute a goal amount of terminals to connect
            // this computation ignores the fact that some terminals may be impossible
            int connectionsLeft = this.ConnectFactor.Pick(rand) * candBranchPoints.Count / 2 / 100;

            while (candBranchPoints.Count > 0 && connectionsLeft > 0)
            {
                // choose random point to connect from
                int randIndex = rand.Next(candBranchPoints.Count);
                var chosenDestResult = ChooseConnection(rand, floorPlan, candBranchPoints);

                if (chosenDestResult is ListPathTraversalNode chosenDest)
                {
                    // connect
                    PermissiveRoomGen<T> hall = (PermissiveRoomGen<T>)this.GenericHalls.Pick(rand).Copy();
                    hall.PrepareSize(rand, chosenDest.Connector.Size);
                    hall.SetLoc(chosenDest.Connector.Start);
                    floorPlan.AddHall(hall, this.Components.Clone(), chosenDest.From, chosenDest.To);
                    candBranchPoints.RemoveAt(randIndex);
                    connectionsLeft--;
                    GenContextDebug.DebugProgress("Added Connection");

                    // check to see if connection destination was also a candidate,
                    // counting this as a double if so
                    for (int jj = 0; jj < candBranchPoints.Count; jj++)
                    {
                        if (candBranchPoints[jj] == chosenDest.To)
                        {
                            candBranchPoints.RemoveAt(jj);
                            connectionsLeft--;
                            break;
                        }
                    }
                }
                else
                {
                    // remove the list anyway, but don't call it a success
                    candBranchPoints.RemoveAt(randIndex);
                }
            }
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}%", this.GetType().Name, this.ConnectFactor);
        }
    }
}
