// <copyright file="IConnectRoomStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Defines the interface for steps that connect rooms in a floor plan.
    /// </summary>
    public interface IConnectRoomStep
    {
        /// <summary>
        /// Gets or sets the connection factor determining how many connections to make.
        /// </summary>
        RandRange ConnectFactor { get; set; }
    }

    /// <summary>
    /// Connects rooms in the floor plan based on a connection factor.
    /// </summary>
    /// <typeparam name="T">The generation context type, which must implement <see cref="IFloorPlanGenContext"/>.</typeparam>
    /// <remarks>
    /// <para>
    /// This step creates additional connections between rooms in the floor plan. Unlike
    /// <see cref="ConnectBranchStep{T}"/> which focuses on dead ends, this step can connect
    /// any eligible rooms based on the <see cref="ConnectFactor"/>.
    /// </para>
    /// <para>
    /// The connection factor is scaled: 100 means each room is connected once, 200 means
    /// each room is connected twice on average.
    /// </para>
    /// </remarks>
    /// <seealso cref="ConnectBranchStep{T}"/>
    [Serializable]
    public class ConnectRoomStep<T> : ConnectStep<T>, IConnectRoomStep
        where T : class, IFloorPlanGenContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectRoomStep{T}"/> class.
        /// </summary>
        public ConnectRoomStep()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectRoomStep{T}"/> class with specified hall generators.
        /// </summary>
        /// <param name="genericHalls">The picker for hall generators.</param>
        public ConnectRoomStep(IRandPicker<PermissiveRoomGen<T>> genericHalls)
            : base(genericHalls)
        {
        }

        /// <summary>
        /// Gets or sets the connection factor determining the number of connections to make.
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item><description>0 = No connections</description></item>
        /// <item><description>50 = Half of all rooms connected</description></item>
        /// <item><description>100 = All rooms connected once</description></item>
        /// <item><description>200 = All rooms connected twice on average</description></item>
        /// </list>
        /// </remarks>
        public RandRange ConnectFactor { get; set; }

        /// <summary>
        /// Applies this step to connect rooms in the floor plan.
        /// </summary>
        /// <param name="rand">The random number generator.</param>
        /// <param name="floorPlan">The floor plan to modify.</param>
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

        /// <summary>
        /// Returns a string representation of this step.
        /// </summary>
        /// <returns>A string describing this step's configuration.</returns>
        public override string ToString()
        {
            return string.Format("{0}: {1}%", this.GetType().GetFormattedTypeName(), this.ConnectFactor);
        }
    }
}
