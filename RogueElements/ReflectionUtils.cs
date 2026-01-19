// <copyright file="ReflectionUtils.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace RogueElements
{
    /// <summary>
    /// Provides utility methods for reflection operations.
    /// </summary>
    public static class ReflectionUtils
    {
        /// <summary>
        /// Gets a formatted type name without generic arity suffixes.
        /// </summary>
        /// <param name="t">The type.</param>
        /// <returns>The formatted type name (e.g., "List" instead of "List`1").</returns>
        public static string GetFormattedTypeName(this Type t)
        {
            if (t.IsGenericType)
                return t.Name.Substring(0, t.Name.LastIndexOf("`", StringComparison.InvariantCulture));

            return t.Name;
        }
    }
}
