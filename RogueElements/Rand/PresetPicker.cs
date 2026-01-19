// <copyright file="PresetPicker.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Generates an item that is predefined by the user.
    /// </summary>
    /// <typeparam name="T">The type of item to generate.</typeparam>
    [Serializable]
    public class PresetPicker<T> : IRandPicker<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PresetPicker{T}"/> class.
        /// </summary>
        public PresetPicker()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PresetPicker{T}"/> class with a preset item.
        /// </summary>
        /// <param name="toSpawn">The item to return when picked.</param>
        public PresetPicker(T toSpawn)
        {
            this.ToSpawn = toSpawn;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PresetPicker{T}"/> class by copying another instance.
        /// </summary>
        /// <param name="other">The instance to copy.</param>
        protected PresetPicker(PresetPicker<T> other)
        {
            this.ToSpawn = other.ToSpawn;
        }

        /// <summary>
        /// Gets or sets the item to return when picked.
        /// </summary>
        public T ToSpawn { get; set; }

        /// <inheritdoc/>
        public bool ChangesState => false;

        /// <inheritdoc/>
        public bool CanPick => true;

        /// <inheritdoc/>
        public IRandPicker<T> CopyState() => new PresetPicker<T>(this);

        /// <inheritdoc/>
        public IEnumerable<T> EnumerateOutcomes()
        {
            yield return this.ToSpawn;
        }

        /// <inheritdoc/>
        public T Pick(IRandom rand) => this.ToSpawn;

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("{{{0}}}", this.ToSpawn != null ? this.ToSpawn.ToString() : "NULL");
        }
    }
}
