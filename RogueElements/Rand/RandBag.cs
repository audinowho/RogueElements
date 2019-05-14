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
        /// <summary>
        /// The items to choose from.
        /// </summary>
        public List<T> ToSpawn;

        /// <summary>
        /// False if this is a bag with replacement.  True if not.
        /// </summary>
        public bool RemoveOnRoll;

        public bool ChangesState { get { return RemoveOnRoll; } }
        public bool CanPick { get { return ToSpawn.Count > 0; } }

        public RandBag() { ToSpawn = new List<T>(); }
        public RandBag(params T[] toSpawn) : this() { ToSpawn.AddRange(toSpawn); }
        public RandBag(List<T> toSpawn) { ToSpawn = toSpawn; }
        protected RandBag(RandBag<T> other) : this()
        {
            ToSpawn.AddRange(other.ToSpawn);
            RemoveOnRoll = other.RemoveOnRoll;
        }
        public IRandPicker<T> CopyState() { return new RandBag<T>(this); }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (T element in ToSpawn)
                yield return element;
        }
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        public T Pick(IRandom rand)
        {
            int index = rand.Next(ToSpawn.Count);
            T choice = ToSpawn[index];
            if (RemoveOnRoll)
                ToSpawn.RemoveAt(index);
            return choice;
        }
    }

}
