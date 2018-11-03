using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Subclass of RoomGen that cannot deal with every combination of paths leading into it.  Its RequestedBorder must be obeyed under all circumstances.
    /// It always has at least one open RequestedBorder tile open for each side.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class RoomGen<T> : IRoomGen where T : ITiledGenContext
    {
        //Ranges that must have at least one of their permitted tiles touched
        [NonSerialized]
        protected List<Range>[] roomSideReqs;
        
        //tiles that have been touched during room gen
        public bool GetOpenedBorder(Dir4 dir, int index) { return openedBorder[(int)dir][index]; }
        [NonSerialized]
        protected bool[][] openedBorder;
        
        //tiles that this room can take as incoming paths
        public bool GetFulfillableBorder(Dir4 dir, int index) { return fulfillableBorder[(int)dir][index]; }
        [NonSerialized]
        protected bool[][] fulfillableBorder;

        //tiles that, if touched during this room's gen, signify that the req has been filled
        //"the req" refers to the side reqs for that side.
        [NonSerialized]
        protected bool[][] borderToFulfill;

        public int GetBorderLength(Dir4 dir) { return Draw.GetSide(dir.ToAxis()).Length; }

        [NonSerialized]
        protected Rect draw;
        /// <summary>
        /// The rectangle that the room is drawn in.
        /// </summary>
        public virtual Rect Draw { get { return draw; } }

        public RoomGen()
        {

            roomSideReqs = new List<Range>[DirExt.DIR4_COUNT];
            for (int ii = 0; ii < roomSideReqs.Length; ii++)
                roomSideReqs[ii] = new List<Range>();
            openedBorder = new bool[DirExt.DIR4_COUNT][];
            fulfillableBorder = new bool[DirExt.DIR4_COUNT][];
            borderToFulfill = new bool[DirExt.DIR4_COUNT][];

            draw = new Rect(new Loc(-1), new Loc(-1));
        }

        //this structure is serialized, so make sure runtime state variables are clean at start

        public abstract Loc ProposeSize(IRandom rand);
        public virtual void PrepareSize(IRandom rand, Loc size)
        {
            if (size.X <= 0 || size.Y <= 0)
                throw new ArgumentException("Rooms must be of a positive size.");

            draw.Size = size;
            //set all border tile classes to the correct length
            for (int ii = 0; ii < DirExt.DIR4_COUNT; ii++)
            {
                openedBorder[ii] = new bool[GetBorderLength((Dir4)ii)];
                fulfillableBorder[ii] = new bool[GetBorderLength((Dir4)ii)];
                borderToFulfill[ii] = new bool[GetBorderLength((Dir4)ii)];
            }

            PrepareFulfillableBorders(rand);

            //verify that possible borders has at least one TRUE in each array
            for (int ii = 0; ii < DirExt.DIR4_COUNT; ii++)
            {
                bool hasRequestable = false;
                for (int jj = 0; jj < fulfillableBorder[ii].Length; jj++)
                {
                    if (fulfillableBorder[ii][jj])
                    {
                        hasRequestable = true;
                        break;
                    }
                }
                if (!hasRequestable)
                    throw new NotImplementedException("PrepareFulfillableBorders did not open at least one open tile for each direction!");
            }
        }
        
        public void SetLoc(Loc loc) { draw.Start = loc; }
        
        protected abstract void PrepareFulfillableBorders(IRandom rand);


        /// <summary>
        /// Transfers the opened tiles of one direction's OpenedBorder to the adjacent room's PermittedBorder
        /// </summary>
        /// <param name="sourceRoom">The target room</param>
        /// <param name="dir">The direction that the target room lies, relative to this room.</param>
        public virtual void ReceiveOpenedBorder(IRoomGen sourceRoom, Dir4 dir)
        {
            ReceiveBorder(sourceRoom, dir, false);
        }

        /// <summary>
        /// Transfers the opened tiles of one direction's FulfillableBorder to the adjacent room's PermittedBorder
        /// </summary>
        /// <param name="sourceRoom">The target room</param>
        /// <param name="dir">The direction that the target room lies, relative to this room.</param>
        public virtual void ReceiveFulfillableBorder(IRoomGen sourceRoom, Dir4 dir)
        {
            ReceiveBorder(sourceRoom, dir, true);
        }

        protected void ReceiveBorder(IRoomGen sourceRoom, Dir4 dir, bool fulfillable)
        {
            Loc startLoc = GetEdgeLoc(dir, 0);
            Loc endLoc = sourceRoom.GetEdgeLoc(dir.Reverse(), 0);
            if (startLoc + dir.GetLoc() != endLoc)
                throw new ArgumentException("Rooms must touch each other in the specified direction.");

            //compute the starting index in otherBorder to start transferring
            Range sourceSide = sourceRoom.Draw.GetSide(dir.ToAxis());
            fillSideReq(sourceSide, dir);
            
            bool[] destBorder = borderToFulfill[(int)dir];
            Loc diff = sourceRoom.Draw.Start - Draw.Start;//how far ahead the start of source is to dest
            //compute the starting index in otherBorder to start transferring
            int offset = diff.GetScalar(dir.ToAxis().Orth());
            //Traverse the region that both borders touch
            //make this room's opened borders into the other room's permitted borders
            bool hasOpening = false;
            int sourceLength = sourceRoom.GetBorderLength(dir.Reverse());
            for (int ii = Math.Max(0, offset); ii - offset < sourceLength && ii < destBorder.Length; ii++)
            {
                bool sourceOpened = false;
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

        //assumes that the borders are touching.
        public virtual void ReceiveBorderRange(Range range, Dir4 dir)
        {
            fillSideReq(range, dir);
            
            bool[] destBorder = borderToFulfill[(int)dir];
            //compute the starting index in otherBorder to start transferring
            int offset = range.Min - Draw.Start.GetScalar(dir.ToAxis().Orth());
            //Traverse the region that both borders touch
            //make this room's opened borders into the other room's permitted borders
            for (int ii = Math.Max(0, -offset); ii < range.Length && ii + offset < destBorder.Length; ii++)
                destBorder[ii + offset] = true;
        }

        private void fillSideReq(Range range, Dir4 dir)
        {
            if (range.Length <= 0)
                throw new ArgumentException("Range must have a positive length.");
            if (dir <= Dir4.None || (int)dir >= DirExt.DIR4_COUNT)
                throw new ArgumentException("Invalid dir value.");
            //also throw exception if the range fails to
            //hit at least one open requestableBorderTile
            Range side = Draw.GetSide(dir.ToAxis());
            Range trueRange = new Range(Math.Max(range.Min, side.Min), Math.Min(range.Max, side.Max));
            bool fulfillable = false;
            for (int ii = trueRange.Min - side.Min; ii < trueRange.Max - side.Min; ii++)
            {
                if (fulfillableBorder[(int)dir][ii])
                {
                    fulfillable = true;
                    break;
                }
            }
            if (!fulfillable)
                throw new ArgumentException("The given range does not include a fulfillable tile!");
            roomSideReqs[(int)dir].Add(trueRange);
        }

        //needs to pass in cell size
        //just return the map itself?
        //also needs to return offset
        //can pass in item/mob lists via map
        public abstract void DrawOnMap(T map);

        void IRoomGen.DrawOnMap(ITiledGenContext map) { DrawOnMap((T)map); }

        protected void DrawMapDefault(T map)
        {
            //draw on all
            for (int x = 0; x < Draw.Size.X; x++)
            {
                for (int y = 0; y < Draw.Size.Y; y++)
                    map.SetTile(new Loc(Draw.X + x, Draw.Y + y), map.RoomTerrain.Copy());
            }

            SetRoomBorders(map);
        }

        public virtual void SetRoomBorders(T map)
        {
            for (int ii = 0; ii < Draw.Width; ii++)
            {
                openedBorder[(int)Dir4.Up][ii] = fulfillableBorder[(int)Dir4.Up][ii] && !map.TileBlocked(new Loc(Draw.Start.X + ii, Draw.Start.Y));
                openedBorder[(int)Dir4.Down][ii] = fulfillableBorder[(int)Dir4.Down][ii] && !map.TileBlocked(new Loc(Draw.Start.X + ii, Draw.End.Y - 1));
            }
            
            for (int ii = 0; ii < Draw.Height; ii++)
            {
                openedBorder[(int)Dir4.Left][ii] = fulfillableBorder[(int)Dir4.Left][ii] && !map.TileBlocked(new Loc(Draw.Start.X, Draw.Start.Y + ii));
                openedBorder[(int)Dir4.Right][ii] = fulfillableBorder[(int)Dir4.Right][ii] && !map.TileBlocked(new Loc(Draw.End.X - 1, Draw.Start.Y + ii));
            }
        }

        /// <summary>
        /// Simple method to fulfill border requirements by digging until the room is reached.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="room"></param>
        public virtual void FulfillRoomBorders(T map)
        {
            //NOTE: This assumes that reaching any open tile results in reaching the room as a whole.
            //It also assumes that an open tile would eventually be reached if dug far enough.
            //for each side

            //get all unfulfilled borders
            List<Range>[] unfulfilled = new List<Range>[roomSideReqs.Length];
            for (int ii = 0; ii < roomSideReqs.Length; ii++)
            {
                unfulfilled[ii] = new List<Range>();
                unfulfilled[ii].AddRange(roomSideReqs[ii]);
            }

            for (int ii = 0; ii < Draw.Width; ii++)
            {
                if (!map.TileBlocked(new Loc(Draw.Start.X + ii, Draw.Start.Y)))
                    updateUnfulfilled(unfulfilled[(int)Dir4.Up], Draw.Start.X + ii);

                if (!map.TileBlocked(new Loc(Draw.Start.X + ii, Draw.End.Y - 1)))
                    updateUnfulfilled(unfulfilled[(int)Dir4.Down], Draw.Start.X + ii);
            }

            for (int ii = 0; ii < Draw.Height; ii++)
            {
                if (!map.TileBlocked(new Loc(Draw.Start.X, Draw.Start.Y + ii)))
                    updateUnfulfilled(unfulfilled[(int)Dir4.Left], Draw.Start.Y + ii);
                if (!map.TileBlocked(new Loc(Draw.End.X - 1, Draw.Start.Y + ii)))
                    updateUnfulfilled(unfulfilled[(int)Dir4.Right], Draw.Start.Y + ii);
            }

            for (int ii = 0; ii < unfulfilled.Length; ii++)
            {
                Dir4 dir = (Dir4)ii;
                //get the permitted tiles for each sidereq
                Range side = Draw.GetSide(dir.ToAxis());
                bool[] permittedRange = new bool[side.Length];
                for (int jj = 0; jj < permittedRange.Length; jj++)
                    permittedRange[jj] = true;
                List<HashSet<int>> candidateEntrances = ChoosePossibleStartRanges(map.Rand, side.Min, permittedRange, unfulfilled[ii]);
                //randomly roll them
                List<int> resultEntrances = new List<int>();
                foreach(HashSet<int> candidateSet in candidateEntrances)
                    resultEntrances.Add(MathUtils.ChooseFromHash(candidateSet, map.Rand));
                //fulfill them with a simple inwards digging until a walkable is reached
                for (int jj = 0; jj < resultEntrances.Count; jj++)
                    DigAtBorder(map, dir, resultEntrances[jj]);
            }
        }

        /// <summary>
        /// Digs inwards from a border until it reaches a traversible tile.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="room"></param>
        /// <param name="dir">The direction of the border, facing outwards.</param>
        /// <param name="scalar"></param>
        public virtual void DigAtBorder(ITiledGenContext map, Dir4 dir, int scalar)
        {
            Loc curLoc = GetEdgeLoc(dir, scalar);
            int length = dir.ToAxis() == Axis4.Vert ? Draw.Height : Draw.Width;
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
            if (!foundTile)//complain if we reach the end
                throw new ArgumentException("Room border auto-tunneling could not find open tile.");
        }
        
        private void updateUnfulfilled(List<Range> unfulfilled, int ii)
        {
            for (int jj = unfulfilled.Count - 1; jj >= 0; jj--)
            {
                if (unfulfilled[jj].Contains(ii))
                    unfulfilled.RemoveAt(jj);
            }
        }
        
        /// <summary>
        /// Gets the loc just inside the room, from the specified direction, with the specified scalar.  The scalar determines X if it's a vertical, and Y if it's a horizontal side.
        /// </summary>
        /// <returns></returns>
        public Loc GetEdgeLoc(Dir4 dir, int scalar)
        {
            switch (dir)
            {
                case Dir4.Down:
                    return new Loc(scalar, Draw.End.Y - 1);
                case Dir4.Left:
                    return new Loc(Draw.X, scalar);
                case Dir4.Up:
                    return new Loc(scalar, Draw.Y);
                case Dir4.Right:
                    return new Loc(Draw.End.X - 1, scalar);
            }
            throw new ArgumentException("Must specify a valid direction!");
        }
        
        public Loc GetEdgeRectLoc(Dir4 dir, Loc size, int scalar)
        {
            switch (dir)
            {
                case Dir4.Down:
                    return new Loc(scalar, Draw.End.Y);
                case Dir4.Left:
                    return new Loc(Draw.X - size.X, scalar);
                case Dir4.Up:
                    return new Loc(scalar, Draw.Y - size.Y);
                case Dir4.Right:
                    return new Loc(Draw.End.X, scalar);
            }
            throw new ArgumentException("Must specify a valid direction!");
        }

        /// <summary>
        /// Returns a list of tile-collections, the whole of which would cover all sidereqs.
        /// The sets are all mutually exclusive to each other, and the minimum amount is always chosen.
        /// </summary>
        /// <param name="room"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        public virtual List<HashSet<int>> ChoosePossibleStartRanges(IRandom rand, int scalarStart, bool[] permittedRange, List<Range> origSideReqs)
        {
            //Gets the starting X if the direction is vertical, starting Y if the direction is horizontal
            List<Range> sideReqs = new List<Range>();
            sideReqs.AddRange(origSideReqs);

            List<HashSet<int>> resultStarts = new List<HashSet<int>>();
            while (sideReqs.Count > 0)
            {
                //choose a random sidereq
                int chosenIndex = rand.Next(sideReqs.Count);
                HashSet<int> tiles = new HashSet<int>();
                //add all included bordertiles to its set
                for (int ii = 0; ii < permittedRange.Length; ii++)
                {
                    if (permittedRange[ii] && sideReqs[chosenIndex].Contains(ii + scalarStart))
                        tiles.Add(ii + scalarStart);
                }
                bool overlap = false;
                for (int ii = 0; ii < resultStarts.Count; ii++)
                {
                    //does its bordertiles overlap? superset? subset? with an existing set?
                    if (tiles.IsSupersetOf(resultStarts[ii]))
                    {
                        //if superset, do nothing
                        overlap = true;
                        break;
                    }
                    else if (tiles.Overlaps(resultStarts[ii]))
                    {
                        //if subset, narrow the tiles to that set
                        //if intersect, narrow the tiles to that set
                        resultStarts[ii].IntersectWith(tiles);
                        overlap = true;
                        break;
                    }
                }
                //if nothing, just add the new set
                if (!overlap)
                    resultStarts.Add(tiles);

                //continue for all sidereqs
                sideReqs.RemoveAt(chosenIndex);
            }
            return resultStarts;
        }

    }


    public interface IRoomGen
    {
        void ReceiveOpenedBorder(IRoomGen sourceRoom, Dir4 dir);
        void ReceiveFulfillableBorder(IRoomGen sourceRoom, Dir4 dir);
        void ReceiveBorderRange(Range range, Dir4 dir);
        bool GetOpenedBorder(Dir4 dir, int index);
        bool GetFulfillableBorder(Dir4 dir, int index);
        int GetBorderLength(Dir4 dir);
        Rect Draw { get; }
        Loc ProposeSize(IRandom rand);
        void PrepareSize(IRandom rand, Loc size);
        void SetLoc(Loc loc);
        void DrawOnMap(ITiledGenContext map);
        Loc GetEdgeLoc(Dir4 dir, int scalar);
        Loc GetEdgeRectLoc(Dir4 dir, Loc size, int scalar);
    }
}
