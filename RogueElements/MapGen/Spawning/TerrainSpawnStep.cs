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
    /// <typeparam name="TGenContext">The generation context type, which must support placing spawnable entities and tile access.</typeparam>
    /// <typeparam name="TSpawnable">The type of spawnable entity to place on the map.</typeparam>
    [Serializable]
    public class TerrainSpawnStep<TGenContext, TSpawnable> : BaseSpawnStep<TGenContext, TSpawnable>
        where TGenContext : class, IPlaceableGenContext<TSpawnable>, ITiledGenContext
        where TSpawnable : ISpawnable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TerrainSpawnStep{TGenContext, TSpawnable}"/> class.
        /// </summary>
        public TerrainSpawnStep()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TerrainSpawnStep{TGenContext, TSpawnable}"/> class with the specified terrain type.
        /// </summary>
        /// <param name="terrain">The terrain type to spawn objects on.</param>
        public TerrainSpawnStep(ITile terrain)
            : base()
        {
            this.Terrain = terrain;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TerrainSpawnStep{TGenContext, TSpawnable}"/> class with the specified terrain type and spawner.
        /// </summary>
        /// <param name="terrain">The terrain type to spawn objects on.</param>
        /// <param name="spawn">The spawner that generates the list of items to place.</param>
        public TerrainSpawnStep(ITile terrain, IStepSpawner<TGenContext, TSpawnable> spawn)
            : base(spawn)
        {
            this.Terrain = terrain;
        }

        /// <summary>
        /// Gets or sets the terrain type that tiles must match for spawn placement.
        /// </summary>
        public ITile Terrain { get; set; }

        /// <summary>
        /// Distributes spawns by placing each on a randomly selected tile matching the specified terrain.
        /// </summary>
        /// <param name="map">The generation context to place spawns in.</param>
        /// <param name="spawns">The list of spawnable entities to distribute.</param>
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

        /// <inheritdoc/>
        public override string ToString()
        {
            if (this.Spawn == null || this.Terrain == null)
                return string.Format("{0}<{1}>: [EMPTY]", this.GetType().GetFormattedTypeName(), typeof(TSpawnable).Name);
            return string.Format("{0}<{1}>[{2}] Terrain: {3}", this.GetType().GetFormattedTypeName(), typeof(TSpawnable).Name, this.Spawn.ToString(), this.Terrain.ToString());
        }
    }
}
