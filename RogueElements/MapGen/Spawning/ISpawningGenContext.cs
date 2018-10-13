using System;
using System.Collections.Generic;

namespace RogueElements
{

    public interface ISpawningGenContext<T> : IGenContext
    {
        IRandPicker<T> Spawner { get; }
    }
}
