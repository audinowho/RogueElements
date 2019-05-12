// <copyright file="StairsStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class StairsStep<T, E, F> : GenStep<T>
        where T : class, IPlaceableGenContext<E>, IPlaceableGenContext<F>
    {
        public List<E> Entrance;
        public List<F> Exit;

        public StairsStep()
        {
            Entrance = new List<E>();
            Exit = new List<F>();
        }
        
        public StairsStep(E entrance, F exit) : this()
        {
            Entrance.Add(entrance);
            Exit.Add(exit);
        }

        public override void Apply(T map)
        {

            Loc defaultLoc = new Loc();

            for (int ii = 0; ii < Entrance.Count; ii++)
            {
                Loc start = getOutlet<E>(map);
                if (start == new Loc(-1))
                    start = defaultLoc;
                else
                    defaultLoc = start;
                ((IPlaceableGenContext<E>)map).PlaceItem(start, Entrance[ii]);
                GenContextDebug.DebugProgress("Entrance");
            }

            for (int ii = 0; ii < Exit.Count; ii++)
            {
                Loc end = getOutlet<F>(map);
                if (end == new Loc(-1))
                    end = defaultLoc;
                ((IPlaceableGenContext<F>)map).PlaceItem(end, Exit[ii]);
                GenContextDebug.DebugProgress("Exit");
            }
        }


        private Loc getOutlet<N>(T map)
        {
            List<Loc> tiles = ((IPlaceableGenContext<N>)map).GetAllFreeTiles();

            if (tiles.Count > 0)
                return tiles[map.Rand.Next(tiles.Count)];

            return new Loc(-1);
        }

    }
}
