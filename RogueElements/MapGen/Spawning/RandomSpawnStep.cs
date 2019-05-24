// <copyright file="RandomSpawnStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
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
    }
}
