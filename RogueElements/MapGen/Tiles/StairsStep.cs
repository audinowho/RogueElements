// <copyright file="StairsStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Places entrance and exit stairs on the floor at random walkable tile locations.
    /// </summary>
    /// <typeparam name="TGenContext">The type of map context that implements placement for entrances and exits.</typeparam>
    /// <typeparam name="TEntrance">The type of entrance to place.</typeparam>
    /// <typeparam name="TExit">The type of exit to place.</typeparam>
    /// <remarks>
    /// This step is not room-conscious and selects random free tiles for stair placement.
    /// </remarks>
    [Serializable]
    public class StairsStep<TGenContext, TEntrance, TExit> : GenStep<TGenContext>
        where TGenContext : class, IPlaceableGenContext<TEntrance>, IPlaceableGenContext<TExit>
        where TEntrance : IEntrance
        where TExit : IExit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StairsStep{TGenContext, TEntrance, TExit}"/> class.
        /// </summary>
        public StairsStep()
        {
            this.Entrance = new List<TEntrance>();
            this.Exit = new List<TExit>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StairsStep{TGenContext, TEntrance, TExit}"/> class with the specified entrance and exit.
        /// </summary>
        /// <param name="entrance">The entrance to place on the map.</param>
        /// <param name="exit">The exit to place on the map.</param>
        public StairsStep(TEntrance entrance, TExit exit)
        {
            this.Entrance = new List<TEntrance> { entrance };
            this.Exit = new List<TExit> { exit };
        }

        /// <summary>
        /// Gets the list of entrances to place on the map.
        /// </summary>
        public List<TEntrance> Entrance { get; }

        /// <summary>
        /// Gets the list of exits to place on the map.
        /// </summary>
        public List<TExit> Exit { get; }

        /// <inheritdoc/>
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
