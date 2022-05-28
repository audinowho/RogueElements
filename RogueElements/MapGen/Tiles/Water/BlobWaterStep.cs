// <copyright file="BlobWaterStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Creates blobs of water using cellular automata, and places them around the map.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class BlobWaterStep<T> : WaterStep<T>
        where T : class, ITiledGenContext
    {
        private const int AUTOMATA_CHANCE = 55;
        private const int AUTOMATA_ROUNDS = 5;

        public BlobWaterStep()
            : base()
        {
        }

        public BlobWaterStep(RandRange blobs, ITile terrain, ITerrainStencil<T> stencil, int minScale, RandRange startScale)
            : base(terrain, stencil)
        {
            this.Blobs = blobs;
            this.MinScale = minScale;
            this.StartScale = startScale;
        }

        /// <summary>
        /// The number of blobs to place.
        /// </summary>
        public RandRange Blobs { get; set; }

        /// <summary>
        /// The minimum size of the area creating the blob.  It is measured in percentage of the full map size.
        /// </summary>
        public int MinScale { get; set; }

        /// <summary>
        /// The maximum size of the area creating the blob.  It is measured in percentage of the full map size.
        /// </summary>
        public RandRange StartScale { get; set; }

        public override void Apply(T map)
        {
            int blobs = this.Blobs.Pick(map.Rand);
            int startScale = Math.Max(this.MinScale, this.StartScale.Pick(map.Rand));
            for (int ii = 0; ii < blobs; ii++)
            {
                Loc size = new Loc(map.Width * startScale / 100, map.Height * startScale / 100);
                int area = size.X * size.Y;
                bool placed = false;
                while (area > 0 && area >= this.MinScale * map.Width / 100 * this.MinScale * map.Height / 100)
                {
                    bool[][] noise = new bool[size.X][];
                    for (int xx = 0; xx < size.X; xx++)
                    {
                        noise[xx] = new bool[size.Y];
                        for (int yy = 0; yy < size.Y; yy++)
                            noise[xx][yy] = map.Rand.Next(100) < AUTOMATA_CHANCE;
                    }

                    noise = NoiseGen.IterateAutomata(noise, CellRule.Gte5, CellRule.Gte4, AUTOMATA_ROUNDS);

                    bool IsWaterValid(Loc loc) => noise[loc.X][loc.Y];

                    BlobMap blobMap = Detection.DetectBlobs(new Rect(0, 0, noise.Length, noise[0].Length), IsWaterValid);

                    if (blobMap.Blobs.Count > 0)
                    {
                        int blobIdx = 0;
                        for (int bb = 1; bb < blobMap.Blobs.Count; bb++)
                        {
                            if (blobMap.Blobs[bb].Area > blobMap.Blobs[blobIdx].Area)
                                blobIdx = bb;
                        }

                        placed = this.AttemptBlob(map, blobMap, blobIdx);
                    }

                    if (placed)
                        break;

                    size = size * 7 / 10;
                    area = size.X * size.Y;
                }
            }
        }

        protected virtual bool AttemptBlob(T map, BlobMap blobMap, int blobIdx)
        {
            Rect blobRect = blobMap.Blobs[blobIdx].Bounds;
            Loc offset = new Loc(map.Rand.Next(0, map.Width - blobRect.Width), map.Rand.Next(0, map.Height - blobRect.Height));

            this.DrawBlob(map, blobMap, blobIdx, offset);
            return true;
        }
    }
}
