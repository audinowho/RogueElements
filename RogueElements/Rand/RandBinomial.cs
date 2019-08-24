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
        /// Adds an amount to the result before returning.
        /// </summary>
        private readonly int offset;

        /// <summary>
        /// The number of trials in the binomial distribution.
        /// </summary>
        private readonly int trials;

        /// <summary>
        /// The chance of an individual event occurring in the binomial distribution.
        /// </summary>
        private readonly int percent;

        public RandBinomial()
        {
        }

        public RandBinomial(int trials, int percent)
        {
            this.trials = trials;
            this.percent = percent;
        }

        public RandBinomial(int trials, int percent, int offset)
            : this(trials, percent)
        {
            this.offset = offset;
        }

        protected RandBinomial(RandBinomial other)
        {
            this.offset = other.offset;
            this.trials = other.trials;
            this.percent = other.percent;
        }

        public bool ChangesState => false;

        public bool CanPick => true;

        public IRandPicker<int> CopyState() => new RandBinomial(this);

        public IEnumerator<int> GetEnumerator()
        {
            for (int ii = 0; ii < this.trials; ii++)
                yield return ii;
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public int Pick(IRandom rand)
        {
            int total = 0;
            for (int ii = 0; ii < this.trials; ii++)
            {
                if (rand.Next(100) < this.percent)
                    total++;
            }

            return this.offset + total;
        }
    }
}
