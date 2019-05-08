using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class ContextSpawner<T, E> : IStepSpawner<T, E> 
        where T : ISpawningGenContext<E>
        where E : ISpawnable
    {
        public RandRange Amount;

        public ContextSpawner() { }

        public ContextSpawner(RandRange amount)
        {
            Amount = amount;
        }

        public List<E> GetSpawns(T map)
        {
            int chosenAmount = Amount.Pick(map.Rand);
            List<E> results = new List<E>();
            for (int ii = 0; ii < chosenAmount; ii++)
            {
                if (!map.Spawner.CanPick)
                    break;
                results.Add(map.Spawner.Pick(map.Rand));
            }
            return results;
        }
    }
}
