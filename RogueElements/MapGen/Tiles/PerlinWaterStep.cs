// <copyright file="PerlinWaterStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Generates a random spread of water on the map. This is achieved by generating a heighTContext using Perlin Noise,
    /// then converting all tiles with a height value below a certain threshold to water.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class PerlinWaterStep<T> : WaterStep<T>
        where T : class, ITiledGenContext
    {
        private const int BUFFER_SIZE = 5;

        /// <summary>
        /// Determines how many iterations of Perlin noise to generate the heighTContext with. Higher complexity = higher variation of heights and more natural looking terrain.
        /// </summary>
        private readonly int orderComplexity;

        /// <summary>
        /// Determines the smallest uit of water tiles on the map. 0 = 1x1 tile of water, 1 = 2x2 tile of water, etc.
        /// </summary>
        private readonly int orderSoftness;

        /// <summary>
        /// Decides if the water can paint over floor tiles if the blob itself does not break connectivity
        /// </summary>
        private readonly bool respectFloor;

        /// <summary>
        /// The percent chance of water occurring.
        /// </summary>
        private RandRange waterPercent;

        public PerlinWaterStep(RandRange waterPercent, int complexity, ITile terrain, int softness = default, bool respectFloor = default)
            : base(terrain)
        {
            this.waterPercent = waterPercent;
            this.orderComplexity = complexity;
            this.orderSoftness = softness;
            this.respectFloor = respectFloor;
        }

        public override void Apply(T map)
        {
            int waterPercent = this.waterPercent.Pick(map.Rand);
            if (waterPercent == 0)
                return;

            int depthRange = 0x1 << (this.orderComplexity + this.orderSoftness); // aka, 2 ^ degree
            int minWater = waterPercent * map.Width * map.Height / 100;
            int[][] noise = NoiseGen.PerlinNoise(map.Rand, map.Width, map.Height, this.orderComplexity, this.orderSoftness);
            int[] depthCount = new int[depthRange];
            for (int xx = 0; xx < map.Width; xx++)
            {
                for (int yy = 0; yy < map.Height; yy++)
                    depthCount[noise[xx][yy]]++;
            }

            int waterMark = 0;
            int totalDepths = 0;
            for (int ii = 0; ii < depthCount.Length; ii++)
            {
                if (totalDepths + depthCount[ii] >= minWater)
                {
                    if (totalDepths + depthCount[ii] - minWater < minWater - totalDepths)
                        waterMark++;
                    break;
                }

                totalDepths += depthCount[ii];
                waterMark++;
            }

            if (this.respectFloor)
            {
                this.DrawWhole(map, noise, depthRange, waterMark);
                return;
            }

            while (waterMark > 0)
            {
                bool IsWaterValid(Loc loc)
                {
                    int heightPercent = Math.Min(100, Math.Min(Math.Min(loc.X * 100 / BUFFER_SIZE, loc.Y * 100 / BUFFER_SIZE), Math.Min((map.Width - 1 - loc.X) * 100 / BUFFER_SIZE, (map.Height - 1 - loc.Y) * 100 / BUFFER_SIZE)));
                    int noiseVal = (noise[loc.X][loc.Y] * heightPercent / 100) + (depthRange * (100 - heightPercent) / 100);
                    return noiseVal < waterMark;
                }

                BlobMap blobMap = Detection.DetectBlobs(new Rect(0, 0, map.Width, map.Height), IsWaterValid);

                bool IsMapValid(Loc loc) => map.GetTile(loc).TileEquivalent(map.RoomTerrain);

                int blobIdx = 0;
                bool IsBlobValid(Loc loc)
                {
                    Loc srcLoc = loc + blobMap.Blobs[blobIdx].Bounds.Start;
                    if (!map.CanSetTile(srcLoc, this.Terrain))
                        return false;
                    return blobMap.Map[srcLoc.X][srcLoc.Y] == blobIdx;
                }

                for (; blobIdx < blobMap.Blobs.Count; blobIdx++)
                {
                    Rect blobRect = blobMap.Blobs[blobIdx].Bounds;

                    // pass this into the walkable detection function
                    bool disconnects = Detection.DetectDisconnect(new Rect(0, 0, map.Width, map.Height), IsMapValid, blobRect.Start, blobRect.Size, IsBlobValid, true);

                    // if it's a pass, draw on tile if it's a wall terrain or a room terrain
                    if (!disconnects)
                    {
                        this.DrawBlob(map, blobMap, blobIdx, blobRect.Start, true);
                    }
                    else
                    {
                        // if it's a fail, draw on the tile only if wall terrain
                        this.DrawBlob(map, blobMap, blobIdx, blobRect.Start, false);
                    }
                }

                waterMark -= Math.Max(1, depthRange / 8);
            }
        }

        private void DrawWhole(T map, int[][] noise, int depthRange, int waterMark)
        {
            for (int xx = 0; xx < map.Width; xx++)
            {
                for (int yy = 0; yy < map.Height; yy++)
                {
                    if (!map.GetTile(new Loc(xx, yy)).TileEquivalent(map.WallTerrain))
                        continue;

                    // the last BUFFER_SIZE tiles near the edge gradually multiply the actual noise value by smaller numbers
                    int heightPercent = Math.Min(100, Math.Min(Math.Min(xx * 100 / BUFFER_SIZE, yy * 100 / BUFFER_SIZE), Math.Min((map.Width - 1 - xx) * 100 / BUFFER_SIZE, (map.Height - 1 - yy) * 100 / BUFFER_SIZE)));

                    // interpolate UPWARDS to make it like a bowl
                    int noiseVal = (noise[xx][yy] * heightPercent / 100) + (depthRange * (100 - heightPercent) / 100);

                    if (noiseVal < waterMark)
                        map.SetTile(new Loc(xx, yy), this.Terrain.Copy());
                }
            }
        }
    }
}
