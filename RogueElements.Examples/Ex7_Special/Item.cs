// <copyright file="Item.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using RogueElements;

namespace RogueElements.Examples.Ex7_Special
{
    public class Item : ISpawnable
    {
        public Item()
        {
        }

        public Item(int id)
        {
            this.ID = id;
        }

        public Item(int id, Loc loc)
        {
            this.ID = id;
            this.Loc = loc;
        }

        protected Item(Item other)
            : this(other.ID, other.Loc)
        {
        }

        public int ID { get; set; }

        public Loc Loc { get; set; }

        public ISpawnable Copy() => new Item(this);
    }
}
