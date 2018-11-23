using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RogueElements
{
    public static class Graph
    {
        public delegate List<int> GetAdjacents(int nodeIndex);
        public delegate void DistNodeAction(int nodeIndex, int distance);

        /// <summary>
        /// Traverses a list of nodes. Internally handles the state of traversed/untraversed nodes.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="start"></param>
        /// <param name="nodeAct"></param>
        /// <param name="getAdjacents"></param>
        public static void TraverseBreadthFirst(int count, int start, DistNodeAction nodeAct, GetAdjacents getAdjacents)
        {
            int[] found = new int[count];
            for (int ii = 0; ii < found.Length; ii++)
                found[ii] = -1;
            Queue<int> toExplore = new Queue<int>();
            toExplore.Enqueue(start);
            found[start] = 0;
            while (toExplore.Count > 0)
            {
                //take a node
                int node = toExplore.Dequeue();
                //act on node
                nodeAct(node, found[node]);
                //add adjacents to the END of queue
                List<int> adjacents = getAdjacents(node);
                for (int ii = 0; ii < adjacents.Count; ii++)
                {
                    int adjacent = adjacents[ii];
                    if (found[adjacent] == -1)
                    {
                        toExplore.Enqueue(adjacent);
                        found[adjacent] = found[node]+1;
                    }
                }
            }
        }
        
    }
}
