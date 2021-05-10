// <copyright file="StepSpawner.cs" company="Audino">
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
    public class StepSpawner<TGenContext, TSpawnable> : IStepSpawner<TGenContext, TSpawnable>
        where TGenContext : IGenContext
        where TSpawnable : ISpawnable
    {
        public StepSpawner()
        {
        }

        public StepSpawner(IMultiRandPicker<IStepSpawner<TGenContext, TSpawnable>> picker)
        {
            this.Picker = picker;
        }

        public IMultiRandPicker<IStepSpawner<TGenContext, TSpawnable>> Picker { get; set; }

        public List<TSpawnable> GetSpawns(TGenContext map)
        {
            if (this.Picker is null)
                return new List<TSpawnable>();
            IMultiRandPicker<IStepSpawner<TGenContext, TSpawnable>> picker = this.Picker;
            if (picker.ChangesState)
                picker = picker.CopyState();
            List<IStepSpawner<TGenContext, TSpawnable>> resultPickers = picker.Roll(map.Rand);
            List<TSpawnable> copyResults = new List<TSpawnable>();
            foreach (IStepSpawner<TGenContext, TSpawnable> resultPicker in resultPickers)
            {
                List<TSpawnable> results = resultPicker.GetSpawns(map);
                foreach (TSpawnable result in results)
                    copyResults.Add((TSpawnable)result.Copy());
            }

            return copyResults;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", this.GetType().Name, this.Picker.ToString());
        }
    }
}
