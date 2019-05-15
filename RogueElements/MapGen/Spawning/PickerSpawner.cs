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
        private readonly IMultiRandPicker<TSpawnable> picker;

        public PickerSpawner(IMultiRandPicker<TSpawnable> picker)
        {
            this.picker = picker;
        }

        public List<TSpawnable> GetSpawns(TGenContext map)
        {
            IMultiRandPicker<TSpawnable> picker = this.picker;
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
