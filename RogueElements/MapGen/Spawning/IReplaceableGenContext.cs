// <copyright file="IReplaceableGenContext.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    public interface IReplaceableGenContext<T> : IViewPlaceableGenContext<T>
        where T : ISpawnable
    {
        void SetItem(int index, T item);

        void RemoveItemAt(int index);
    }
}
