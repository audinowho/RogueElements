// <copyright file="GridHallPlan.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Contains data about which cells a room occupies in a GridFloorPlan.
    /// </summary>
    public class GridHallPlan
    {
        public GridHallPlan()
        {
            this.Gens = new List<IPermissiveRoomGen>();
            this.Components = new ComponentCollection();
        }

        public List<IPermissiveRoomGen> Gens { get; set; }

        // TODO: 1 gen is needed before bounds calc, 2 gens are needed after that phase.
        // Find a better way to split them
        // Maybe just turn the containing array into a 2D array
        public IPermissiveRoomGen MainGen => this.Gens.Count > 0 ? this.Gens[0] : null;

        public ComponentCollection Components { get; private set; }

        public void SetGen(IPermissiveRoomGen roomGen, ComponentCollection components)
        {
            this.Gens = new List<IPermissiveRoomGen>();
            if (roomGen != null)
                this.Gens.Add(roomGen);
            this.Components = components;
        }
    }
}
