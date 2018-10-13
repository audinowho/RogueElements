using System;
using System.Collections.Generic;

namespace RogueElements
{
    public interface IStepSpawner<T, E> where T : IGenContext
    {
        List<E> GetSpawns(T map);

    }
}
