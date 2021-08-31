//#define USE_OCTILE_DIAGONAL_DISTANCE
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameU
{
    public class AStarSearch
    {
        public readonly INavNodeGraph graph;

        public AStarSearch(INavNodeGraph graph)
        {
            this.graph = graph;
        }

        public List<NavNode> FindPath(NavNode startNode, NavNode goalNode, out float totalCost)
        {
            totalCost = 0f;
            if (startNode == null || goalNode == null) return null;

            closedNodes.Clear();
            openNodesQueue.Clear();
            openNodesLookup.Clear();

            OpenedNode start = new OpenedNode(startNode, goalNode);
            openNodesQueue.Enqueue(start);

            int failsafe = graph.TotalNodeCount;
            while (openNodesQueue.Count > 0)
            {
                if (failsafe-- < 0)
                    break;

                // From the OPEN set, take the node with lowest total cost
                OpenedNode current = openNodesQueue.Dequeue();
                closedNodes.Add(current.node);
                openNodesLookup.Remove(current.node);

                if (current.node == goalNode)
                {
                    // SUCCESS!
                    totalCost = current.TotalCost;
                    return ConstructPath(current);
                }

                // LESSON NOTE: notice how the following lambda function captures more than just neighbor and edgeCost

                graph.ForEachNeighbor(current.node, (neighbor, edgeCost) =>
                {
                    if (closedNodes.Contains(neighbor)) return;
                    bool isNeighborOpen = openNodesLookup.TryGetValue(neighbor, out OpenedNode openNeighbor);
                    if (isNeighborOpen)
                    {
                        float costViaCurrentNode = current.PartialPathCost + edgeCost + neighbor.penaltyCost;
                        if (costViaCurrentNode < openNeighbor.PartialPathCost)
                        {
                            openNeighbor.Update(current, edgeCost, goalNode);
                            openNodesQueue.Reprioritize(openNeighbor);
                        }
                    }
                    else
                    {
                        openNeighbor = new OpenedNode(neighbor, goalNode, current, edgeCost);
                        openNodesQueue.Enqueue(openNeighbor);
                        openNodesLookup.Add(neighbor, openNeighbor);
                    }
                });
            }

            // No path found
            return null;
        }

        private List<NavNode> ConstructPath(OpenedNode pathNode)
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

        // TODO - replace MinHeap with PriorityQueue once .NET 6 is available for use
        private readonly MinHeap<OpenedNode> openNodesQueue = new MinHeap<OpenedNode>();
        private readonly Dictionary<NavNode, OpenedNode> openNodesLookup = new Dictionary<NavNode, OpenedNode>();
        private readonly HashSet<NavNode> closedNodes = new HashSet<NavNode>();

        /// <summary>
        /// Information about a node in the open set.
        /// </summary>
        private class OpenedNode : IEquatable<OpenedNode>, IComparable<OpenedNode>, IHeapItem
        {
            public OpenedNode(NavNode node, NavNode goalNode, OpenedNode upstreamNode = null, float edgeCost = 0f)
            {
                this.node = node;
                Update(upstreamNode, edgeCost, goalNode);
            }

            public readonly NavNode node;
            public OpenedNode UpstreamNode { get; private set; }
            public float PartialPathCost { get; private set; }
            public float RemainingPathCost { get; private set; }
            public float TotalCost => PartialPathCost + RemainingPathCost;
            public int HeapPosition { get; set; } = -1;

            public int CompareTo(OpenedNode other)
            {
                float d = this.TotalCost - other.TotalCost;
#if false
                // ERROR! Unity's Sign function never returns zero
                int cmp = (int)Mathf.Sign(d);
#else
                int cmp = d < 0f ? -1 : (d > 0f ? 1 : 0);
#endif
                return cmp;
            }

            public void Update(OpenedNode upstreamNode, float edgeCost, NavNode goalNode)
            {
                this.UpstreamNode = upstreamNode;
                this.PartialPathCost = node.penaltyCost;
                if (upstreamNode != null)
                {
                    this.PartialPathCost += edgeCost + upstreamNode.PartialPathCost;
                }
                RemainingPathCost = EstimatePathCost(node.coords, goalNode.coords);
            }

            public static float EstimatePathCost(Vector2Int from, Vector2Int to)
            {
#if USE_OCTILE_DIAGONAL_DISTANCE
                Vector2Int delta = to - from;
                delta.x = delta.x < 0 ? -delta.x : delta.x;
                delta.y = delta.y < 0 ? -delta.y : delta.y;
                const float SQRT2 = 1.41421356f;
                if (delta.x < delta.y)
                {
                    return delta.x * SQRT2 + (delta.y - delta.x);
                }
                else
                {
                    return delta.y * SQRT2 + (delta.x - delta.y);
                }
#else
                return Vector2.Distance(from, to);
#endif
            }

            public bool Equals(OpenedNode other)
            {
                return node == other.node &&
                    UpstreamNode == other.UpstreamNode &&
                    PartialPathCost == other.PartialPathCost &&
                    RemainingPathCost == other.RemainingPathCost &&
                    HeapPosition == other.HeapPosition;
            }
        }
    }
}
