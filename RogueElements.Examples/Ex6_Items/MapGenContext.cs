// <copyright file="MapGenContext.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using RogueElements;

namespace RogueElements.Examples.Ex6_Items
{
    public class MapGenContext : BaseMapGenContext<Map>, IRoomGridGenContext,
        IViewPlaceableGenContext<StairsUp>, IViewPlaceableGenContext<StairsDown>,
        IPlaceableGenContext<Item>, IPlaceableGenContext<Mob>
    {
        public MapGenContext()
            : base()
        {
        }

        protected delegate List<Loc> GetOpen(Rect rect);

        public FloorPlan RoomPlan { get; private set; }

        public GridPlan GridPlan { get; private set; }

        public List<StairsUp> GenEntrances => this.Map.GenEntrances;

        public List<StairsDown> GenExits => this.Map.GenExits;

        int IViewPlaceableGenContext<StairsUp>.Count => this.GenEntrances.Count;

        int IViewPlaceableGenContext<StairsDown>.Count => this.GenExits.Count;

        public override bool CanSetTile(Loc loc, ITile tile)
        {
            for (int ii = 0; ii < this.GenEntrances.Count; ii++)
            {
                if (this.GenEntrances[ii].Loc == loc)
                    return false;
            }

            for (int ii = 0; ii < this.GenExits.Count; ii++)
            {
                if (this.GenExits[ii].Loc == loc)
                    return false;
            }

            return true;
        }

        public void InitPlan(FloorPlan plan)
        {
            this.RoomPlan = plan;
        }

        public void InitGrid(GridPlan plan)
        {
            this.GridPlan = plan;
        }

        List<Loc> IPlaceableGenContext<Item>.GetAllFreeTiles() => this.GetAllFreeTiles(this.GetOpenTiles);

        List<Loc> IPlaceableGenContext<Mob>.GetAllFreeTiles() => this.GetAllFreeTiles(this.GetOpenTiles);

        List<Loc> IPlaceableGenContext<StairsUp>.GetAllFreeTiles() => this.GetAllFreeTiles(this.GetOpenTiles);

        List<Loc> IPlaceableGenContext<StairsDown>.GetAllFreeTiles() => this.GetAllFreeTiles(this.GetOpenTiles);

        List<Loc> IPlaceableGenContext<Item>.GetFreeTiles(Rect rect) => this.GetOpenTiles(rect);

        List<Loc> IPlaceableGenContext<Mob>.GetFreeTiles(Rect rect) => this.GetOpenTiles(rect);

        List<Loc> IPlaceableGenContext<StairsUp>.GetFreeTiles(Rect rect) => this.GetOpenTiles(rect);

        List<Loc> IPlaceableGenContext<StairsDown>.GetFreeTiles(Rect rect) => this.GetOpenTiles(rect);

        bool IPlaceableGenContext<Item>.CanPlaceItem(Loc loc) => !this.IsTileOccupied(loc);

        bool IPlaceableGenContext<Mob>.CanPlaceItem(Loc loc) => !this.IsTileOccupied(loc);

        bool IPlaceableGenContext<StairsUp>.CanPlaceItem(Loc loc) => !this.IsTileOccupied(loc);

        bool IPlaceableGenContext<StairsDown>.CanPlaceItem(Loc loc) => !this.IsTileOccupied(loc);

        void IPlaceableGenContext<Item>.PlaceItem(Loc loc, Item item)
        {
            Item newItem = new Item(item.ID, loc);
            this.Map.Items.Add(newItem);
        }

        void IPlaceableGenContext<Mob>.PlaceItem(Loc loc, Mob item)
        {
            Mob newItem = new Mob(item.ID, loc);
            this.Map.Mobs.Add(newItem);
        }

        void IPlaceableGenContext<StairsUp>.PlaceItem(Loc loc, StairsUp item)
        {
            var stairs = (StairsUp)item.Copy();
            stairs.Loc = loc;
            this.GenEntrances.Add(stairs);
        }

        void IPlaceableGenContext<StairsDown>.PlaceItem(Loc loc, StairsDown item)
        {
            var stairs = (StairsDown)item.Copy();
            stairs.Loc = loc;
            this.GenExits.Add(stairs);
        }

        StairsUp IViewPlaceableGenContext<StairsUp>.GetItem(int index) => this.GenEntrances[index];

        Loc IViewPlaceableGenContext<StairsUp>.GetLoc(int index) => this.GenEntrances[index].Loc;

        StairsDown IViewPlaceableGenContext<StairsDown>.GetItem(int index) => this.GenExits[index];

        Loc IViewPlaceableGenContext<StairsDown>.GetLoc(int index) => this.GenExits[index].Loc;

        protected virtual List<Loc> GetAllFreeTiles(GetOpen func)
        {
            return func?.Invoke(new Rect(0, 0, this.Width, this.Height));
        }

        protected List<Loc> GetOpenTiles(Rect rect)
        {
            bool CheckOp(Loc loc) => !this.IsTileOccupied(loc);

            return Grid.FindTilesInBox(rect.Start, rect.Size, CheckOp);
        }

        private bool IsTileOccupied(Loc loc)
        {
            if (this.Map.Tiles[loc.X][loc.Y].ID != Map.ROOM_TERRAIN_ID)
                return true;

            foreach (Item item in this.Map.Items)
            {
                if (item.Loc == loc)
                    return true;
            }

            foreach (Mob item in this.Map.Mobs)
            {
                if (item.Loc == loc)
                    return true;
            }

            return false;
        }
    }
}
