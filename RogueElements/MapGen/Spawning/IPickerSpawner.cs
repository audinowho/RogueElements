// <copyright file="IPickerSpawner.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Provides a non-generic interface for spawners that use a random picker to select entities directly.
    /// </summary>
    /// <seealso cref="PickerSpawner{TGenContext, TSpawnable}"/>
    public interface IPickerSpawner
    {
        /// <summary>
        /// Gets or sets the picker that selects which entities to spawn.
        /// </summary>
        IMultiRandPicker Picker { get; set; }
    }

    /// <summary>
    /// Generates spawnables from a specifically defined IMultiRandPicker.
    /// </summary>
    /// <typeparam name="TGenContext">The generation context type used for randomization.</typeparam>
    /// <typeparam name="TSpawnable">The type of spawnable entity to generate.</typeparam>
    [Serializable]
    public class PickerSpawner<TGenContext, TSpawnable> : IStepSpawner<TGenContext, TSpawnable>, IPickerSpawner
        where TGenContext : IGenContext
        where TSpawnable : ISpawnable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PickerSpawner{TGenContext, TSpawnable}"/> class.
        /// </summary>
        public PickerSpawner()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PickerSpawner{TGenContext, TSpawnable}"/> class with the specified picker.
        /// </summary>
        /// <param name="picker">The picker that selects which entities to spawn.</param>
        public PickerSpawner(IMultiRandPicker<TSpawnable> picker)
        {
            this.Picker = picker;
        }

        /// <summary>
        /// The IMultiRandPicker that decides the objects to spawn.
        /// </summary>
        public IMultiRandPicker<TSpawnable> Picker { get; set; }

        /// <inheritdoc/>
        IMultiRandPicker IPickerSpawner.Picker
        {
            get { return this.Picker; }
            set { this.Picker = (IMultiRandPicker<TSpawnable>)value; }
        }

        /// <summary>
        /// Generates spawns by rolling the picker and copying the selected entities.
        /// </summary>
        /// <param name="map">The generation context used for randomization.</param>
        /// <returns>A list of spawnable entity copies selected from the picker.</returns>
        public List<TSpawnable> GetSpawns(TGenContext map)
        {
            if (this.Picker is null)
                return new List<TSpawnable>();
            IMultiRandPicker<TSpawnable> picker = this.Picker;
            if (picker.ChangesState)
                picker = picker.CopyState();
            List<TSpawnable> results = picker.Roll(map.Rand);
            var copyResults = new List<TSpawnable>();
            foreach (TSpawnable result in results)
                copyResults.Add((TSpawnable)result.Copy());
            return copyResults;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("{0}: {1}", this.GetType().GetFormattedTypeName(), this.Picker.ToString());
        }
    }
}
