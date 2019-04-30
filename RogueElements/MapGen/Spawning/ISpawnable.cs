using System;

namespace RogueElements
{
    public interface ISpawnable
    {
        /// <summary>
        /// Creates a copy of the object, to be placed in the generated layout.
        /// </summary>
        /// <returns></returns>
        ISpawnable Copy();
    }
}
