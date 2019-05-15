// <copyright file="SpecificSpawnStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class SpecificSpawnStep<TGenContext, TSpawnable> : GenStep<TGenContext>
        where TGenContext : class, IPlaceableGenContext<TSpawnable>
        where TSpawnable : ISpawnable
    {
        public SpecificSpawnStep(List<(TSpawnable Item, Loc Loc)> spawns)
        {
            this.Spawns = spawns;
        }

        public List<(TSpawnable Item, Loc Loc)> Spawns { get; }

        public override void Apply(TGenContext map)
        {
            foreach ((TSpawnable item, Loc loc) in this.Spawns)
            {
                map.PlaceItem(loc, item);
                GenContextDebug.DebugProgress("Placed Object");
            }
        }
    }
}
