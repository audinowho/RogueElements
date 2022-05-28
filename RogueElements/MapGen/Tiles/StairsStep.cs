// <copyright file="StairsStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Adds the entrance and exit to the floor.  Is not room-conscious and only picks random tiles.
    /// </summary>
    /// <typeparam name="TGenContext"></typeparam>
    /// <typeparam name="TEntrance"></typeparam>
    /// <typeparam name="TExit"></typeparam>
    [Serializable]
    public class StairsStep<TGenContext, TEntrance, TExit> : GenStep<TGenContext>
        where TGenContext : class, IPlaceableGenContext<TEntrance>, IPlaceableGenContext<TExit>
        where TEntrance : IEntrance
        where TExit : IExit
    {
        public StairsStep()
        {
            this.Entrance = new List<TEntrance>();
            this.Exit = new List<TExit>();
        }

        public StairsStep(TEntrance entrance, TExit exit)
        {
            this.Entrance = new List<TEntrance> { entrance };
            this.Exit = new List<TExit> { exit };
        }

        public List<TEntrance> Entrance { get; }

        public List<TExit> Exit { get; }

        public override void Apply(TGenContext map)
        {
            Loc defaultLoc = Loc.Zero;

            for (int ii = 0; ii < this.Entrance.Count; ii++)
            {
                Loc start = GetOutlet<TEntrance>(map);
                if (start == new Loc(-1))
                    start = defaultLoc;
                else
                    defaultLoc = start;
                ((IPlaceableGenContext<TEntrance>)map).PlaceItem(start, this.Entrance[ii]);
                GenContextDebug.DebugProgress(nameof(this.Entrance));
            }

            for (int ii = 0; ii < this.Exit.Count; ii++)
            {
                Loc end = GetOutlet<TExit>(map);
                if (end == new Loc(-1))
                    end = defaultLoc;
                ((IPlaceableGenContext<TExit>)map).PlaceItem(end, this.Exit[ii]);
                GenContextDebug.DebugProgress(nameof(this.Exit));
            }
        }

        private static Loc GetOutlet<T>(TGenContext map)
            where T : ISpawnable
        {
            List<Loc> tiles = ((IPlaceableGenContext<T>)map).GetAllFreeTiles();

            if (tiles.Count > 0)
                return tiles[map.Rand.Next(tiles.Count)];

            return -Loc.One;
        }
    }
}
