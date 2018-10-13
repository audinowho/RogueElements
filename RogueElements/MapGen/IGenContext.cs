using System;
using System.Collections.Generic;

namespace RogueElements
{

    public interface IGenContext
    {
        IRandom Rand { get; }
        void InitSeed(ulong seed);
        void FinishGen();
    }
}
