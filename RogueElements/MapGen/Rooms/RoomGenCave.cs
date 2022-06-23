// <copyright file="RoomGenCave.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Generates a cave-like room using cellular automata.
    /// Will generate a square if asked to generate for a size it did not propose.
    /// For square-looking rooms, check to make sure the room was not cut down.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class RoomGenCave<T> : RoomGen<T>, ISizedRoomGen
        where T : ITiledGenContext
    {
        private const int AUTOMATA_CHANCE = 55;

        private const int AUTOMATA_ROUNDS = 5;

        [NonSerialized]
        private bool[][] tiles;

        public RoomGenCave()
        {
        }

        public RoomGenCave(RandRange width, RandRange height)
        {
            this.Width = width;
            this.Height = height;
        }

        protected RoomGenCave(RoomGenCave<T> other)
        {
            this.Width = other.Width;
            this.Height = other.Height;
        }

        /// <summary>
        /// The max width of the room.  The actual cave will tend to be smaller.
        /// </summary>
        public RandRange Width { get; set; }

        /// <summary>
        /// The max height of the room.  The actual cave will tend to be smaller.
        /// </summary>
        public RandRange Height { get; set; }

        protected bool[][] Tiles { get => this.tiles; set => this.tiles = value; }

        public override RoomGen<T> Copy() => new RoomGenCave<T>(this);

        public override Loc ProposeSize(IRandom rand)
        {
            BlobMap largestMap = null;
            int blobIdx = -1;

            for (int ii = 0; ii < 10; ii++)
            {
                Loc size = new Loc(this.Width.Max, this.Height.Max);
                Loc testSize = new Loc(Math.Max(6, size.X + 1), Math.Max(6, size.Y + 1));
                bool[][] noise = new bool[testSize.X][];
                for (int xx = 0; xx < testSize.X; xx++)
                {
                    noise[xx] = new bool[testSize.Y];
                    for (int yy = 0; yy < testSize.Y; yy++)
                        noise[xx][yy] = rand.Next(100) < AUTOMATA_CHANCE;
                }

                noise = NoiseGen.IterateAutomata(noise, CellRule.Gte5, CellRule.Gte4, AUTOMATA_ROUNDS, false);

                bool IsValid(Loc loc) => noise[loc.X][loc.Y];

                BlobMap blobMap = Detection.DetectBlobs(new Rect(0, 0, noise.Length, noise[0].Length), IsValid);

                if (blobMap.Blobs.Count > 0)
                {
                    for (int bb = 0; bb < blobMap.Blobs.Count; bb++)
                    {
                        if (blobMap.Blobs[bb].Bounds.Width <= size.X && blobMap.Blobs[bb].Bounds.Height <= size.Y)
                        {
                            if (largestMap == null || blobMap.Blobs[bb].Bounds.Area > largestMap.Blobs[blobIdx].Bounds.Area)
                            {
                                largestMap = blobMap;
                                blobIdx = bb;

                                if (largestMap.Blobs[blobIdx].Bounds.Width >= this.Width.Min && largestMap.Blobs[blobIdx].Bounds.Height >= this.Height.Min)
                                {
                                    Rect blobRect = largestMap.Blobs[blobIdx].Bounds;
                                    this.Tiles = new bool[blobRect.Width][];
                                    for (int xx = 0; xx < blobRect.Width; xx++)
                                    {
                                        this.Tiles[xx] = new bool[blobRect.Height];
                                        for (int yy = 0; yy < blobRect.Height; yy++)
                                            this.Tiles[xx][yy] = largestMap.Map[xx + blobRect.X][yy + blobRect.Y] == blobIdx;
                                    }

                                    return new Loc(this.Tiles.Length, this.Tiles[0].Length);
                                }
                            }
                        }
                    }
                }
            }

            if (largestMap != null)
            {
                Rect blobRect = largestMap.Blobs[blobIdx].Bounds;
                this.Tiles = new bool[blobRect.Width][];
                for (int xx = 0; xx < blobRect.Width; xx++)
                {
                    this.Tiles[xx] = new bool[blobRect.Height];
                    for (int yy = 0; yy < blobRect.Height; yy++)
                        this.Tiles[xx][yy] = largestMap.Map[xx + blobRect.X][yy + blobRect.Y] == blobIdx;
                }

                return new Loc(this.Tiles.Length, this.Tiles[0].Length);
            }

            // unlikely, but will default if the case
            this.Tiles = new bool[1][];
            this.Tiles[0] = new bool[1];
            this.Tiles[0][0] = true;
            return new Loc(this.Tiles.Length, this.Tiles[0].Length);
        }

        public override void DrawOnMap(T map)
        {
            if (this.Draw.Width != this.Tiles.Length || this.Draw.Height != this.Tiles[0].Length)
            {
                this.DrawMapDefault(map);
                return;
            }

            for (int xx = 0; xx < this.Draw.Width; xx++)
            {
                for (int yy = 0; yy < this.Draw.Height; yy++)
                {
                    if (this.Tiles[xx][yy])
                        map.SetTile(new Loc(this.Draw.X + xx, this.Draw.Y + yy), map.RoomTerrain.Copy());
                }
            }

            // hall restrictions
            this.SetRoomBorders(map);
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}x{2}", this.GetType().Name, this.Width, this.Height);
        }

        protected override void PrepareFulfillableBorders(IRandom rand)
        {
            // accept nothing but the randomly chosen size
            if (this.Draw.Width != this.Tiles.Length || this.Draw.Height != this.Tiles[0].Length)
            {
                foreach (Dir4 dir in DirExt.VALID_DIR4)
                {
                    for (int jj = 0; jj < this.FulfillableBorder[dir].Length; jj++)
                        this.FulfillableBorder[dir][jj] = true;
                }
            }
            else
            {
                for (int ii = 0; ii < this.Draw.Width; ii++)
                {
                    this.FulfillableBorder[Dir4.Up][ii] = this.Tiles[ii][0];
                    this.FulfillableBorder[Dir4.Down][ii] = this.Tiles[ii][this.Draw.Height - 1];
                }

                for (int ii = 0; ii < this.Draw.Height; ii++)
                {
                    this.FulfillableBorder[Dir4.Left][ii] = this.Tiles[0][ii];
                    this.FulfillableBorder[Dir4.Right][ii] = this.Tiles[this.Draw.Width - 1][ii];
                }
            }
        }
    }
}
