// <copyright file="StairsDown.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using RogueElements;

namespace RogueElements.Examples
{
    public class StairsDown : Stairs, IExit
    {
        public StairsDown()
            : base()
        {
        }

        protected StairsDown(StairsDown other)
            : base(other)
        {
        }

        public override ISpawnable Copy() => new StairsDown(this);
    }
}