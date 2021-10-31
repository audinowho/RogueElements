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
        public RandBinomial()
        {
        }

        public RandBinomial(int trials, int percent)
        {
            this.Trials = trials;
            this.Percent = percent;
        }

        public RandBinomial(int trials, int percent, int offset)
            : this(trials, percent)
        {
            this.Offset = offset;
        }

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

        public bool ChangesState => false;

        public bool CanPick => true;

        public IRandPicker<int> CopyState() => new RandBinomial(this);

        public IEnumerable<int> EnumerateOutcomes()
        {
            for (int ii = 0; ii < this.Trials; ii++)
                yield return ii;
        }

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
    }
}
