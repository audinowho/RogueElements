// <copyright file="ReflectionUtils.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace RogueElements
{
    public static class ReflectionUtils
    {
        public static string GetFormattedTypeName(this Type t)
        {
            if (t.IsGenericType)
                return t.Name.Substring(0, t.Name.LastIndexOf("`", StringComparison.InvariantCulture));

            return t.Name;
        }
    }
}
