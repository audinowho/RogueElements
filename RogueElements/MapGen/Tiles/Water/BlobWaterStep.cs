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
            this.BlobStencil = new DefaultBlobStencil<T>();
        }

        public BlobWaterStep(RandRange blobs, ITile terrain, ITerrainStencil<T> stencil, IBlobStencil<T> blobStencil, IntRange areaScale, IntRange generateScale)
            : base(terrain, stencil)
        {
            this.Blobs = blobs;
            this.BlobStencil = blobStencil;
            this.AreaScale = areaScale;
            this.GenerateScale = generateScale;
        }

        /// <summary>
        /// The number of blobs to place.
        /// </summary>
        public RandRange Blobs { get; set; }

        /// <summary>
        /// The minimum size of the area creating the blob.  It is measured in tiles.
        /// </summary>
        public IntRange AreaScale { get; set; }

        /// <summary>
        /// The maximum size of the area creating the blob.  It is measured in tiles.
        /// </summary>
        public IntRange GenerateScale { get; set; }

        /// <summary>
        /// Blob-wide stencil.  All-or-nothing: If the blob position passes this stencil, it is drawn.  Otherwise it is not.
        /// </summary>
        public IBlobStencil<T> BlobStencil { get; set; }

        public override void Apply(T map)
        {
            int blobs = this.Blobs.Pick(map.Rand);
            int startScale = this.GenerateScale.Max - 1;
            for (int ii = 0; ii < blobs; ii++)
            {
                Loc size = new Loc(startScale, startScale);
                int area = size.X * size.Y;
                bool placed = false;
                while (area > 0 && area >= this.GenerateScale.Min * this.GenerateScale.Min)
                {
                    bool[][] noise = new bool[size.X][];
                    for (int xx = 0; xx < size.X; xx++)
                    {
                        noise[xx] = new bool[size.Y];
                        for (int yy = 0; yy < size.Y; yy++)
                            noise[xx][yy] = map.Rand.Next(100) < AUTOMATA_CHANCE;
                    }

                    noise = NoiseGen.IterateAutomata(noise, CellRule.Gte5, CellRule.Gte4, AUTOMATA_ROUNDS, false);

                    bool IsWaterValid(Loc loc) => noise[loc.X][loc.Y];

                    BlobMap blobMap = Detection.DetectBlobs(new Rect(0, 0, noise.Length, noise[0].Length), IsWaterValid);

                    if (blobMap.Blobs.Count > 0)
                    {
                        int blobIdx = 0;
                        for (int bb = 1; bb < blobMap.Blobs.Count; bb++)
                        {
                            if (this.AreaScale.Min <= blobMap.Blobs[bb].Area && blobMap.Blobs[bb].Area < this.AreaScale.Max)
                                blobIdx = bb;
                        }

                        BlobMap.Blob mapBlob = blobMap.Blobs[blobIdx];

                        // attempt to place in 20 locations
                        for (int jj = 0; jj < 20; jj++)
                        {
                            Loc offset = new Loc(map.Rand.Next(0, map.Width - mapBlob.Bounds.Width), map.Rand.Next(0, map.Height - mapBlob.Bounds.Height));
                            placed = this.AttemptBlob(map, blobMap, blobIdx, offset);

                            if (placed)
                                break;
                        }
                    }

                    if (placed)
                        break;

                    size = size * 3 / 4;
                    area = size.X * size.Y;
                }
            }
        }

        protected virtual bool AttemptBlob(T map, BlobMap blobMap, int blobIdx, Loc offset)
        {
            BlobMap.Blob mapBlob = blobMap.Blobs[blobIdx];
            if (!this.BlobStencil.Test(map, new Rect(offset, mapBlob.Bounds.Size)))
                return false;

            this.DrawBlob(map, blobMap, blobIdx, offset);
            return true;
        }
    }
}
