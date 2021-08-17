using UnityEngine;

namespace GameU
{
    public class NavNode
    {
        public readonly Vector2Int coords;
        public float penaltyCost;
        public NavNode(Vector2Int coords, float penaltyCost = 0f)
        {
            this.coords = coords;
            this.penaltyCost = penaltyCost;
        }
    }
}