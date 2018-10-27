using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class RoomGenBlocked<T> : PermissiveRoomGen<T> where T : ITiledGenContext
    {
        public RandRange Width;
        public RandRange Height;
        public RandRange BlockWidth;
        public RandRange BlockHeight;

        public ITile BlockTerrain;

        public RoomGenBlocked() { }

        public RoomGenBlocked(ITile blockTerrain, RandRange width, RandRange height, RandRange blockWidth, RandRange blockHeight)
        {
            BlockTerrain = blockTerrain;
            Width = width;
            Height = height;
            BlockWidth = blockWidth;
            BlockHeight = blockHeight;
        }

        public override Loc ProposeSize(IRandom rand)
        {
            return new Loc(Width.Pick(rand), Height.Pick(rand));
        }

        public override void DrawOnMap(T map)
        {
            for (int x = 0; x < Draw.Size.X; x++)
            {
                for (int y = 0; y < Draw.Size.Y; y++)
                    map.SetTile(new Loc(Draw.X + x, Draw.Y + y), map.RoomTerrain.Copy());
            }

            Loc blockSize = new Loc(Math.Min(BlockWidth.Pick(map.Rand), Draw.Size.X - 2), Math.Min(BlockHeight.Pick(map.Rand), Draw.Size.Y - 2));
            Loc blockStart = new Loc(Draw.X + map.Rand.Next(1, Draw.Size.X - blockSize.X - 1), Draw.Y + map.Rand.Next(1, Draw.Size.Y - blockSize.Y - 1));
            for (int x = 0; x < blockSize.X; x++)
            {
                for (int y = 0; y < blockSize.Y; y++)
                    map.SetTile(new Loc(blockStart.X + x, blockStart.Y + y), BlockTerrain.Copy());
            }

            //hall restrictions
            SetRoomBorders(map);
        }
    }
}
