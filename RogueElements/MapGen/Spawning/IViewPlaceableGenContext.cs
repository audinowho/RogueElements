// <copyright file="IViewPlaceableGenContext.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    public interface IViewPlaceableGenContext<T> : IPlaceableGenContext<T>
        where T : ISpawnable
    {
        int Count { get; }

        T GetItem(int index);

        Loc GetLoc(int index);
    }
}
