// <copyright file="RandGenStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Initializes a map of Width x Height tiles.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class RandGenStep<T> : GenStep<T>
        where T : class, IGenContext
    {
        public RandGenStep()
        {
        }

        public IMultiRandPicker<GenStep<T>> Spawns { get; set; }

        public override void Apply(T map)
        {
            List<GenStep<T>> steps = this.Spawns.Roll(map.Rand);
            foreach (GenStep<T> step in steps)
                step.Apply(map);
        }

        public override string ToString()
        {
            if (this.Spawns == null)
                return string.Format("{0}", this.GetType().GetFormattedTypeName());

            return string.Format("{0}[{1}]", this.GetType().GetFormattedTypeName(), this.Spawns.ToString());
        }
    }
}
