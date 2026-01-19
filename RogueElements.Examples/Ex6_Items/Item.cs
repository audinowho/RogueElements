// <copyright file="Item.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using RogueElements;

namespace RogueElements.Examples.Ex6_Items
{
    /// <summary>
    /// Represents a spawnable item that can be placed on the map.
    /// Implements ISpawnable to work with RogueElements' spawning system.
    /// </summary>
    /// <remarks>
    /// <para>
    /// ISpawnable is a simple interface with just one requirement: Copy().
    /// This allows the spawning system to create copies of template items
    /// without knowing their concrete type.
    /// </para>
    /// <para>
    /// The spawning pattern works like this:
    /// 1. Define template items in a SpawnList (no location set)
    /// 2. RandomSpawnStep picks items from the list using weighted random
    /// 3. For each pick, Copy() creates a new instance
    /// 4. PlaceItem() sets the location on the copy and adds it to the map
    /// </para>
    /// <para>
    /// In a real game, Item would have properties like:
    /// - Name, Description
    /// - ItemType (weapon, armor, consumable, etc.)
    /// - Stats, effects, durability
    /// </para>
    /// </remarks>
    public class Item : ISpawnable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Item"/> class.
        /// Default constructor required for serialization.
        /// </summary>
        public Item()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Item"/> class with an ID.
        /// Used when defining item templates in a SpawnList.
        /// </summary>
        /// <param name="id">The item's identifier (used as display character in this example).</param>
        public Item(int id)
        {
            this.ID = id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Item"/> class with ID and location.
        /// Used when placing an item on the map.
        /// </summary>
        /// <param name="id">The item's identifier.</param>
        /// <param name="loc">The map location where this item is placed.</param>
        public Item(int id, Loc loc)
        {
            this.ID = id;
            this.Loc = loc;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Item"/> class as a copy.
        /// Protected to encourage using Copy() method.
        /// </summary>
        /// <param name="other">The item to copy.</param>
        protected Item(Item other)
            : this(other.ID, other.Loc)
        {
        }

        /// <summary>
        /// Gets or sets the item's identifier.
        /// In this example, the ID is used as the ASCII display character.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the item's map location.
        /// Set when the item is placed via PlaceItem().
        /// </summary>
        public Loc Loc { get; set; }

        /// <summary>
        /// Creates a copy of this item for spawning.
        /// </summary>
        /// <returns>A new Item instance with the same properties.</returns>
        /// <remarks>
        /// This is the ISpawnable interface requirement. The spawning system
        /// calls this to create instances from templates without knowing the
        /// concrete type. The copy constructor handles the actual copying.
        /// </remarks>
        public ISpawnable Copy() => new Item(this);
    }
}
