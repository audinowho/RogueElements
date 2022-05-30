// <copyright file="MapGen.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace RogueElements
{
    [Serializable]
    public class MapGen<T>
        where T : class, IGenContext
    {
        public MapGen()
        {
            this.GenSteps = new PriorityList<GenStep<T>>();
        }

        public PriorityList<GenStep<T>> GenSteps { get; }

        // an initial create-map method
        public T GenMap(ulong seed)
        {
            // may not need floor ID
            T map = (T)Activator.CreateInstance(typeof(T));
            map.InitSeed(seed);

            GenContextDebug.DebugInit(map);

            // postprocessing steps:
            StablePriorityQueue<Priority, IGenStep> queue = new StablePriorityQueue<Priority, IGenStep>();
            foreach (Priority priority in this.GenSteps.GetPriorities())
            {
                foreach (IGenStep genStep in this.GenSteps.GetItems(priority))
                    queue.Enqueue(priority, genStep);
            }

            ApplyGenSteps(map, queue);

            map.FinishGen();

            return map;
        }

        protected static void ApplyGenSteps(T map, StablePriorityQueue<Priority, IGenStep> queue)
        {
            while (queue.Count > 0)
            {
                IGenStep postProc = queue.Dequeue();
                GenContextDebug.StepIn(postProc.ToString());

                try
                {
                    postProc.Apply(map);
                }
                catch (Exception ex)
                {
                    GenContextDebug.DebugError(ex);
                }

                GenContextDebug.StepOut();
            }
        }
    }
}
