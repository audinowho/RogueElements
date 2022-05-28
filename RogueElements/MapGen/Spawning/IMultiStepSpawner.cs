// <copyright file="IMultiStepSpawner.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    public interface IMultiStepSpawner
    {
        IMultiRandPicker Picker { get; set; }
    }

    /// <summary>
    /// Randomly chooses an IStepSpawner from a spawner of spawners, then generates the objects from the chosen IStepSpawner.
    /// </summary>
    /// <typeparam name="TGenContext"></typeparam>
    /// <typeparam name="TSpawnable"></typeparam>
    [Serializable]
    public class MultiStepSpawner<TGenContext, TSpawnable> : IStepSpawner<TGenContext, TSpawnable>, IMultiStepSpawner
        where TGenContext : IGenContext
        where TSpawnable : ISpawnable
    {
        public MultiStepSpawner()
        {
        }

        public MultiStepSpawner(IMultiRandPicker<IStepSpawner<TGenContext, TSpawnable>> picker)
        {
            this.Picker = picker;
        }

        /// <summary>
        /// The spawner of spawners.
        /// </summary>
        public IMultiRandPicker<IStepSpawner<TGenContext, TSpawnable>> Picker { get; set; }

        IMultiRandPicker IMultiStepSpawner.Picker
        {
            get { return this.Picker; }
            set { this.Picker = (IMultiRandPicker<IStepSpawner<TGenContext, TSpawnable>>)value; }
        }

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
