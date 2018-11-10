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
#if DEBUG
            Debug.WriteLine("Seed: "+seed.ToString());
#endif
            //may not need floor ID
            T map = (T)Activator.CreateInstance(typeof(T));
            map.InitSeed(seed);

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
#if DEBUG
            //TODO: replace this hardcoding with something more modular...

            IRoomGridGenContext gridContext = map as IRoomGridGenContext;
            string gridDebugString = "";
            IFloorPlanGenContext listContext = map as IFloorPlanGenContext;
            string listDebugString = "";
            ITiledGenContext tiledContext = map as ITiledGenContext;
            string tileDebugString = "";
#endif

            while (queue.Count > 0)
            {
                IGenStep postProc = queue.Dequeue();
                postProc.Apply(map);

#if DEBUG
                Debug.WriteLine(postProc.ToString() + ":");

                if (gridContext != null)
                {
                    string newGridDebugString = gridContext.PrintGridRoomHalls();
                    if (gridDebugString != newGridDebugString)
                        Debug.Write(newGridDebugString);
                    gridDebugString = newGridDebugString;
                }

                if (listContext != null)
                {
                    string newListDebugString = listContext.PrintListRoomHalls();
                    if (listDebugString != newListDebugString)
                        Debug.Write(newListDebugString);
                    listDebugString = newListDebugString;
                }

                if (tiledContext != null)
                {
                    string newTileDebugString = tiledContext.PrintTiles();
                    if (tileDebugString != newTileDebugString)
                        Debug.Write(newTileDebugString);
                    tileDebugString = newTileDebugString;
                }
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
