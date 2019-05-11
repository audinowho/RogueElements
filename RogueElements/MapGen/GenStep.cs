using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public abstract class GenStep<T> : IGenStep<T>
        where T : IGenContext
    {
        //change activemap into an interface that supports tile, mob, and item modification
        public abstract void Apply(T map);

        public bool CanApply(IGenContext context)
        {
            if (context is T)
                return true;
            else
                return false;
        }

        public void Apply(IGenContext context)
        {
            if (context is T)
                Apply((T)context);
        }
    }

    public interface IGenStep<in T> : IGenStep
        where T : IGenContext
    {
    }

    public interface IGenStep
    {
        bool CanApply(IGenContext context);
        void Apply(IGenContext context);
    }
}
