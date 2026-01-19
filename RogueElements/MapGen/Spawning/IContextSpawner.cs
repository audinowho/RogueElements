// <copyright file="IContextSpawner.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Provides a non-generic interface for spawners that draw from the map's own spawn tables.
    /// </summary>
    /// <seealso cref="ContextSpawner{TGenContext, TSpawnable}"/>
    public interface IContextSpawner
    {
        /// <summary>
        /// Gets or sets the amount of spawns to roll from the spawn tables.
        /// </summary>
        RandRange Amount { get; set; }
    }

    /// <summary>
    /// Spawns items from the map's own spawn tables.
    /// </summary>
    /// <typeparam name="TGenContext">The generation context type, which must provide spawn tables.</typeparam>
    /// <typeparam name="TSpawnable">The type of spawnable entity to generate.</typeparam>
    [Serializable]
    public class ContextSpawner<TGenContext, TSpawnable> : IStepSpawner<TGenContext, TSpawnable>, IContextSpawner
        where TGenContext : ISpawningGenContext<TSpawnable>
        where TSpawnable : ISpawnable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContextSpawner{TGenContext, TSpawnable}"/> class.
        /// </summary>
        public ContextSpawner()
        {
            this.Amount = RandRange.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextSpawner{TGenContext, TSpawnable}"/> class with the specified amount.
        /// </summary>
        /// <param name="amount">The range of spawns to generate.</param>
        public ContextSpawner(RandRange amount)
        {
            this.Amount = amount;
        }

        /// <inheritdoc/>
        public RandRange Amount { get; set; }

        /// <summary>
        /// Generates spawns by picking from the map's own spawn tables.
        /// </summary>
        /// <param name="map">The generation context providing the spawn tables.</param>
        /// <returns>A list of spawnable entities picked from the context's spawner.</returns>
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

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("{0}[{1}]", this.GetType().GetFormattedTypeName(), this.Amount.ToString());
        }
    }
}
