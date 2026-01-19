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
    /// <typeparam name="T">The type of items in the bag.</typeparam>
    [Serializable]
    public class RandBag<T> : IRandPicker<T>
    {
        private bool removeOnRoll;

        /// <summary>
        /// Initializes a new instance of the <see cref="RandBag{T}"/> class.
        /// </summary>
        public RandBag()
        {
            this.ToSpawn = new List<T>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RandBag{T}"/> class with specified items.
        /// </summary>
        /// <param name="toSpawn">The items to include in the bag.</param>
        public RandBag(params T[] toSpawn)
        {
            this.ToSpawn = new List<T>(toSpawn);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RandBag{T}"/> class with optional removal on pick.
        /// </summary>
        /// <param name="remove">If true, items are removed after being picked.</param>
        /// <param name="toSpawn">The list of items.</param>
        public RandBag(bool remove, List<T> toSpawn)
        {
            this.removeOnRoll = remove;
            this.ToSpawn = toSpawn;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RandBag{T}"/> class with a list of items.
        /// </summary>
        /// <param name="toSpawn">The list of items.</param>
        public RandBag(List<T> toSpawn)
        {
            this.ToSpawn = toSpawn;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RandBag{T}"/> class by copying another instance.
        /// </summary>
        /// <param name="other">The instance to copy.</param>
        protected RandBag(RandBag<T> other)
        {
            this.ToSpawn = new List<T>(other.ToSpawn);
            this.removeOnRoll = other.removeOnRoll;
        }

        /// <summary>
        /// The items to choose from.
        /// </summary>
        public List<T> ToSpawn { get; }

        /// <summary>
        /// False if this is a bag with replacement.  True if not.
        /// </summary>
        public bool RemoveOnRoll => this.removeOnRoll;

        /// <inheritdoc/>
        public bool ChangesState => this.RemoveOnRoll;

        /// <inheritdoc/>
        public bool CanPick => this.ToSpawn.Count > 0;

        /// <inheritdoc/>
        public IRandPicker<T> CopyState() => new RandBag<T>(this);

        /// <inheritdoc/>
        public IEnumerable<T> EnumerateOutcomes()
        {
            foreach (T spawn in this.ToSpawn)
                yield return spawn;
        }

        /// <inheritdoc/>
        public T Pick(IRandom rand)
        {
            int index = rand.Next(this.ToSpawn.Count);
            T choice = this.ToSpawn[index];
            if (this.RemoveOnRoll)
                this.ToSpawn.RemoveAt(index);
            return choice;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            if (this.ToSpawn.Count == 1)
                return string.Format("{{{0}}}", this.ToSpawn[0].ToString());
            return string.Format("{0}[{1}]", this.GetType().GetFormattedTypeName(), this.ToSpawn.Count);
        }
    }
}
