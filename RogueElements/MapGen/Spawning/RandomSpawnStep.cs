using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class RandomSpawnStep<T, E> : BaseSpawnStep<T, E>
        where T : class, IPlaceableGenContext<E>
    {
        public RandomSpawnStep() { }

        public RandomSpawnStep(IStepSpawner<T, E> spawn) : base(spawn) { }

        public override void DistributeSpawns(T map, List<E> spawns)
        {
            List<Loc> freeTiles = map.GetAllFreeTiles();

            for (int ii = 0; ii < spawns.Count && freeTiles.Count > 0; ii++)
            {
                E item = spawns[ii];

                int randIndex = map.Rand.Next(freeTiles.Count);
                map.PlaceItem(freeTiles[randIndex], item);
                freeTiles.RemoveAt(randIndex);
                GenContextDebug.DebugProgress("Placed Object");
            }
        }
    }
}
