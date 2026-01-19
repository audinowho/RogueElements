// <copyright file="Mob.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using RogueElements;

namespace RogueElements.Examples.Ex6_Items
{
    /// <summary>
    /// Represents a spawnable mob (monster/NPC) that can be placed on the map.
    /// Implements ISpawnable using the same pattern as Item.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Mobs demonstrate that the ISpawnable/IPlaceableGenContext pattern
    /// works identically for any entity type. The same RandomSpawnStep
    /// and SpawnList classes work with mobs just as they do with items.
    /// </para>
    /// <para>
    /// The key insight: RogueElements separates WHAT gets spawned (ISpawnable)
    /// from WHERE it can go (IPlaceableGenContext) from HOW MANY spawn
    /// (LoopedRand/RandRange). This separation allows mixing and matching
    /// different spawn logic, placement rules, and quantity calculations.
    /// </para>
    /// <para>
    /// In a real game, Mob would have properties like:
    /// - Name, AI type
    /// - Health, attack, defense
    /// - Special abilities, drop tables
    /// - Movement speed, vision range
    /// </para>
    /// </remarks>
    public class Mob : ISpawnable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Mob"/> class.
        /// Default constructor required for serialization.
        /// </summary>
        public Mob()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Mob"/> class with an ID.
        /// Used when defining mob templates in a SpawnList.
        /// </summary>
        /// <param name="id">The mob's identifier (used as display character in this example).</param>
        public Mob(int id)
        {
            this.ID = id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Mob"/> class with ID and location.
        /// Used when placing a mob on the map.
        /// </summary>
        /// <param name="id">The mob's identifier.</param>
        /// <param name="loc">The map location where this mob is placed.</param>
        public Mob(int id, Loc loc)
        {
            this.ID = id;
            this.Loc = loc;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Mob"/> class as a copy.
        /// Protected to encourage using Copy() method.
        /// </summary>
        /// <param name="other">The mob to copy.</param>
        protected Mob(Mob other)
            : this(other.ID, other.Loc)
        {
        }

        /// <summary>
        /// Gets or sets the mob's identifier.
        /// In this example, the ID is used as the ASCII display character
        /// (e.g., 'r' for rat, 'D' for dragon).
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the mob's map location.
        /// Set when the mob is placed via PlaceItem().
        /// </summary>
        public Loc Loc { get; set; }

        /// <summary>
        /// Creates a copy of this mob for spawning.
        /// </summary>
        /// <returns>A new Mob instance with the same properties.</returns>
        /// <remarks>
        /// Identical pattern to Item.Copy(). The spawning system
        /// doesn't care about the concrete type - it just calls Copy()
        /// through the ISpawnable interface.
        /// </remarks>
        public ISpawnable Copy() => new Mob(this);
    }
}
