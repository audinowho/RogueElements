// <copyright file="ConnectStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Finds rooms in the floor plan that can be connected and connects them.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class ConnectStep<T> : FloorPlanStep<T>
        where T : class, IFloorPlanGenContext
    {
        protected ConnectStep()
        {
            this.Components = new ComponentCollection();
            this.Filters = new List<BaseRoomFilter>();
        }

        protected ConnectStep(IRandPicker<PermissiveRoomGen<T>> genericHalls)
        {
            this.GenericHalls = genericHalls;
            this.Components = new ComponentCollection();
            this.Filters = new List<BaseRoomFilter>();
        }

        /// <summary>
        /// Determines which rooms are eligible to be connected.
        /// </summary>
        public List<BaseRoomFilter> Filters { get; set; }

        /// <summary>
        /// The room types that can be used as the hall connecting the two base rooms.
        /// </summary>
        public IRandPicker<PermissiveRoomGen<T>> GenericHalls { get; set; }

        /// <summary>
        /// Components that the newly added halls will be labeled with.
        /// </summary>
        public ComponentCollection Components { get; set; }

        protected static bool HasBorderOpening(IRoomGen roomFrom, Rect rectTo, Dir4 expandTo)
        {
            Loc diff = roomFrom.Draw.Start - rectTo.Start; // how far ahead the start of source is to dest
            int offset = diff.GetScalar(expandTo.ToAxis().Orth());

            // Traverse the region that both borders touch
            int sourceLength = roomFrom.Draw.GetBorderLength(expandTo);
            int destLength = rectTo.Size.GetScalar(expandTo.ToAxis().Orth());
            for (int ii = Math.Max(0, offset); ii - offset < sourceLength && ii < destLength; ii++)
            {
                bool sourceFulfill = roomFrom.GetFulfillableBorder(expandTo, ii - offset);
                if (sourceFulfill)
                    return true;
            }

            return false;
        }

        protected static ListPathTraversalNode? GetRoomToConnect(FloorPlan floorPlan, RoomHallIndex chosenFrom, Dir4 dir)
        {
            // extend a rectangle to the border of the floor in the chosen direction
            bool vertical = dir.ToAxis() == Axis4.Vert;
            int dirSign = dir.GetLoc().GetScalar(dir.ToAxis());
            IRoomGen genFrom = floorPlan.GetRoomHall(chosenFrom).RoomGen;
            Rect sampleRect = genFrom.Draw;

            // expand from the start of that border direction to the borders of the floor
            sampleRect.Start += dir.GetLoc() * sampleRect.Size.GetScalar(dir.ToAxis());

            // it doesn't have to be exactly the borders so just add the total size to be sure
            sampleRect.Expand(dir, vertical ? floorPlan.Size.Y : floorPlan.Size.X);

            // find the closest room.
            var chosenTo = new RoomHallIndex(-1, false);
            foreach (RoomHallIndex collision in floorPlan.CheckCollision(sampleRect))
            {
                Rect collidedRect = floorPlan.GetRoomHall(collision).RoomGen.Draw;

                // limit the expansion by direction
                int sampleScalar = sampleRect.GetScalar(dir);
                int collidedScalar = collidedRect.GetScalar(dir.Reverse());
                bool limit = dirSign == Math.Sign(sampleScalar - collidedScalar);
                if (limit)
                {
                    // update the boundaries
                    sampleRect.SetScalar(dir, collidedScalar);
                    chosenTo = collision;
                }
            }

            // if it didn't collide with ANYTHING, then quit
            if (chosenTo.Index == -1)
                return null;

            IRoomGen genTo = floorPlan.GetRoomHall(chosenTo).RoomGen;

            // narrow the rectangle if touching something on the side
            // widen the rectangle by width
            Rect widthRect = sampleRect;
            widthRect.Inflate(vertical ? 1 : 0, vertical ? 0 : 1);
            bool retractLeft = false;
            bool retractRight = false;
            Dir4 leftDir = DirExt.AddAngles(dir, Dir4.Left);
            Dir4 rightDir = DirExt.AddAngles(dir, Dir4.Right);
            foreach (RoomHallIndex collision in floorPlan.CheckCollision(widthRect))
            {
                Rect collidedRect = floorPlan.GetRoomHall(collision).RoomGen.Draw;
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

            // retract the rectangle
            if (retractLeft)
                sampleRect.Expand(leftDir, -1);
            if (retractRight)
                sampleRect.Expand(rightDir, -1);

            // if the rectangle has been retracted too much, we can't go on
            if (sampleRect.Area <= 0)
                return null;

            // check for border availability between start and end
            bool foundFrom = HasBorderOpening(genFrom, sampleRect, dir);
            bool foundTo = HasBorderOpening(genTo, sampleRect, dir.Reverse());

            // return the expansion if one is found
            if (foundFrom && foundTo)
                return new ListPathTraversalNode(chosenFrom, chosenTo, sampleRect);
            else
                return null;
        }

        protected static SpawnList<ListPathTraversalNode> GetPossibleExpansions(FloorPlan floorPlan, List<RoomHallIndex> candList)
        {
            // get all probabilities.
            // the probability of an extension is the distance that the target room is from the start room, in rooms
            var expansions = new SpawnList<ListPathTraversalNode>();

            for (int nn = 0; nn < candList.Count; nn++)
            {
                // find the room to connect to
                // go through all sides of all rooms (randomly)
                RoomHallIndex chosenFrom = candList[nn];
                IFloorRoomPlan planFrom = floorPlan.GetRoomHall(chosenFrom);

                // exhausting all possible directions (randomly)
                foreach (Dir4 dir in DirExt.VALID_DIR4)
                {
                    bool forbidExtend = false;
                    foreach (RoomHallIndex adjacent in planFrom.Adjacents)
                    {
                        Rect adjRect = floorPlan.GetRoomHall(adjacent).RoomGen.Draw;
                        if (planFrom.RoomGen.Draw.GetScalar(dir) == adjRect.GetScalar(dir.Reverse()))
                        {
                            forbidExtend = true;
                            break;
                        }
                    }

                    if (!forbidExtend)
                    {
                        // find a rectangle to connect it with
                        ListPathTraversalNode? expandToResult = GetRoomToConnect(floorPlan, chosenFrom, dir);

                        if (expandToResult is ListPathTraversalNode expandTo)
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

        protected static ListPathTraversalNode? ChooseConnection(IRandom rand, FloorPlan floorPlan, List<RoomHallIndex> candList)
        {
            SpawnList<ListPathTraversalNode> expansions = GetPossibleExpansions(floorPlan, candList);

            if (expansions.Count > 0)
                return expansions.Pick(rand);
            else
                return null;
        }

        public struct ListPathTraversalNode
        {
            public RoomHallIndex From;
            public RoomHallIndex To;
            public Rect Connector;

            public ListPathTraversalNode(RoomHallIndex from, RoomHallIndex to)
            {
                this.From = from;
                this.To = to;
                this.Connector = Rect.Empty;
            }

            public ListPathTraversalNode(RoomHallIndex from, RoomHallIndex to, Rect connector)
            {
                this.From = from;
                this.To = to;
                this.Connector = connector;
            }
        }
    }
}
