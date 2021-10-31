// <copyright file="LoopedRand.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Generates a list of items by repeatedly calling an IRandPicker
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class LoopedRand<T> : IMultiRandPicker<T>
    {
        public LoopedRand()
        {
        }

        public LoopedRand(IRandPicker<T> spawner, IRandPicker<int> amountSpawner)
        {
            this.Spawner = spawner;
            this.AmountSpawner = amountSpawner;
        }

        protected LoopedRand(LoopedRand<T> other)
        {
            this.Spawner = other.Spawner.CopyState();
            this.AmountSpawner = other.AmountSpawner.CopyState();
        }

        public bool ChangesState => this.Spawner.ChangesState || this.AmountSpawner.ChangesState;

        public IRandPicker<T> Spawner { get; set; }

        public IRandPicker<int> AmountSpawner { get; set; }

        public bool CanPick => this.AmountSpawner.CanPick;

        public IMultiRandPicker<T> CopyState() => new LoopedRand<T>(this);

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
    }
}
