// <copyright file="TestComponent.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RogueElements.Tests
{
    public class TestComponent : RoomComponent
    {
        public override RoomComponent Clone()
        {
            return new TestComponent();
        }
    }
}