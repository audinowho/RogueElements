// <copyright file="PickerSpawner.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Geenrates spawnables from a specifically defined IMultiRandPicker.
    /// </summary>
    /// <typeparam name="TGenContext"></typeparam>
    /// <typeparam name="TSpawnable"></typeparam>
    [Serializable]
    public class PickerSpawner<TGenContext, TSpawnable> : IStepSpawner<TGenContext, TSpawnable>
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

        public IMultiRandPicker<TSpawnable> Picker { get; set; }

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
    }
}
