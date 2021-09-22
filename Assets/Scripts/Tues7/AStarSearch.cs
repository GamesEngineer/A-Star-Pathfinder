using System.Collections.Generic;
using UnityEngine;

namespace Tues7
{
    public class AStarSearch
    {
        public readonly GridGraph graph;

        public AStarSearch(GridGraph graph)
        {
            this.graph = graph;
        }

        /// <summary>
        /// A reference to a NavNode with additional "cost" information that is used by the search algorithm.
        /// </summary>
        private class OpenedNode // TODO - implement interfaces necessary for prioritization
        {
            public OpenedNode(NavNode node, NavNode goalNode, OpenedNode upstreamNode = null, float edgeCost = 0f)
            {
                this.node = node;
                Update(upstreamNode, edgeCost, goalNode);
            }

            public readonly NavNode node;
            public OpenedNode UpstreamNode { get; private set; }
            public float PartialPathCost { get; private set; } // Actual cost so far
            public float RemainingPathCost { get; private set; } // Estimated cost to go
            public float TotalCost => PartialPathCost + RemainingPathCost;
            public void Update(OpenedNode upstreamNode, float edgeCost, NavNode goalNode)
            {
                UpstreamNode = upstreamNode;
                PartialPathCost = node.penaltyCost;
                if (upstreamNode != null)
                {
                    PartialPathCost += edgeCost + upstreamNode.PartialPathCost;
                }
                RemainingPathCost = Vector2.Distance(node.coords, goalNode.coords); // This must NOT be a pesimistic estimate
            }
        }

        private readonly Queue<OpenedNode> openNodesQueue = new Queue<OpenedNode>(); // TODO - replace with a PriorityQueue or MinHeap
        private readonly Dictionary<NavNode, OpenedNode> openNodesLookup = new Dictionary<NavNode, OpenedNode>();
        private readonly HashSet<NavNode> closedNodes = new HashSet<NavNode>();

        public List<NavNode> FindPath(NavNode startNode, NavNode goalNode, out float totalCost)
        {
            totalCost = 0f;
            if (startNode == null || goalNode == null) return null;

            closedNodes.Clear();
            openNodesLookup.Clear();
            openNodesQueue.Clear();

            OpenedNode start = new OpenedNode(startNode, goalNode); // DISCUSS: this creates garbage for the Garbage Collector
            openNodesQueue.Enqueue(start);

            int failsafe = graph.TotalNodeCount;
            while (openNodesQueue.Count > 0)
            {
                if (--failsafe < 0) break;

                // From the OPEN queue, take the next node, and close it
                OpenedNode current = openNodesQueue.Dequeue();
                closedNodes.Add(current.node);
                openNodesLookup.Remove(current.node);

                if (current.node == goalNode)
                {
                    // SUCCESS!
                    totalCost = current.TotalCost;
                    return ConstructPath(current);
                }

                // TODO - open/update neighbors of the current node, unless they are already closed
                graph.ForEachNeighbor(current.node,
                    (neighbor, edgeCost) =>
                    {
                        if (closedNodes.Contains(neighbor)) return;
                        bool isNeighborOpen = openNodesLookup.TryGetValue(neighbor, out OpenedNode openNeighbor);
                        if (isNeighborOpen)
                        {
                            float costViaCurrentNode = current.PartialPathCost + edgeCost + neighbor.penaltyCost;
                            if (costViaCurrentNode < openNeighbor.PartialPathCost)
                            {
                                openNeighbor.Update(current, edgeCost, goalNode);
                                // TODO - reprioritize the queue
                            }
                        }
                        else
                        {
                            openNeighbor = new OpenedNode(neighbor, goalNode, current, edgeCost);
                            openNodesQueue.Enqueue(openNeighbor);
                            openNodesLookup.Add(neighbor, openNeighbor);
                        }
                    }
                );
            }

            // no path found
            return null;
        }

        private static List<NavNode> ConstructPath(OpenedNode pathNode)
        {
            var path = new List<NavNode>();
            while (pathNode != null)
            {
                path.Add(pathNode.node);
                pathNode = pathNode.UpstreamNode;
            }
            path.Reverse();
            return path;
        }
    }
}