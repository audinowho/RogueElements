// <copyright file="ExampleCreationStrategy.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using RogueSharp;
using RogueSharp.MapCreation;

namespace RogueElements.Examples.Ex8_Integration
{
    public class ExampleCreationStrategy<T> : IMapCreationStrategy<T>
        where T : Map, new()
    {
        public ExampleCreationStrategy()
        {
            this.Layout = new MapGen<MapGenContext>();
        }

        public ulong Seed { get; set; }

        public MapGen<MapGenContext> Layout { get; set; }

        /// <summary>
        /// Creates a new IMap of the specified type.
        /// </summary>
        /// <returns>An IMap of the specified type</returns>
        public T CreateMap()
        {
            MapGenContext context = this.Layout.GenMap(MathUtils.Rand.NextUInt64());

            return (T)context.Map;
        }
    }
}
