// <copyright file="IPresetMultiRand.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Non-generic interface for preset multi-item random generators.
    /// </summary>
    public interface IPresetMultiRand
    {
        /// <summary>
        /// Gets the list of items to spawn.
        /// </summary>
        IList ToSpawn { get; }

        /// <summary>
        /// Gets a value indicating whether items can be picked.
        /// </summary>
        bool CanPick { get; }

        /// <summary>
        /// Gets the number of items in the spawn list.
        /// </summary>
        int Count { get; }
    }

    /// <summary>
    /// Generates a list of items predefined by the user.
    /// </summary>
    /// <typeparam name="T">The type of items to generate.</typeparam>
    [Serializable]
    public class PresetMultiRand<T> : IMultiRandPicker<T>, IPresetMultiRand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PresetMultiRand{T}"/> class.
        /// </summary>
        public PresetMultiRand()
        {
            this.ToSpawn = new List<IRandPicker<T>>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PresetMultiRand{T}"/> class with the specified pickers.
        /// </summary>
        /// <param name="toSpawn">The pickers to include in the list.</param>
        public PresetMultiRand(params IRandPicker<T>[] toSpawn)
        {
            this.ToSpawn = new List<IRandPicker<T>>(toSpawn);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PresetMultiRand{T}"/> class with preset items.
        /// </summary>
        /// <param name="toSpawn">The items to include in the list.</param>
        public PresetMultiRand(params T[] toSpawn)
        {
            this.ToSpawn = new List<IRandPicker<T>>();
            foreach (T item in toSpawn)
                this.ToSpawn.Add(new PresetPicker<T>(item));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PresetMultiRand{T}"/> class with a list of pickers.
        /// </summary>
        /// <param name="toSpawn">The list of pickers.</param>
        public PresetMultiRand(List<IRandPicker<T>> toSpawn)
        {
            this.ToSpawn = toSpawn;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PresetMultiRand{T}"/> class with a list of items.
        /// </summary>
        /// <param name="toSpawn">The list of items.</param>
        public PresetMultiRand(List<T> toSpawn)
        {
            this.ToSpawn = new List<IRandPicker<T>>();
            foreach (T item in toSpawn)
                this.ToSpawn.Add(new PresetPicker<T>(item));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PresetMultiRand{T}"/> class by copying another instance.
        /// </summary>
        /// <param name="other">The instance to copy.</param>
        protected PresetMultiRand(PresetMultiRand<T> other)
        {
            this.ToSpawn = new List<IRandPicker<T>>(other.ToSpawn);
        }

        /// <summary>
        /// Gets the list of pickers to spawn from.
        /// </summary>
        public List<IRandPicker<T>> ToSpawn { get; }

        /// <inheritdoc/>
        public bool ChangesState => false;

        /// <inheritdoc/>
        public bool CanPick => this.ToSpawn != null;

        /// <inheritdoc/>
        public int Count => this.ToSpawn != null ? this.ToSpawn.Count : 0;

        /// <inheritdoc/>
        IList IPresetMultiRand.ToSpawn => this.ToSpawn;

        /// <inheritdoc/>
        public IMultiRandPicker<T> CopyState() => new PresetMultiRand<T>(this);

        /// <inheritdoc/>
        public List<T> Roll(IRandom rand)
        {
            List<T> result = new List<T>();
            foreach (IRandPicker<T> picker in this.ToSpawn)
            {
                if (picker.CanPick)
                    result.Add(picker.Pick(rand));
            }

            return result;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            if (this.Count == 1)
                return string.Format("{{{0}}}", this.ToSpawn[0].ToString());
            return string.Format("{0}[{1}]", this.GetType().GetFormattedTypeName(), this.Count);
        }
    }
}
