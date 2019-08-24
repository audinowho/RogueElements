// <copyright file="StairsUp.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using RogueElements;

namespace RogueElements.Examples
{
    public class StairsUp : Stairs, IEntrance
    {
        public StairsUp()
            : base()
        {
        }

        protected StairsUp(StairsUp other)
            : base(other)
        {
        }

        public override ISpawnable Copy() => new StairsUp(this);
    }
}