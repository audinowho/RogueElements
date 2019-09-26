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
        private readonly IRandPicker<T> spawner;
        private readonly IRandPicker<int> amountSpawner;

        public LoopedRand()
        {
        }

        public LoopedRand(IRandPicker<T> spawner, IRandPicker<int> amountSpawner)
        {
            this.spawner = spawner;
            this.amountSpawner = amountSpawner;
        }

        protected LoopedRand(LoopedRand<T> other)
        {
            this.spawner = other.spawner.CopyState();
            this.amountSpawner = other.amountSpawner.CopyState();
        }

        public bool ChangesState => this.spawner.ChangesState || this.amountSpawner.ChangesState;

        public bool CanPick => this.amountSpawner.CanPick;

        public IMultiRandPicker<T> CopyState() => new LoopedRand<T>(this);

        public List<T> Roll(IRandom rand)
        {
            List<T> result = new List<T>();
            int amount = this.amountSpawner.Pick(rand);
            for (int ii = 0; ii < amount; ii++)
            {
                if (!this.spawner.CanPick)
                    break;
                result.Add(this.spawner.Pick(rand));
            }

            return result;
        }
    }
}
