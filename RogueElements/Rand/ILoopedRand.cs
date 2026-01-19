// <copyright file="ILoopedRand.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Non-generic interface for looped random generators.
    /// </summary>
    public interface ILoopedRand
    {
        /// <summary>
        /// Gets or sets the picker that determines how many items to generate.
        /// </summary>
        IRandPicker<int> AmountSpawner { get; set; }
    }

    /// <summary>
    /// Generates a list of items by repeatedly calling an IRandPicker
    /// </summary>
    /// <typeparam name="T">The type of items to generate.</typeparam>
    [Serializable]
    public class LoopedRand<T> : IMultiRandPicker<T>, IRandPicker, ILoopedRand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoopedRand{T}"/> class.
        /// </summary>
        public LoopedRand()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoopedRand{T}"/> class with a spawner and amount picker.
        /// </summary>
        /// <param name="spawner">The picker to use for generating each item.</param>
        /// <param name="amountSpawner">The picker that determines how many items to generate.</param>
        public LoopedRand(IRandPicker<T> spawner, IRandPicker<int> amountSpawner)
        {
            this.Spawner = spawner;
            this.AmountSpawner = amountSpawner;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoopedRand{T}"/> class by copying another instance.
        /// </summary>
        /// <param name="other">The instance to copy.</param>
        protected LoopedRand(LoopedRand<T> other)
        {
            this.Spawner = other.Spawner.CopyState();
            this.AmountSpawner = other.AmountSpawner.CopyState();
        }

        /// <inheritdoc/>
        public bool ChangesState => this.Spawner.ChangesState || this.AmountSpawner.ChangesState;

        /// <summary>
        /// Gets or sets the picker used to generate each item.
        /// </summary>
        public IRandPicker<T> Spawner { get; set; }

        /// <inheritdoc/>
        public IRandPicker<int> AmountSpawner { get; set; }

        /// <inheritdoc/>
        public bool CanPick => this.AmountSpawner.CanPick;

        /// <inheritdoc/>
        public IMultiRandPicker<T> CopyState() => new LoopedRand<T>(this);

        /// <inheritdoc/>
        public List<T> Roll(IRandom rand)
        {
            List<T> result = new List<T>();
            int amount = this.AmountSpawner.Pick(rand);
            for (int ii = 0; ii < amount; ii++)
            {
                if (!this.Spawner.CanPick)
                    break;
                result.Add(this.Spawner.Pick(rand));
            }

            return result;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            if (this.AmountSpawner == null)
                return string.Format("{0}[EMPTY]", this.GetType().GetFormattedTypeName());
            return string.Format("{0}[{1}]", this.GetType().GetFormattedTypeName(), this.AmountSpawner.ToString());
        }
    }
}
