// <copyright file="GenStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public abstract class GenStep<T> : IGenStep
        where T : class, IGenContext
    {
        //change activemap into an interface that supports tile, mob, and item modification
        public abstract void Apply(T map);

        public bool CanApply(IGenContext context)
        {
            if (!(context is T))
                return false;
            return true;
        }

        public void Apply(IGenContext context)
        {
            if (!(context is T map))
                return;
            Apply(map);
        }
    }

    public interface IGenStep
    {
        bool CanApply(IGenContext context);
        void Apply(IGenContext context);
    }
}
