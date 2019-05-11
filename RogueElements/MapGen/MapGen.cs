﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace RogueElements
{
    [Serializable]
    public class MapGen<T>
        where T : class, IGenContext
    {
        public PriorityList<IGenStep<T>> GenSteps { get; set; }
        
        public MapGen()
        {
            GenSteps = new PriorityList<IGenStep<T>>();
        }
        

        //an initial create-map method
        public T GenMap(ulong seed)
        {
            //may not need floor ID
            T map = (T)Activator.CreateInstance(typeof(T));
            map.InitSeed(seed);

            GenContextDebug.DebugInit(map);

            //postprocessing steps:
            StablePriorityQueue<int, IGenStep> queue = new StablePriorityQueue<int, IGenStep>();
            foreach (int priority in GenSteps.GetPriorities())
            {
                foreach (IGenStep genStep in GenSteps.GetItems(priority))
                    queue.Enqueue(priority, genStep);
            }

            ApplyGenSteps(map, queue);
            
            map.FinishGen();

            return map;
        }


        protected void ApplyGenSteps(T map, StablePriorityQueue<int, IGenStep> queue)
        {

            while (queue.Count > 0)
            {
                IGenStep postProc = queue.Dequeue();
                GenContextDebug.StepIn(postProc.ToString());
                postProc.Apply(map);
                GenContextDebug.StepOut();
            }
        }

    }
}
