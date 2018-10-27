using System;
using System.Collections.Generic;
using RogueSharp;
using RogueSharp.MapCreation;

namespace RogueElements.Examples.Ex7_Integration
{
    public class ExampleCreationStrategy<T> : IMapCreationStrategy<T> where T : Map, new()
    {
        public ulong Seed { get; set; }
        public MapGen<MapGenContext> Layout;

        public ExampleCreationStrategy()
        {
            Layout = new MapGen<MapGenContext>();
        }


        /// <summary>
        /// Creates a new IMap of the specified type.
        /// </summary>
        /// <returns>An IMap of the specified type</returns>
        public T CreateMap()
        {
            MapGenContext context = Layout.GenMap(MathUtils.Rand.NextUInt64());

            return (T)context.Map;
        }
    }
}
