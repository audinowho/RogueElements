// <copyright file="RandBinomial.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Generates a random number in a binomial distribution.
    /// </summary>
    [Serializable]
    public class RandBinomial : IRandPicker<int>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RandBinomial"/> class.
        /// </summary>
        public RandBinomial()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RandBinomial"/> class with trial count and probability.
        /// </summary>
        /// <param name="trials">The number of trials.</param>
        /// <param name="percent">The probability percentage (0-100) for each trial.</param>
        public RandBinomial(int trials, int percent)
        {
            this.Trials = trials;
            this.Percent = percent;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RandBinomial"/> class with trial count, probability, and offset.
        /// </summary>
        /// <param name="trials">The number of trials.</param>
        /// <param name="percent">The probability percentage (0-100) for each trial.</param>
        /// <param name="offset">The value to add to the result.</param>
        public RandBinomial(int trials, int percent, int offset)
            : this(trials, percent)
        {
            this.Offset = offset;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RandBinomial"/> class by copying another instance.
        /// </summary>
        /// <param name="other">The instance to copy.</param>
        protected RandBinomial(RandBinomial other)
        {
            this.Offset = other.Offset;
            this.Trials = other.Trials;
            this.Percent = other.Percent;
        }

        /// <summary>
        /// Adds an amount to the result before returning.
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// The number of trials in the binomial distribution.
        /// </summary>
        public int Trials { get; set; }

        /// <summary>
        /// The chance of an individual event occurring in the binomial distribution.
        /// </summary>
        public int Percent { get; set; }

        /// <inheritdoc/>
        public bool ChangesState => false;

        /// <inheritdoc/>
        public bool CanPick => true;

        /// <inheritdoc/>
        public IRandPicker<int> CopyState() => new RandBinomial(this);

        /// <inheritdoc/>
        public IEnumerable<int> EnumerateOutcomes()
        {
            for (int ii = 0; ii < this.Trials; ii++)
                yield return ii;
        }

        /// <inheritdoc/>
        public int Pick(IRandom rand)
        {
            int total = 0;
            for (int ii = 0; ii < this.Trials; ii++)
            {
                if (rand.Next(100) < this.Percent)
                    total++;
            }

            return this.Offset + total;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("{0}+{1}%x{2}", this.Offset, this.Percent, this.Trials);
        }
    }
}
