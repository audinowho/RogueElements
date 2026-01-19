// <copyright file="BlobWaterStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Generates water blobs using cellular automata and places them at random locations on the map.
    /// </summary>
    /// <typeparam name="T">The type of map context that implements <see cref="ITiledGenContext"/>.</typeparam>
    [Serializable]
    public class BlobWaterStep<T> : WaterStep<T>, IBlobWaterStep
        where T : class, ITiledGenContext
    {
        private const int AUTOMATA_CHANCE = 55;
        private const int AUTOMATA_ROUNDS = 5;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobWaterStep{T}"/> class.
        /// </summary>
        public BlobWaterStep()
            : base()
        {
            this.BlobStencil = new DefaultBlobStencil<T>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobWaterStep{T}"/> class with the specified parameters.
        /// </summary>
        /// <param name="blobs">The range for the number of blobs to generate.</param>
        /// <param name="terrain">The water terrain tile to place.</param>
        /// <param name="stencil">The per-tile stencil for eligible placement locations.</param>
        /// <param name="blobStencil">The blob-wide stencil for valid blob placement.</param>
        /// <param name="areaScale">The acceptable area range for each blob.</param>
        /// <param name="generateScale">The generation area range for creating blobs.</param>
        public BlobWaterStep(RandRange blobs, ITile terrain, ITerrainStencil<T> stencil, IBlobStencil<T> blobStencil, IntRange areaScale, IntRange generateScale)
            : base(terrain, stencil)
        {
            this.Blobs = blobs;
            this.BlobStencil = blobStencil;
            this.AreaScale = areaScale;
            this.GenerateScale = generateScale;
        }

        /// <summary>
        /// Gets or sets the range for the number of blobs to place.
        /// </summary>
        public RandRange Blobs { get; set; }

        /// <summary>
        /// Gets or sets the NxN size range of the generation area for creating blobs.
        /// It is recommended to pick a range with at least 4 between min and max.
        /// </summary>
        public IntRange GenerateScale { get; set; }

        /// <summary>
        /// Gets or sets the NxN size range of the acceptable blob area.
        /// Must be equal to or smaller than <see cref="GenerateScale"/>.
        /// </summary>
        public IntRange AreaScale { get; set; }

        /// <summary>
        /// Gets or sets the blob-wide stencil for placement validation.
        /// If the blob position fails this stencil, the entire blob is rejected.
        /// </summary>
        public IBlobStencil<T> BlobStencil { get; set; }

        /// <inheritdoc/>
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
                        int blobIdx = -1;
                        for (int bb = 0; bb < blobMap.Blobs.Count; bb++)
                        {
                            if (this.AreaScale.Min * this.AreaScale.Min <= blobMap.Blobs[bb].Area && blobMap.Blobs[bb].Area < this.AreaScale.Max * this.AreaScale.Max)
                                blobIdx = bb;
                        }

                        if (blobIdx > -1)
                        {
                            BlobMap.Blob mapBlob = blobMap.Blobs[blobIdx];

                            // attempt to place in 30 locations
                            for (int jj = 0; jj < 30; jj++)
                            {
                                int maxWidth = Math.Max(1, map.Width - mapBlob.Bounds.Width);
                                int maxHeight = Math.Max(1, map.Height - mapBlob.Bounds.Height);
                                Loc offset = new Loc(map.Rand.Next(0, maxWidth), map.Rand.Next(0, maxHeight));
                                placed = this.AttemptBlob(map, blobMap, blobIdx, offset);

                                if (placed)
                                    break;
                            }
                        }
                    }

                    if (placed)
                        break;

                    size = size * 3 / 4;
                    area = size.X * size.Y;
                }
            }
        }

        public override string ToString()
        {
            return string.Format("{0}: Amt:{1} Size:{2}", this.GetType().GetFormattedTypeName(), this.Blobs.ToString(), this.AreaScale.ToString());
        }

        /// <summary>
        /// Attempts to place a blob from the blob map at the specified offset.
        /// </summary>
        /// <param name="map">The map context to place the blob on.</param>
        /// <param name="blobMap">The blob map containing the blob shapes.</param>
        /// <param name="blobIdx">The index of the blob to place.</param>
        /// <param name="offset">The position offset for blob placement.</param>
        /// <returns><c>true</c> if the blob was successfully placed; otherwise, <c>false</c>.</returns>
        protected virtual bool AttemptBlob(T map, BlobMap blobMap, int blobIdx, Loc offset)
        {
            BlobMap.Blob mapBlob = blobMap.Blobs[blobIdx];

            bool IsBlobValid(Loc loc)
            {
                Loc srcLoc = loc - offset + mapBlob.Bounds.Start;
                if (Collision.InBounds(mapBlob.Bounds, srcLoc))
                    return blobMap.Map[srcLoc.X][srcLoc.Y] == blobIdx;
                return false;
            }

            if (!this.BlobStencil.Test(map, new Rect(offset, mapBlob.Bounds.Size), IsBlobValid))
                return false;

            this.DrawBlob(map, new Rect(offset, mapBlob.Bounds.Size), IsBlobValid);
            return true;
        }
    }
}
