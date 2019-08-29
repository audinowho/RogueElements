// <copyright file="Stairs.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using RogueElements;

namespace RogueElements.Examples
{
    public abstract class Stairs : ISpawnable
    {
        protected Stairs()
        {
        }

        protected Stairs(Stairs other)
        {
            this.Loc = other.Loc;
        }

        public Loc Loc { get; set; }

        public abstract ISpawnable Copy();
    }
}
