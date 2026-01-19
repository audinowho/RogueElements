// <copyright file="Graph.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RogueElements
{
    /// <summary>
    /// Provides utility methods for graph traversal algorithms.
    /// </summary>
    public static class Graph
    {
        /// <summary>
        /// Delegate for retrieving adjacent nodes from a given node.
        /// </summary>
        /// <typeparam name="T">The type of node identifier.</typeparam>
        /// <param name="nodeIndex">The node to get adjacents for.</param>
        /// <returns>List of adjacent node identifiers.</returns>
        public delegate List<T> GetAdjacents<T>(T nodeIndex);

        /// <summary>
        /// Delegate for performing an action on a node with its distance from the start.
        /// </summary>
        /// <typeparam name="T">The type of node identifier.</typeparam>
        /// <param name="nodeIndex">The node identifier.</param>
        /// <param name="distance">The distance from the starting node.</param>
        public delegate void DistNodeAction<T>(T nodeIndex, int distance);

        /// <summary>
        /// Traverses a list of nodes. Internally handles the state of traversed/untraversed nodes.
        /// </summary>
        /// <typeparam name="TID"></typeparam>
        /// <param name="start"></param>
        /// <param name="nodeAct"></param>
        /// <param name="getAdjacents"></param>
        public static void TraverseBreadthFirst<TID>(TID start, DistNodeAction<TID> nodeAct, GetAdjacents<TID> getAdjacents)
        {
            if (nodeAct == null)
                throw new ArgumentNullException(nameof(nodeAct));
            if (getAdjacents == null)
                throw new ArgumentNullException(nameof(getAdjacents));

            Dictionary<TID, int> found = new Dictionary<TID, int>();
            Queue<TID> toExplore = new Queue<TID>();
            toExplore.Enqueue(start);
            found[start] = 0;
            while (toExplore.Count > 0)
            {
                // take a node
                TID node = toExplore.Dequeue();

                // act on node
                nodeAct(node, found[node]);

                // add adjacents to the END of queue
                List<TID> adjacents = getAdjacents(node);
                for (int ii = 0; ii < adjacents.Count; ii++)
                {
                    TID adjacent = adjacents[ii];
                    if (!found.ContainsKey(adjacent))
                    {
                        toExplore.Enqueue(adjacent);
                        found[adjacent] = found[node] + 1;
                    }
                }
            }
        }
    }
}
