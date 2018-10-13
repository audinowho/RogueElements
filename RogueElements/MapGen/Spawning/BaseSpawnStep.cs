using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public abstract class BaseSpawnStep<T, E> : GenStep<T>
        where T : class, IPlaceableGenContext<E>
    {
        public IStepSpawner<T, E> Spawn;

        public abstract void DistributeSpawns(T map, List<E> spawns);

        public override void Apply(T map)
        {
            List<E> spawns = Spawn.GetSpawns(map);

            if (spawns.Count > 0)
                DistributeSpawns(map, spawns);
        }
    }
}
