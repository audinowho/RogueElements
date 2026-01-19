// <copyright file="ConnectStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Base class for steps that connect rooms in a floor plan with hallways.
    /// </summary>
    /// <typeparam name="T">The generation context type, which must implement <see cref="IFloorPlanGenContext"/>.</typeparam>
    /// <remarks>
    /// <para>
    /// Connect steps find pairs of rooms that can be connected by straight hallways and create
    /// those connections. This is useful for creating more interconnected floor layouts after
    /// the initial path generation.
    /// </para>
    /// <para>
    /// The connection algorithm extends a rectangle from each eligible room toward other rooms,
    /// finding the closest room that can be reached in each direction.
    /// </para>
    /// </remarks>
    /// <seealso cref="ConnectBranchStep{T}"/>
    /// <seealso cref="ConnectRoomStep{T}"/>
    [Serializable]
    public abstract class ConnectStep<T> : FloorPlanStep<T>
        where T : class, IFloorPlanGenContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectStep{T}"/> class.
        /// </summary>
        protected ConnectStep()
        {
            this.Components = new ComponentCollection();
            this.Filters = new List<BaseRoomFilter>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectStep{T}"/> class with specified hall generators.
        /// </summary>
        /// <param name="genericHalls">The picker for hall generators.</param>
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

        /// <summary>
        /// Checks if a room has any border opening toward another rectangle.
        /// </summary>
        /// <param name="roomFrom">The room to check for openings.</param>
        /// <param name="rectTo">The target rectangle.</param>
        /// <param name="expandTo">The direction to check.</param>
        /// <returns>True if any border tile can create an opening in the specified direction.</returns>
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

        /// <summary>
        /// Finds a room that can be connected from the specified room in the given direction.
        /// </summary>
        /// <param name="floorPlan">The floor plan to search.</param>
        /// <param name="chosenFrom">The room to connect from.</param>
        /// <param name="dir">The direction to search for connections.</param>
        /// <returns>The connection details if a valid target is found; otherwise, null.</returns>
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
            sampleRect.Expand(dir, floorPlan.Size.GetScalar(dir.ToAxis()));

            // find the closest room.
            var chosenTo = new RoomHallIndex(-1, false);
            foreach (RoomHallIndex collision in floorPlan.CheckCollision(sampleRect))
            {
                Rect collidedRect = floorPlan.GetRoomHall(collision).RoomGen.Draw;

                // limit the expansion by direction
                int sampleScalar = sampleRect.GetScalar(dir);
                int collidedScalar = collidedRect.GetScalar(dir.Reverse());

                // "unwrap" the collided scalar, such that it is always in front of the extrusion starting point
                if (floorPlan.Wrap)
                {
                    int sampleStartScalar = sampleRect.GetScalar(dir.Reverse());
                    collidedScalar = WrappedCollision.GetClosestDirWrap(floorPlan.Size.GetScalar(dir.ToAxis()), sampleStartScalar, collidedScalar, dirSign);
                }

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
            int leftScalar = sampleRect.GetScalar(leftDir);
            int rightScalar = sampleRect.GetScalar(rightDir);
            foreach (RoomHallIndex collision in floorPlan.CheckCollision(widthRect))
            {
                Rect collidedRect = floorPlan.GetRoomHall(collision).RoomGen.Draw;
                if (!retractLeft)
                {
                    int checkLeft = collidedRect.GetScalar(leftDir.Reverse());

                    // the roomhall in question must be as close as possible to the left scalar before comparing
                    if (floorPlan.Wrap)
                        checkLeft = WrappedCollision.GetClosestWrap(floorPlan.Size.GetScalar(dir.ToAxis().Orth()), leftScalar, checkLeft);
                    if (checkLeft == leftScalar)
                        retractLeft = true;
                }

                if (!retractRight)
                {
                    int checkRight = collidedRect.GetScalar(rightDir.Reverse());

                    // the roomhall in question must be as close as possible to the right scalar before comparing
                    if (floorPlan.Wrap)
                        checkRight = WrappedCollision.GetClosestWrap(floorPlan.Size.GetScalar(dir.ToAxis().Orth()), rightScalar, checkRight);
                    if (checkRight == rightScalar)
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

            Rect borderTestRect = sampleRect;
            if (floorPlan.Wrap)
            {
                // if wrapped, borderTestRect must be moved as close to genTo as possible
                Loc start = borderTestRect.Start;
                start.SetScalar(dir.ToAxis().Orth(), WrappedCollision.GetClosestBounds(
                    floorPlan.Size.GetScalar(dir.ToAxis().Orth()),
                    genTo.Draw.Start.GetScalar(dir.ToAxis().Orth()),
                    genTo.Draw.GetBorderLength(dir),
                    borderTestRect.Start.GetScalar(dir.ToAxis().Orth()),
                    borderTestRect.GetBorderLength(dir)));
                borderTestRect.Start = start;
            }

            bool foundTo = HasBorderOpening(genTo, borderTestRect, dir.Reverse());

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

        /// <summary>
        /// Chooses a connection from a list of candidate rooms.
        /// </summary>
        /// <param name="rand">The random number generator.</param>
        /// <param name="floorPlan">The floor plan to search.</param>
        /// <param name="candList">The list of candidate rooms to connect from.</param>
        /// <returns>The chosen connection details if one is found; otherwise, null.</returns>
        protected static ListPathTraversalNode? ChooseConnection(IRandom rand, FloorPlan floorPlan, List<RoomHallIndex> candList)
        {
            SpawnList<ListPathTraversalNode> expansions = GetPossibleExpansions(floorPlan, candList);

            if (expansions.Count > 0)
                return expansions.Pick(rand);
            else
                return null;
        }

        /// <summary>
        /// Represents the details of a potential connection between two rooms.
        /// </summary>
        public struct ListPathTraversalNode
        {
            /// <summary>
            /// The room or hall to connect from.
            /// </summary>
            public RoomHallIndex From;

            /// <summary>
            /// The room or hall to connect to.
            /// </summary>
            public RoomHallIndex To;

            /// <summary>
            /// The bounding rectangle for the connecting hall.
            /// </summary>
            public Rect Connector;

            /// <summary>
            /// Initializes a new instance of the <see cref="ListPathTraversalNode"/> struct without a connector.
            /// </summary>
            /// <param name="from">The room to connect from.</param>
            /// <param name="to">The room to connect to.</param>
            public ListPathTraversalNode(RoomHallIndex from, RoomHallIndex to)
            {
                this.From = from;
                this.To = to;
                this.Connector = Rect.Empty;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ListPathTraversalNode"/> struct with a connector.
            /// </summary>
            /// <param name="from">The room to connect from.</param>
            /// <param name="to">The room to connect to.</param>
            /// <param name="connector">The bounding rectangle for the hall.</param>
            public ListPathTraversalNode(RoomHallIndex from, RoomHallIndex to, Rect connector)
            {
                this.From = from;
                this.To = to;
                this.Connector = connector;
            }
        }
    }
}
