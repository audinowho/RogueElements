using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace RogueElements
{
    [Serializable]
    public class MapGen<T>
        where T : class, IGenContext
    {
        public List<GenPriority<GenStep<T>>> GenSteps { get; set; }
        
        public MapGen()
        {
            GenSteps = new List<GenPriority<GenStep<T>>>();
        }
        

        //an initial create-map method
        public T GenMap(ulong seed)
        {
            //may not need floor ID
            T map = (T)Activator.CreateInstance(typeof(T));
            map.InitSeed(seed);

#if DEBUG
            if (GenContextDebug.OnInit != null)
                GenContextDebug.OnInit();
#endif

            //postprocessing steps:
            StablePriorityQueue<int, IGenStep> queue = new StablePriorityQueue<int, IGenStep>();
            foreach (GenPriority<GenStep<T>> postProc in GenSteps)
                queue.Enqueue(postProc.Priority, postProc.Item);

            ApplyGenSteps(map, queue);
            
            map.FinishGen();

            return map;
        }


        protected void ApplyGenSteps(T map, StablePriorityQueue<int, IGenStep> queue)
        {

            while (queue.Count > 0)
            {
                IGenStep postProc = queue.Dequeue();
                postProc.Apply(map);

#if DEBUG
                if (GenContextDebug.OnStep != null)
                    GenContextDebug.OnStep(map, postProc);
#endif
            }
        }

    }


    [Serializable]
    public class GenPriority<T> : IGenPriority where T : IGenStep
    {
        public int Priority { get; set; }
        public T Item;

        public GenPriority() { }
        public GenPriority(T effect)
        {
            Item = effect;
        }
        public GenPriority(int priority, T effect)
        {
            Priority = priority;
            Item = effect;
        }

        public IGenStep GetItem() { return Item; }
    }

    public interface IGenPriority
    {
        int Priority { get; set; }
        IGenStep GetItem();
    }
}
