using System;
using System.Collections.Generic;

namespace RogueElements
{

    public interface ISpawningGenContext<T> : IGenContext
        where T : ISpawnable
    {
        IRandPicker<T> Spawner { get; }
    }
}
