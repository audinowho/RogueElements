// <copyright file="INoise.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Runtime;
using System.Runtime.CompilerServices;

namespace RogueElements
{
    public interface INoise
    {
        ulong FirstSeed { get; }

        int GetInt(ulong position);

        int GetInt(ulong position, int maxValue);

        int GetInt(ulong position, int minValue, int maxValue);

        ulong GetUInt64(ulong position);

        ulong Get2DUInt64(ulong x, ulong y);

        double GetDouble(ulong position);
    }
}
