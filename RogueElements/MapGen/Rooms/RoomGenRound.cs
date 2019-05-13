// <copyright file="RoomGenRound.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace RogueElements
{
    [Serializable]
    public class RoomGenRound<T> : RoomGen<T> where T : ITiledGenContext
    {
        public RandRange Width;
        public RandRange Height;

        public RoomGenRound() { }

        public RoomGenRound(RandRange width, RandRange height)
        {
            Width = width;
            Height = height;
        }
        protected RoomGenRound(RoomGenRound<T> other)
        {
            Width = other.Width;
            Height = other.Height;
        }
        public override RoomGen<T> Copy() { return new RoomGenRound<T>(this); }

        public override Loc ProposeSize(IRandom rand)
        {
            return new Loc(Width.Pick(rand), Height.Pick(rand));
        }

        protected override void PrepareFulfillableBorders(IRandom rand)
        {
            int diameter = Math.Min(Draw.Width, Draw.Height);
            for (int jj = 0; jj < Draw.Width; jj++)
            {
                if (isTileWithinRoom(jj, 0, diameter, Draw.Size))
                {
                    fulfillableBorder[(int)Dir4.Up][jj] = true;
                    fulfillableBorder[(int)Dir4.Down][jj] = true;
                }
            }
            for (int jj = 0; jj < Draw.Height; jj++)
            {
                if (isTileWithinRoom(0, jj, diameter, Draw.Size))
                {
                    fulfillableBorder[(int)Dir4.Left][jj] = true;
                    fulfillableBorder[(int)Dir4.Right][jj] = true;
                }
            }
        }

        public override void DrawOnMap(T map)
        {
            int diameter = Math.Min(Draw.Width, Draw.Height);

            for (int ii = 0; ii < Draw.Width; ii++)
            {
                for (int jj = 0; jj < Draw.Height; jj++)
                {

                    if (isTileWithinRoom(ii, jj, diameter, Draw.Size))
                        map.SetTile(new Loc(Draw.X + ii, Draw.Y + jj), map.RoomTerrain.Copy());
                }
            }

            //hall restrictions
            SetRoomBorders(map);
        }

        private bool isTileWithinRoom(int baseX, int baseY, int diameter, Loc size)
        {
            Loc sizeX2 = size * 2;
            int x = baseX * 2 + 1;
            int y = baseY * 2 + 1;

            if (x < diameter)
            {
                if (y < diameter)
                {
                    if ((diameter - x) * (diameter - x) + (diameter - y) * (diameter - y) < diameter * diameter)
                        return true;
                }
                else if (y > sizeX2.Y - diameter)
                {
                    if ((diameter - x) * (diameter - x) + (y - (sizeX2.Y - diameter)) * (y - (sizeX2.Y - diameter)) < diameter * diameter)
                        return true;
                }
                else
                    return true;
            }
            else if (x > sizeX2.X - diameter)
            {
                if (y < diameter)
                {
                    if ((x - (sizeX2.X - diameter)) * (x - (sizeX2.X - diameter)) + (diameter - y) * (diameter - y) < diameter * diameter)
                        return true;
                }
                else if (y > sizeX2.Y - diameter)
                {
                    if ((x - (sizeX2.X - diameter)) * (x - (sizeX2.X - diameter)) + (y - (sizeX2.Y - diameter)) * (y - (sizeX2.Y - diameter)) < diameter * diameter)
                        return true;
                }
                else
                    return true;
            }
            else
                return true;

            return false;
        }
    }
}
