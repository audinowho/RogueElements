// <copyright file="RoomGenCave.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class RoomGenCave<T> : RoomGen<T> where T : ITiledGenContext
    {
        const int AUTOMATA_CHANCE = 55;
        const int AUTOMATA_ROUNDS = 5;

        public RandRange Width;
        public RandRange Height;

        [NonSerialized]
        protected bool[][] tiles;
        [NonSerialized]
        protected int chosenMinorHeight;
        [NonSerialized]
        protected int chosenOffsetX;
        [NonSerialized]
        protected int chosenOffsetY;

        public RoomGenCave() { }

        public RoomGenCave(RandRange width, RandRange height)
        {
            Width = width;
            Height = height;
        }
        protected RoomGenCave(RoomGenCave<T> other)
        {
            Width = other.Width;
            Height = other.Height;
        }
        public override RoomGen<T> Copy() { return new RoomGenCave<T>(this); }

        public override Loc ProposeSize(IRandom rand)
        {
            for (int ii = 0; ii < 10; ii++)
            {
                Loc size = new Loc(Width.Pick(rand), Height.Pick(rand));
                bool[][] noise = new bool[size.X][];
                for (int xx = 0; xx < size.X; xx++)
                {
                    noise[xx] = new bool[size.Y];
                    for (int yy = 0; yy < size.Y; yy++)
                        noise[xx][yy] = (rand.Next(100) < AUTOMATA_CHANCE);
                }

                noise = NoiseGen.IterateAutomata(noise, CellRule.Gte5, CellRule.Gte4, AUTOMATA_ROUNDS);

                bool isValid(Loc loc) => noise[loc.X][loc.Y];

                BlobMap blobMap = Detection.DetectBlobs(new Rect(0, 0, noise.Length, noise[0].Length), isValid);

                if (blobMap.Blobs.Count > 0)
                {
                    int blobIdx = 0;
                    for (int bb = 1; bb < blobMap.Blobs.Count; bb++)
                    {
                        if (blobMap.Blobs[bb].Area > blobMap.Blobs[blobIdx].Area)
                            blobIdx = bb;
                    }
                    Rect blobRect = blobMap.Blobs[blobIdx].Bounds;
                    tiles = new bool[blobRect.Width][];
                    for (int xx = 0; xx < blobRect.Width; xx++)
                    {
                        tiles[xx] = new bool[blobRect.Height];
                        for (int yy = 0; yy < blobRect.Height; yy++)
                            tiles[xx][yy] = blobMap.Map[xx + blobRect.X][yy + blobRect.Y] == blobIdx;
                    }
                    return new Loc(tiles.Length, tiles[0].Length);
                }
            }

            //unlikely, but will default if the case
            tiles = new bool[1][];
            tiles[0] = new bool[1];
            tiles[0][0] = true;
            return new Loc(tiles.Length, tiles[0].Length);
        }


        protected override void PrepareFulfillableBorders(IRandom rand)
        {
            //accept nothing but the randomly chosen size
            if (Draw.Width != tiles.Length || Draw.Height != tiles[0].Length)
            {
                for (int ii = 0; ii < 4; ii++)
                {
                    for (int jj = 0; jj < fulfillableBorder[ii].Length; jj++)
                        fulfillableBorder[ii][jj] = true;
                }
            }
            else
            {
                for (int ii = 0; ii < draw.Width; ii++)
                {
                    fulfillableBorder[(int)Dir4.Up][ii] = tiles[ii][0];
                    fulfillableBorder[(int)Dir4.Down][ii] = tiles[ii][draw.Height - 1];
                }

                for (int ii = 0; ii < draw.Height; ii++)
                {
                    fulfillableBorder[(int)Dir4.Left][ii] = tiles[0][ii];
                    fulfillableBorder[(int)Dir4.Right][ii] = tiles[Draw.Width - 1][ii];
                }
            }
        }

        public override void DrawOnMap(T map)
        {
            if (Draw.Width != tiles.Length || Draw.Height != tiles[0].Length)
            {
                DrawMapDefault(map);
                return;
            }
            for (int xx = 0; xx < Draw.Width; xx++)
            {
                for (int yy = 0; yy < Draw.Height; yy++)
                {
                    if (tiles[xx][yy])
                        map.SetTile(new Loc(Draw.X + xx, Draw.Y + yy), map.RoomTerrain.Copy());
                }
            }

            //hall restrictions
            SetRoomBorders(map);
        }
    }
}
