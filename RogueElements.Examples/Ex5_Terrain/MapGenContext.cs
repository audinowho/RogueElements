using System;
using System.Collections.Generic;
using System.Linq;
using RogueElements;

namespace RogueElements.Examples.Ex5_Terrain
{

    public class MapGenContext : ITiledGenContext, IRoomGridGenContext, IViewPlaceableGenContext<StairsUp>, IViewPlaceableGenContext<StairsDown>
    {
        public Map Map { get; set; }

        public ITile RoomTerrain { get { return new Tile(Map.ROOM_TERRAIN_ID); } }
        public ITile WallTerrain { get { return new Tile(Map.WALL_TERRAIN_ID); } }

        public ITile GetTile(Loc loc) { return Map.Tiles[loc.X][loc.Y]; }
        public bool CanSetTile(Loc loc, ITile tile)
        {
            for (int ii = 0; ii < GenEntrances.Count; ii++)
            {
                if (GenEntrances[ii].Loc == loc)
                    return false;
            }
            for (int ii = 0; ii < GenExits.Count; ii++)
            {
                if (GenExits[ii].Loc == loc)
                    return false;
            }
            return true;
        }
        public bool TrySetTile(Loc loc, ITile tile)
        {
            if (!CanSetTile(loc, tile)) return false;
            Map.Tiles[loc.X][loc.Y] = (Tile)tile;
            return true;
        }
        public void SetTile(Loc loc, ITile tile)
        {
            if (!TrySetTile(loc, tile))
                throw new InvalidOperationException("Can't place tile!");
        }
        public bool TilesInitialized { get { return Map.Tiles != null; } }

        public int Width { get { return Map.Width; } }
        public int Height { get { return Map.Height; } }

        public List<StairsUp> GenEntrances {  get { return Map.GenEntrances; } }
        public List<StairsDown> GenExits { get { return Map.GenExits; } }

        public IRandom Rand { get { return Map.Rand; } }

        public MapGenContext()
        {
            Map = new Map();
        }
        
        public void InitSeed(ulong seed)
        {
            Map.Rand = new ReRandom(seed);
        }

        bool ITiledGenContext.TileBlocked(Loc loc)
        {
            return Map.Tiles[loc.X][loc.Y].ID == Map.WALL_TERRAIN_ID;
        }

        bool ITiledGenContext.TileBlocked(Loc loc, bool diagonal)
        {
            return Map.Tiles[loc.X][loc.Y].ID == Map.WALL_TERRAIN_ID;
        }


        public virtual void CreateNew(int width, int height)
        {
            Map.InitializeTiles(width, height);
        }


        public void FinishGen() { }


        public void InitPlan(FloorPlan plan)
        {
            RoomPlan = plan;
        }

        public FloorPlan RoomPlan { get; private set; }


        public void InitGrid(GridPlan plan)
        {
            GridPlan = plan;
        }
        public GridPlan GridPlan { get; private set; }




        List<Loc> IPlaceableGenContext<StairsUp>.GetAllFreeTiles() { return getAllFreeTiles(getOpenTiles); }
        List<Loc> IPlaceableGenContext<StairsDown>.GetAllFreeTiles() { return getAllFreeTiles(getOpenTiles); }


        protected delegate List<Loc> getOpen(Rect rect);
        protected virtual List<Loc> getAllFreeTiles(getOpen func)
        {
            return func(new Rect(0, 0, Width, Height));
        }

        List<Loc> IPlaceableGenContext<StairsUp>.GetFreeTiles(Rect rect) { return getOpenTiles(rect); }
        List<Loc> IPlaceableGenContext<StairsDown>.GetFreeTiles(Rect rect) { return getOpenTiles(rect); }

        protected List<Loc> getOpenTiles(Rect rect)
        {
            Grid.LocTest checkOp = (Loc loc) =>
            {
                return !isTileOccupied(loc);
            };

            return Grid.FindTilesInBox(rect.Start, rect.Size, checkOp);
        }

        bool IPlaceableGenContext<StairsUp>.CanPlaceItem(Loc loc) { return isTileOccupied(loc); }
        bool IPlaceableGenContext<StairsDown>.CanPlaceItem(Loc loc) { return isTileOccupied(loc); }

        private bool isTileOccupied(Loc loc)
        {
            if (Map.Tiles[loc.X][loc.Y].ID != Map.ROOM_TERRAIN_ID)
                return true;


            return false;
        }


        void IPlaceableGenContext<StairsUp>.PlaceItem(Loc loc, StairsUp item)
        {
            StairsUp stairs = item.Copy();
            stairs.Loc = loc;
            GenEntrances.Add(stairs);
        }
        void IPlaceableGenContext<StairsDown>.PlaceItem(Loc loc, StairsDown item)
        {
            StairsDown stairs = item.Copy();
            stairs.Loc = loc;
            GenExits.Add(stairs);
        }


        int IViewPlaceableGenContext<StairsUp>.Count { get { return GenEntrances.Count; } }
        StairsUp IViewPlaceableGenContext<StairsUp>.GetItem(int index) { return GenEntrances[index]; }
        Loc IViewPlaceableGenContext<StairsUp>.GetLoc(int index) { return GenEntrances[index].Loc; }

        int IViewPlaceableGenContext<StairsDown>.Count { get { return GenExits.Count; } }
        StairsDown IViewPlaceableGenContext<StairsDown>.GetItem(int index) { return GenExits[index]; }
        Loc IViewPlaceableGenContext<StairsDown>.GetLoc(int index) { return GenExits[index].Loc; }

    }
}
