// <copyright file="IGenContext.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    public interface IGenContext
    {
        IRandom Rand { get; }

        void InitSeed(ulong seed);

        void FinishGen();
    }
}
