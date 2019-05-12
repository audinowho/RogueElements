// <copyright file="PresetMultiRand.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class PresetMultiRand<T> : IMultiRandPicker<T>
    {
        public List<T> ToSpawn;
        public bool ChangesState { get { return false; } }
        public bool CanPick { get { return ToSpawn != null; } }

        public PresetMultiRand() { ToSpawn = new List<T>(); }
        public PresetMultiRand(params T[] toSpawn) : this() { ToSpawn.AddRange(toSpawn); }
        public PresetMultiRand(List<T> toSpawn) { ToSpawn = toSpawn; }
        protected PresetMultiRand(PresetMultiRand<T> other) : this()
        {
            ToSpawn.AddRange(other.ToSpawn);
        }
        public IMultiRandPicker<T> CopyState() { return new PresetMultiRand<T>(this); }

        public List<T> Roll(IRandom rand)
        {
            List<T> result = new List<T>();
            result.AddRange(ToSpawn);
            return result;
        }
    }
    
}
