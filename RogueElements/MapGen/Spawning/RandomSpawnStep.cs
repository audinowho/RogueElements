// <copyright file="RandomSpawnStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Spawns objects on randomly chosen tiles.
    /// The tile is chosen from the set of tiles where the object is allowed to be placed.
    /// </summary>
    /// <typeparam name="TGenContext"></typeparam>
    /// <typeparam name="TSpawnable"></typeparam>
    [Serializable]
    public class RandomSpawnStep<TGenContext, TSpawnable> : BaseSpawnStep<TGenContext, TSpawnable>
        where TGenContext : class, IPlaceableGenContext<TSpawnable>
        where TSpawnable : ISpawnable
    {
        public RandomSpawnStep()
            : base()
        {
        }

        public RandomSpawnStep(IStepSpawner<TGenContext, TSpawnable> spawn)
            : base(spawn)
        {
        }

        public override void DistributeSpawns(TGenContext map, List<TSpawnable> spawns)
        {
            List<Loc> freeTiles = map.GetAllFreeTiles();

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
            return string.Format("{0}<{1}>", this.GetType().Name, typeof(TSpawnable).Name);
        }
    }
}
