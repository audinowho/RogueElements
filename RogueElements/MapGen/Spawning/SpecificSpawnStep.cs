using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class SpecificSpawnStep<T, E> : GenStep<T>
        where T : class, IPlaceableGenContext<E>
        where E : ISpawnable
    {
        public List<Tuple<E, Loc>> Spawns;

        public SpecificSpawnStep() { }
        public SpecificSpawnStep(List<Tuple<E, Loc>> spawns)
        {
            Spawns = spawns;
        }

        public override void Apply(T map)
        {
            for (int ii = 0; ii < Spawns.Count; ii++)
            {
                E item = Spawns[ii].Item1;
                Loc loc = Spawns[ii].Item2;

                map.PlaceItem(loc, item);
                GenContextDebug.DebugProgress("Placed Object");
            }
        }
    }
}
