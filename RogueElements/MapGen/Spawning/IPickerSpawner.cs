// <copyright file="IPickerSpawner.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    public interface IPickerSpawner
    {
        IMultiRandPicker Picker { get; set; }
    }

    /// <summary>
    /// Generates spawnables from a specifically defined IMultiRandPicker.
    /// </summary>
    /// <typeparam name="TGenContext"></typeparam>
    /// <typeparam name="TSpawnable"></typeparam>
    [Serializable]
    public class PickerSpawner<TGenContext, TSpawnable> : IStepSpawner<TGenContext, TSpawnable>, IPickerSpawner
        where TGenContext : IGenContext
        where TSpawnable : ISpawnable
    {
        public PickerSpawner()
        {
        }

        public PickerSpawner(IMultiRandPicker<TSpawnable> picker)
        {
            this.Picker = picker;
        }

        /// <summary>
        /// The IMultiRandPicker that decides the objects to spawn.
        /// </summary>
        public IMultiRandPicker<TSpawnable> Picker { get; set; }

        IMultiRandPicker IPickerSpawner.Picker
        {
            get { return this.Picker; }
            set { this.Picker = (IMultiRandPicker<TSpawnable>)value; }
        }

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

        public override string ToString()
        {
            return string.Format("{0}: {1}", this.GetType().Name, this.Picker.ToString());
        }
    }
}
