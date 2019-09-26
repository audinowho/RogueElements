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
        }

        public List<IPermissiveRoomGen> Gens { get; set; }

        public IPermissiveRoomGen MainGen => this.Gens.Count > 0 ? this.Gens[0] : null;

        public void SetGen(IPermissiveRoomGen roomGen)
        {
            this.Gens = new List<IPermissiveRoomGen>();
            if (roomGen != null)
                this.Gens.Add(roomGen);
        }
    }
}
