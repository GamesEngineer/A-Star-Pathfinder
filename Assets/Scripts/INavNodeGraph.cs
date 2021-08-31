using System;

namespace GameU
{
    public interface INavNodeGraph
    {
        void ForEachNeighbor(NavNode node, Action<NavNode, float> visitorFunc);
        int TotalNodeCount { get; }
    }
}
