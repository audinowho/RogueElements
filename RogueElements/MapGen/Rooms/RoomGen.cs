// <copyright file="RoomGen.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// A class representing a room-generating algorithm. It supports connections to other rooms.
    /// </summary>
    /// <remarks>
    /// RoomGens obey the following rules:
    /// 1. All RoomGens must generate a solvable room.  Aka, one where it is possible to get to any opening in its 4 sides, to any other opening in its 4 sides.
    /// * This means, it is okay if some generated rooms can be “cheesed” out of any self-contained puzzle they’re trying to make.A cheesable room is better than a wholly unsolvable.
    /// 2. All RoomGens must be capable of taking any Size, and generate without throwing an exception.
    /// * So if you have a RoomGen that is meant to make a complicated self contained maze, and the calling code says "No, you only get 2x2 tiles of space to work with, deal with it", it will have to comply. (Usually by just making a blank square)
    /// * But, you can ask a RoomGen what dimensions it would like to be, and then pass it those dimensions to play nice with it.This is the usual case.
    /// 3. A RoomGen must be able to produce at least one opening for each of the four cardinal directions, if asked.
    /// * For example, a simple square room has openings on all four sides regardless of how it’s generated.Certain styles of rooms do not want to have any walkable tiles on the North border unless mandated.
    /// * Another example would be if the algorithm from above placed this RoomGen between two rooms: one above and one below.It wants to connect them from above and below.The RoomGen must provide an opening somewhere for its north and south borders.
    /// * The keyword is somewhere.Somewhere that the RoomGen gets to pick and the calling code cannot.
    /// </remarks>
    /// <typeparam name="T">The MapGenContext to apply the room to.</typeparam>
    [Serializable]
    public abstract class RoomGen<T> : IRoomGen
        where T : ITiledGenContext
    {
        // Ranges that must have at least one of their permitted tiles touched
        [NonSerialized]
        private Dictionary<Dir4, List<Range>> roomSideReqs;

        [NonSerialized]
        private Dictionary<Dir4, bool[]> openedBorder;

        [NonSerialized]
        private Dictionary<Dir4, bool[]> fulfillableBorder;

        // tiles that, if touched during this room's gen, signify that the req has been filled
        // "the req" refers to the side reqs for that side.
        [NonSerialized]
        private Dictionary<Dir4, bool[]> borderToFulfill;

        [NonSerialized]
        private Rect draw;

        protected RoomGen()
        {
            this.RoomSideReqs = new Dictionary<Dir4, List<Range>>();
            foreach (Dir4 dir in DirExt.VALID_DIR4)
                this.RoomSideReqs[dir] = new List<Range>();
            this.OpenedBorder = new Dictionary<Dir4, bool[]>();
            this.FulfillableBorder = new Dictionary<Dir4, bool[]>();
            this.BorderToFulfill = new Dictionary<Dir4, bool[]>();

            this.Draw = new Rect(new Loc(-1), new Loc(-1));
        }

        /// <summary>
        /// The rectangle that the room is drawn in.
        /// </summary>
        public virtual Rect Draw { get => this.draw; protected set => this.draw = value; }

        protected Dictionary<Dir4, List<Range>> RoomSideReqs { get => this.roomSideReqs; set => this.roomSideReqs = value; }

        protected Dictionary<Dir4, bool[]> OpenedBorder { get => this.openedBorder; set => this.openedBorder = value; }

        protected Dictionary<Dir4, bool[]> FulfillableBorder { get => this.fulfillableBorder; set => this.fulfillableBorder = value; }

        protected Dictionary<Dir4, bool[]> BorderToFulfill { get => this.borderToFulfill; set => this.borderToFulfill = value; }

        // tiles that have been touched during room gen
        public bool GetOpenedBorder(Dir4 dir, int index) => this.OpenedBorder[dir][index];

        // tiles that this room can take as incoming paths
        public bool GetFulfillableBorder(Dir4 dir, int index) => this.FulfillableBorder[dir][index];

        public int GetBorderLength(Dir4 dir) => this.Draw.GetSide(dir.ToAxis()).Length;

        /// <summary>
        /// Creates a copy of the object, to be placed in the generated layout.
        /// </summary>
        /// <returns></returns>
        public abstract RoomGen<T> Copy();

        IRoomGen IRoomGen.Copy() => this.Copy();

        // this structure is serialized, so make sure runtime state variables are clean at start

        /// <summary>
        /// Returns a Loc that represents the dimensions that this RoomGen prefers to be.
        /// </summary>
        /// <param name="rand"></param>
        /// <returns></returns>
        public abstract Loc ProposeSize(IRandom rand);

        /// <summary>
        /// Initializes the room to the specified size. If its proposed size is not used, it may draw a default empty square.
        /// </summary>
        /// <param name="rand"></param>
        /// <param name="size"></param>
        public virtual void PrepareSize(IRandom rand, Loc size)
        {
            if (size.X <= 0 || size.Y <= 0)
                throw new ArgumentException("Rooms must be of a positive size.");

            Rect currDraw = this.Draw;
            currDraw.Size = size;
            this.Draw = currDraw;

            // set all border tile classes to the correct length
            foreach (Dir4 dir in DirExt.VALID_DIR4)
            {
                this.OpenedBorder[dir] = new bool[this.GetBorderLength(dir)];
                this.FulfillableBorder[dir] = new bool[this.GetBorderLength(dir)];
                this.BorderToFulfill[dir] = new bool[this.GetBorderLength(dir)];
            }

            this.PrepareFulfillableBorders(rand);

            // verify that possible borders has at least one TRUE in each array
            foreach (Dir4 dir in DirExt.VALID_DIR4)
            {
                bool hasRequestable = false;
                for (int jj = 0; jj < this.FulfillableBorder[dir].Length; jj++)
                {
                    if (this.FulfillableBorder[dir][jj])
                    {
                        hasRequestable = true;
                        break;
                    }
                }

                if (!hasRequestable)
                    throw new ArgumentException("PrepareFulfillableBorders did not open at least one open tile for each direction!");
            }
        }

        public void SetLoc(Loc loc)
        {
            Rect currDraw = this.Draw;
            currDraw.Start = loc;
            this.Draw = currDraw;
        }

       /// <summary>
        /// Transfers the opened tiles of one direction's OpenedBorder to the adjacent room's PermittedBorder
        /// </summary>
        /// <param name="sourceRoom">The target room</param>
        /// <param name="dir">The direction that the target room lies, relative to this room.</param>
        public virtual void ReceiveOpenedBorder(IRoomGen sourceRoom, Dir4 dir)
        {
            this.ReceiveBorder(sourceRoom, dir, false);
        }

        /// <summary>
        /// Transfers the opened tiles of one direction's FulfillableBorder to the adjacent room's PermittedBorder
        /// </summary>
        /// <param name="sourceRoom">The target room</param>
        /// <param name="dir">The direction that the target room lies, relative to this room.</param>
        public virtual void ReceiveFulfillableBorder(IRoomGen sourceRoom, Dir4 dir)
        {
            this.ReceiveBorder(sourceRoom, dir, true);
        }

        // assumes that the borders are touching.
        public virtual void ReceiveBorderRange(Range range, Dir4 dir)
        {
            this.FillSideReq(range, dir);

            bool[] destBorder = this.BorderToFulfill[dir];

            // compute the starting index in otherBorder to start transferring
            int offset = range.Min - this.Draw.Start.GetScalar(dir.ToAxis().Orth());

            // Traverse the region that both borders touch
            // make this room's opened borders into the other room's permitted borders
            for (int ii = Math.Max(0, -offset); ii < range.Length && ii + offset < destBorder.Length; ii++)
                destBorder[ii + offset] = true;
        }

        // needs to pass in cell size
        // just return the map itself?
        // also needs to return offset
        // can pass in item/mob lists via map
        public abstract void DrawOnMap(T map);

        void IRoomGen.DrawOnMap(ITiledGenContext map) => this.DrawOnMap((T)map);

        public virtual void SetRoomBorders(T map)
        {
            for (int ii = 0; ii < this.Draw.Width; ii++)
            {
                this.OpenedBorder[Dir4.Up][ii] = this.FulfillableBorder[Dir4.Up][ii] && !map.TileBlocked(new Loc(this.Draw.Start.X + ii, this.Draw.Start.Y));
                this.OpenedBorder[Dir4.Down][ii] = this.FulfillableBorder[Dir4.Down][ii] && !map.TileBlocked(new Loc(this.Draw.Start.X + ii, this.Draw.End.Y - 1));
            }

            for (int ii = 0; ii < this.Draw.Height; ii++)
            {
                this.OpenedBorder[Dir4.Left][ii] = this.FulfillableBorder[Dir4.Left][ii] && !map.TileBlocked(new Loc(this.Draw.Start.X, this.Draw.Start.Y + ii));
                this.OpenedBorder[Dir4.Right][ii] = this.FulfillableBorder[Dir4.Right][ii] && !map.TileBlocked(new Loc(this.Draw.End.X - 1, this.Draw.Start.Y + ii));
            }
        }

        /// <summary>
        /// Simple method to fulfill border requirements by digging until the room is reached.
        /// </summary>
        /// <param name="map">Map to draw on.</param>
        /// <param name="openAll">Chooses all borders instead of just one.</param>
        public virtual void FulfillRoomBorders(T map, bool openAll)
        {
            // NOTE: This assumes that reaching any open tile results in reaching the room as a whole.
            // It also assumes that an open tile would eventually be reached if dug far enough.
            // for each side

            // get all unfulfilled borders
            var unfulfilled = new Dictionary<Dir4, List<Range>>();
            foreach (Dir4 dir in DirExt.VALID_DIR4)
            {
                unfulfilled[dir] = new List<Range>();
                unfulfilled[dir].AddRange(this.RoomSideReqs[dir]);
            }

            if (!openAll)
            {
                for (int ii = 0; ii < this.Draw.Width; ii++)
                {
                    if (!map.TileBlocked(new Loc(this.Draw.Start.X + ii, this.Draw.Start.Y)))
                        UpdateUnfulfilled(unfulfilled[Dir4.Up], this.Draw.Start.X + ii);

                    if (!map.TileBlocked(new Loc(this.Draw.Start.X + ii, this.Draw.End.Y - 1)))
                        UpdateUnfulfilled(unfulfilled[Dir4.Down], this.Draw.Start.X + ii);
                }

                for (int ii = 0; ii < this.Draw.Height; ii++)
                {
                    if (!map.TileBlocked(new Loc(this.Draw.Start.X, this.Draw.Start.Y + ii)))
                        UpdateUnfulfilled(unfulfilled[Dir4.Left], this.Draw.Start.Y + ii);
                    if (!map.TileBlocked(new Loc(this.Draw.End.X - 1, this.Draw.Start.Y + ii)))
                        UpdateUnfulfilled(unfulfilled[Dir4.Right], this.Draw.Start.Y + ii);
                }
            }

            foreach (Dir4 dir in DirExt.VALID_DIR4)
            {
                // get the permitted tiles for each sidereq
                Range side = this.Draw.GetSide(dir.ToAxis());

                if (!openAll)
                {
                    List<HashSet<int>> candidateEntrances = this.ChoosePossibleStartRanges(map.Rand, side.Min, this.BorderToFulfill[dir], unfulfilled[dir]);

                    // randomly roll them
                    List<int> resultEntrances = new List<int>();
                    foreach (HashSet<int> candidateSet in candidateEntrances)
                        resultEntrances.Add(MathUtils.ChooseFromHash(candidateSet, map.Rand));

                    // fulfill them with a simple inwards digging until a walkable is reached
                    for (int jj = 0; jj < resultEntrances.Count; jj++)
                        this.DigAtBorder(map, dir, resultEntrances[jj]);
                }
                else
                {
                    for (int jj = 0; jj < side.Length; jj++)
                    {
                        if (this.FulfillableBorder[dir][jj])
                        {
                            foreach (Range range in unfulfilled[dir])
                            {
                                if (range.Contains(side.Min + jj))
                                {
                                    this.DigAtBorder(map, dir, side.Min + jj);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Digs inwards from a border until it reaches a traversible tile.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="dir">The direction of the border, facing outwards.</param>
        /// <param name="scalar"></param>
        public virtual void DigAtBorder(ITiledGenContext map, Dir4 dir, int scalar)
        {
            Loc curLoc = this.GetEdgeLoc(dir, scalar);
            int length = dir.ToAxis() == Axis4.Vert ? this.Draw.Height : this.Draw.Width;
            bool foundTile = false;
            for (int kk = 0; kk < length; kk++)
            {
                if (!map.TileBlocked(curLoc))
                {
                    foundTile = true;
                    break;
                }

                map.SetTile(curLoc, map.RoomTerrain.Copy());
                curLoc += dir.Reverse().GetLoc();
            }

            if (!foundTile) // complain if we reach the end
                throw new ArgumentException("Room border auto-tunneling could not find open tile.");
        }

        /// <summary>
        /// Gets the loc just inside the room, from the specified direction, with the specified scalar.  The scalar determines X if it's a vertical, and Y if it's a horizontal side.
        /// </summary>
        /// <param name="dir">todo: describe dir parameter on GetEdgeLoc</param>
        /// <param name="scalar">todo: describe scalar parameter on GetEdgeLoc</param>
        /// <returns></returns>
        public Loc GetEdgeLoc(Dir4 dir, int scalar)
        {
            switch (dir)
            {
                case Dir4.Down:
                    return new Loc(scalar, this.Draw.End.Y - 1);
                case Dir4.Left:
                    return new Loc(this.Draw.X, scalar);
                case Dir4.Up:
                    return new Loc(scalar, this.Draw.Y);
                case Dir4.Right:
                    return new Loc(this.Draw.End.X - 1, scalar);
                case Dir4.None:
                    throw new ArgumentException($"No edge for dir {nameof(Dir4.None)}");
                default:
                    throw new ArgumentOutOfRangeException(nameof(dir), "Invalid enum value.");
            }
        }

        public Loc GetEdgeRectLoc(Dir4 dir, Loc size, int scalar)
        {
            switch (dir)
            {
                case Dir4.Down:
                    return new Loc(scalar, this.Draw.End.Y);
                case Dir4.Left:
                    return new Loc(this.Draw.X - size.X, scalar);
                case Dir4.Up:
                    return new Loc(scalar, this.Draw.Y - size.Y);
                case Dir4.Right:
                    return new Loc(this.Draw.End.X, scalar);
                case Dir4.None:
                    throw new ArgumentException($"No edge for dir {nameof(Dir4.None)}");
                default:
                    throw new ArgumentOutOfRangeException(nameof(dir), "Invalid enum value.");
            }

            throw new ArgumentException("Must specify a valid direction!");
        }

        /// <summary>
        /// Returns a list of tile-collections, the whole of which would cover all sidereqs.
        /// The sets are all mutually exclusive to each other, and the minimum amount is always chosen.
        /// </summary>
        /// <param name="rand">todo: describe rand parameter on ChoosePossibleStartRanges</param>
        /// <param name="scalarStart">todo: describe scalarStart parameter on ChoosePossibleStartRanges</param>
        /// <param name="permittedRange">todo: describe permittedRange parameter on ChoosePossibleStartRanges</param>
        /// <param name="origSideReqs">todo: describe origSideReqs parameter on ChoosePossibleStartRanges</param>
        /// <returns></returns>
        public virtual List<HashSet<int>> ChoosePossibleStartRanges(IRandom rand, int scalarStart, bool[] permittedRange, List<Range> origSideReqs)
        {
            // Gets the starting X if the direction is vertical, starting Y if the direction is horizontal
            List<Range> sideReqs = new List<Range>();
            sideReqs.AddRange(origSideReqs);

            List<HashSet<int>> resultStarts = new List<HashSet<int>>();
            while (sideReqs.Count > 0)
            {
                // choose a random sidereq
                int chosenIndex = rand.Next(sideReqs.Count);
                HashSet<int> tiles = new HashSet<int>();

                // add all included bordertiles to its set
                for (int ii = 0; ii < permittedRange.Length; ii++)
                {
                    if (permittedRange[ii] && sideReqs[chosenIndex].Contains(ii + scalarStart))
                        tiles.Add(ii + scalarStart);
                }

                bool overlap = false;
                for (int ii = 0; ii < resultStarts.Count; ii++)
                {
                    // does its bordertiles overlap? superset? subset? with an existing set?
                    if (tiles.IsSupersetOf(resultStarts[ii]))
                    {
                        // if superset, do nothing
                        overlap = true;
                        break;
                    }
                    else if (tiles.Overlaps(resultStarts[ii]))
                    {
                        // if subset, narrow the tiles to that set
                        // if intersect, narrow the tiles to that set
                        resultStarts[ii].IntersectWith(tiles);
                        overlap = true;
                        break;
                    }
                }

                // if nothing, just add the new set
                if (!overlap)
                    resultStarts.Add(tiles);

                // continue for all sidereqs
                sideReqs.RemoveAt(chosenIndex);
            }

            return resultStarts;
        }

        protected abstract void PrepareFulfillableBorders(IRandom rand);

        protected void ReceiveBorder(IRoomGen sourceRoom, Dir4 dir, bool fulfillable)
        {
            Loc startLoc = this.GetEdgeLoc(dir, 0);
            Loc endLoc = sourceRoom.GetEdgeLoc(dir.Reverse(), 0);
            if (startLoc + dir.GetLoc() != endLoc)
                throw new ArgumentException("Rooms must touch each other in the specified direction.");

            // compute the starting index in otherBorder to start transferring
            Range sourceSide = sourceRoom.Draw.GetSide(dir.ToAxis());
            this.FillSideReq(sourceSide, dir);
            bool[] destBorder = this.BorderToFulfill[dir];
            Loc diff = sourceRoom.Draw.Start - this.Draw.Start; // how far ahead the start of source is to dest

            // compute the starting index in otherBorder to start transferring
            int offset = diff.GetScalar(dir.ToAxis().Orth());

            // Traverse the region that both borders touch
            // make this room's opened borders into the other room's permitted borders
            bool hasOpening = false;
            int sourceLength = sourceRoom.GetBorderLength(dir.Reverse());
            for (int ii = Math.Max(0, offset); ii - offset < sourceLength && ii < destBorder.Length; ii++)
            {
                bool sourceOpened;
                if (fulfillable)
                    sourceOpened = sourceRoom.GetFulfillableBorder(dir.Reverse(), ii - offset);
                else
                    sourceOpened = sourceRoom.GetOpenedBorder(dir.Reverse(), ii - offset);

                destBorder[ii] = sourceOpened || destBorder[ii];
                hasOpening |= sourceOpened;
            }

            if (!hasOpening)
                throw new ArgumentException("Permitted borders needs at least one open tile for each sideReq!");
        }

        protected void DrawMapDefault(T map)
        {
            // draw on all
            for (int x = 0; x < this.Draw.Size.X; x++)
            {
                for (int y = 0; y < this.Draw.Size.Y; y++)
                    map.SetTile(new Loc(this.Draw.X + x, this.Draw.Y + y), map.RoomTerrain.Copy());
            }

            this.SetRoomBorders(map);
        }

        private static void UpdateUnfulfilled(List<Range> unfulfilled, int ii)
        {
            for (int jj = unfulfilled.Count - 1; jj >= 0; jj--)
            {
                if (unfulfilled[jj].Contains(ii))
                    unfulfilled.RemoveAt(jj);
            }
        }

        private void FillSideReq(Range range, Dir4 dir)
        {
            if (range.Length <= 0)
                throw new ArgumentException("Range must have a positive length.");
            if (dir == Dir4.None || !dir.Validate())
                throw new ArgumentException("Invalid dir value.");

            // also throw exception if the range fails to
            // hit at least one open requestableBorderTile
            Range side = this.Draw.GetSide(dir.ToAxis());
            Range trueRange = new Range(Math.Max(range.Min, side.Min), Math.Min(range.Max, side.Max));
            bool fulfillable = false;
            for (int ii = trueRange.Min - side.Min; ii < trueRange.Max - side.Min; ii++)
            {
                if (this.FulfillableBorder[dir][ii])
                {
                    fulfillable = true;
                    break;
                }
            }

            if (!fulfillable)
                throw new ArgumentException("The given range does not include a fulfillable tile!");
            this.RoomSideReqs[dir].Add(trueRange);
        }
    }
}
