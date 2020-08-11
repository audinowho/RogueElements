// <copyright file="IRoomPlan.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace RogueElements
{
    public interface IRoomPlan
    {
        IRoomGen RoomGen { get; }

        bool Immutable { get; }

        ComponentCollection Components { get; }
    }
}