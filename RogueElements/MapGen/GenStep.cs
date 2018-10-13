using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public abstract class GenStep<T> : IGenStep
        where T : class, IGenContext
    {
        //change activemap into an interface that supports tile, mob, and item modification
        public abstract void Apply(T map);

        public bool CanApply(IGenContext context)
        {
            T map = context as T;
            if (map == null)
                return false;
            return true;
        }

        public void Apply(IGenContext context)
        {
            T map = context as T;
            if (map == null)
                return;

            Apply(map);
        }
    }

    public interface IGenStep
    {
        bool CanApply(IGenContext context);
        void Apply(IGenContext context);
    }
}
