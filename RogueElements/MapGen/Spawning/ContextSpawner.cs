// <copyright file="ContextSpawner.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class ContextSpawner<TGenContext, TSpawnable> : IStepSpawner<TGenContext, TSpawnable>
        where TGenContext : ISpawningGenContext<TSpawnable>
        where TSpawnable : ISpawnable
    {
        private RandRange amount;

        public ContextSpawner(RandRange amount)
        {
            this.amount = amount;
        }

        public List<TSpawnable> GetSpawns(TGenContext map)
        {
            int chosenAmount = this.amount.Pick(map.Rand);
            var results = new List<TSpawnable>();
            for (int ii = 0; ii < chosenAmount; ii++)
            {
                if (!map.Spawner.CanPick)
                    break;
                results.Add(map.Spawner.Pick(map.Rand));
            }

            return results;
        }
    }
}
