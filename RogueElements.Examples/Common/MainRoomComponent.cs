// <copyright file="MainRoomComponent.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using RogueElements;

namespace RogueElements.Examples
{
    public class MainRoomComponent : RoomComponent
    {
        public override RoomComponent Clone()
        {
            return new MainRoomComponent();
        }
    }
}