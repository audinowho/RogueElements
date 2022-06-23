// <copyright file="TerrainSpawnStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Spawns objects randomly on tiles of a specific terrain.
    /// </summary>
    /// <typeparam name="TGenContext"></typeparam>
    /// <typeparam name="TSpawnable"></typeparam>
    [Serializable]
    public class TerrainSpawnStep<TGenContext, TSpawnable> : BaseSpawnStep<TGenContext, TSpawnable>
        where TGenContext : class, IPlaceableGenContext<TSpawnable>, ITiledGenContext
        where TSpawnable : ISpawnable
    {
        public TerrainSpawnStep()
            : base()
        {
        }

        public TerrainSpawnStep(ITile terrain)
            : base()
        {
            this.Terrain = terrain;
        }

        public TerrainSpawnStep(ITile terrain, IStepSpawner<TGenContext, TSpawnable> spawn)
            : base(spawn)
        {
            this.Terrain = terrain;
        }

        /// <summary>
        /// The type of tile to spawn in.
        /// </summary>
        public ITile Terrain { get; set; }

        public override void DistributeSpawns(TGenContext map, List<TSpawnable> spawns)
        {
            List<Loc> freeTiles = new List<Loc>();

            for (int xx = 0; xx < map.Width; xx++)
            {
                for (int yy = 0; yy < map.Height; yy++)
                {
                    ITile tile = map.GetTile(new Loc(xx, yy));

                    if (this.Terrain.TileEquivalent(tile))
                        freeTiles.Add(new Loc(xx, yy));
                }
            }

            for (int ii = 0; ii < spawns.Count && freeTiles.Count > 0; ii++)
            {
                TSpawnable item = spawns[ii];

                int randIndex = map.Rand.Next(freeTiles.Count);
                map.PlaceItem(freeTiles[randIndex], item);
                freeTiles.RemoveAt(randIndex);
                GenContextDebug.DebugProgress("Placed Object");
            }
        }

        public override string ToString()
        {
            return string.Format("{0}<{1}>: Tile:{2}", this.GetType().Name, typeof(TSpawnable).Name, this.Terrain.ToString());
        }
    }
}
