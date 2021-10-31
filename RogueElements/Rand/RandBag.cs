// <copyright file="RandBag.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Selects an item randomly from a list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class RandBag<T> : IRandPicker<T>
    {
        public RandBag()
        {
            this.ToSpawn = new List<T>();
        }

        public RandBag(params T[] toSpawn)
        {
            this.ToSpawn = new List<T>(toSpawn);
        }

        public RandBag(List<T> toSpawn)
        {
            this.ToSpawn = toSpawn;
        }

        protected RandBag(RandBag<T> other)
        {
            this.ToSpawn = new List<T>(other.ToSpawn);
            this.RemoveOnRoll = other.RemoveOnRoll;
        }

        /// <summary>
        /// The items to choose from.
        /// </summary>
        public List<T> ToSpawn { get; }

        /// <summary>
        /// False if this is a bag with replacement.  True if not.
        /// </summary>
        public bool RemoveOnRoll { get; set; }

        public bool ChangesState => this.RemoveOnRoll;

        public bool CanPick => this.ToSpawn.Count > 0;

        public IRandPicker<T> CopyState() => new RandBag<T>(this);

        public IEnumerable<T> EnumerateOutcomes()
        {
            foreach (T spawn in this.ToSpawn)
                yield return spawn;
        }

        public T Pick(IRandom rand)
        {
            int index = rand.Next(this.ToSpawn.Count);
            T choice = this.ToSpawn[index];
            if (this.RemoveOnRoll)
                this.ToSpawn.RemoveAt(index);
            return choice;
        }

        public override string ToString()
        {
            return string.Format("{0}[{1}]", this.GetType().Name, this.ToSpawn.Count);
        }
    }
}
