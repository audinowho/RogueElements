using System.Collections.Generic;
using System;

namespace RogueElements
{
    /// <summary>
    /// Contains data about which cells a room occupies in a GridFloorPlan.
    /// </summary>
    public class GridHallPlan
    {
        public List<IPermissiveRoomGen> Gens;

        public IPermissiveRoomGen MainGen
        {
            get
            {
                if (Gens.Count > 0)
                    return Gens[0];
                else
                    return null;
            }
        }

        public GridHallPlan()
        {
            Gens = new List<IPermissiveRoomGen>();
        }

        public void SetGen(IPermissiveRoomGen roomGen)
        {
            Gens = new List<IPermissiveRoomGen>();
            if (roomGen != null)
                Gens.Add(roomGen);
        }
    }
    
}
