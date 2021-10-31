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
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class PresetPicker<T> : IRandPicker<T>
    {
        public PresetPicker()
        {
        }

        public PresetPicker(T toSpawn)
        {
            this.ToSpawn = toSpawn;
        }

        protected PresetPicker(PresetPicker<T> other)
        {
            this.ToSpawn = other.ToSpawn;
        }

        public T ToSpawn { get; set; }

        public bool ChangesState => false;

        public bool CanPick => true;

        public IRandPicker<T> CopyState() => new PresetPicker<T>(this);

        public IEnumerable<T> EnumerateOutcomes()
        {
            yield return this.ToSpawn;
        }

        public T Pick(IRandom rand) => this.ToSpawn;

        public override string ToString()
        {
            return string.Format("{0}[{1}]", this.GetType().Name, this.ToSpawn == null ? this.ToSpawn.ToString() : "NULL");
        }
    }
}
