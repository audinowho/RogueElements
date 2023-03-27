// <copyright file="IPresetMultiRand.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;

namespace RogueElements
{
    public interface IPresetMultiRand
    {
        IList ToSpawn { get; }

        bool CanPick { get; }

        int Count { get; }
    }

    /// <summary>
    /// Generates a list of items predefined by the user.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class PresetMultiRand<T> : IMultiRandPicker<T>, IPresetMultiRand
    {
        public PresetMultiRand()
        {
            this.ToSpawn = new List<IRandPicker<T>>();
        }

        public PresetMultiRand(params IRandPicker<T>[] toSpawn)
        {
            this.ToSpawn = new List<IRandPicker<T>>(toSpawn);
        }

        public PresetMultiRand(params T[] toSpawn)
        {
            this.ToSpawn = new List<IRandPicker<T>>();
            foreach (T item in toSpawn)
                this.ToSpawn.Add(new PresetPicker<T>(item));
        }

        public PresetMultiRand(List<IRandPicker<T>> toSpawn)
        {
            this.ToSpawn = toSpawn;
        }

        public PresetMultiRand(List<T> toSpawn)
        {
            this.ToSpawn = new List<IRandPicker<T>>();
            foreach (T item in toSpawn)
                this.ToSpawn.Add(new PresetPicker<T>(item));
        }

        protected PresetMultiRand(PresetMultiRand<T> other)
        {
            this.ToSpawn = new List<IRandPicker<T>>(other.ToSpawn);
        }

        public List<IRandPicker<T>> ToSpawn { get; }

        public bool ChangesState => false;

        public bool CanPick => this.ToSpawn != null;

        public int Count => this.ToSpawn != null ? this.ToSpawn.Count : 0;

        IList IPresetMultiRand.ToSpawn => this.ToSpawn;

        public IMultiRandPicker<T> CopyState() => new PresetMultiRand<T>(this);

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

        public override string ToString()
        {
            if (this.Count == 1)
                return string.Format("{{{0}}}", this.ToSpawn[0].ToString());
            return string.Format("{0}[{1}]", this.GetType().GetFormattedTypeName(), this.Count);
        }
    }
}
