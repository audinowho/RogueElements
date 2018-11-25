using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace RogueElements
{
    public static class GenContextDebug
    {
        public delegate void ProgressTrack(IGenContext map, IGenStep step);

        public static Action OnInit;
        public static ProgressTrack OnStep;


        public static void StressTest<T>(MapGen<T> layout, int amount) where T : class, IGenContext
        {
            ulong seed = 0;
            try
            {
                for (int ii = 0; ii < amount; ii++)
                {
                    seed = MathUtils.Rand.NextUInt64();
                    layout.GenMap(seed);
                }
            }
            catch (Exception ex)
            {
                Debug.Write("ERROR: " + seed);
                Debug.Write(ex.ToString());
                Debug.Write(ex.StackTrace);
            }
        }

    }
}
