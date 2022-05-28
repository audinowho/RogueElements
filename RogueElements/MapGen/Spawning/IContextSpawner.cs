// <copyright file="IContextSpawner.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    public interface IContextSpawner
    {
        /// <summary>
        /// The amount of spawns to roll from the spawn tables.
        /// </summary>
        RandRange Amount { get; set; }
    }

    /// <summary>
    /// Spawns items from the map's own spawn tables.
    /// </summary>
    /// <typeparam name="TGenContext"></typeparam>
    /// <typeparam name="TSpawnable"></typeparam>
    [Serializable]
    public class ContextSpawner<TGenContext, TSpawnable> : IStepSpawner<TGenContext, TSpawnable>, IContextSpawner
        where TGenContext : ISpawningGenContext<TSpawnable>
        where TSpawnable : ISpawnable
    {
        public ContextSpawner()
        {
            this.Amount = RandRange.Empty;
        }

        public ContextSpawner(RandRange amount)
        {
            this.Amount = amount;
        }

        public RandRange Amount { get; set; }

        public List<TSpawnable> GetSpawns(TGenContext map)
        {
            int chosenAmount = this.Amount.Pick(map.Rand);
            var results = new List<TSpawnable>();
            for (int ii = 0; ii < chosenAmount; ii++)
            {
                if (!map.Spawner.CanPick)
                    break;
                results.Add(map.Spawner.Pick(map.Rand));
            }

            return results;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", this.GetType().Name, this.Amount.ToString());
        }
    }
}
