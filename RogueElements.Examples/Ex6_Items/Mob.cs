// <copyright file="Mob.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using RogueElements;

namespace RogueElements.Examples.Ex6_Items
{
    public class Mob : ISpawnable
    {
        public Mob()
        {
        }

        public Mob(int id)
        {
            this.ID = id;
        }

        public Mob(int id, Loc loc)
        {
            this.ID = id;
            this.Loc = loc;
        }

        protected Mob(Mob other)
            : this(other.ID, other.Loc)
        {
        }

        public int ID { get; set; }

        public Loc Loc { get; set; }

        public ISpawnable Copy() => new Mob(this);
    }
}
