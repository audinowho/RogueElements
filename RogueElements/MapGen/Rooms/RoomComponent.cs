// <copyright file="RoomComponent.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
using System;

namespace RogueElements
{
    [Serializable]
    public abstract class RoomComponent
    {
        public abstract RoomComponent Clone();
    }
}
