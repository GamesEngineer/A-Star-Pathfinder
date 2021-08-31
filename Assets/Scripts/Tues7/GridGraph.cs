using System.Collections.Generic;
using UnityEngine;

namespace Tues7
{
    public class GridGraph : MonoBehaviour
    {
        [SerializeField]
        protected Grid grid;

        [SerializeField]
        protected Transform start;

        [SerializeField]
        protected Transform goal;

        [SerializeField]
        protected LineRenderer pathRenderer;

        [SerializeField]
        protected Vector2Int gridSize = Vector2Int.one * 100;

        [SerializeField]
        protected LayerMask obstacleLayers;

        private void OnDrawGizmos()
        {
            if (grid == null) return;

            Vector3 worldSize = grid.CellToLocal( (Vector3Int) gridSize);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position + worldSize / 2f, worldSize);
        }

        private NavNode[,] nodes;
        private List<NavNode> path; // TODO (optional) - move path and its cost into its own class (ala NavMeshPath)
        private float pathCost;
        private AStarSearch pathfinder;

        private void Awake()
        {
            pathfinder = new AStarSearch(this);
            nodes = new NavNode[gridSize.x, gridSize.y];
            for (int y = 0; y < gridSize.y; y++)
            {
                for (int x = 0; x < gridSize.x; x++)
                {
                    Vector3Int coords = new Vector3Int(x, y, 0);
                    Vector3 posWS = grid.GetCellCenterWorld(coords);
                    bool isObstacle = Physics.CheckBox(posWS, grid.cellSize / 2f, Quaternion.identity, obstacleLayers);
                    if (isObstacle) continue;
                    nodes[x, y] = new NavNode((Vector2Int)coords);
                }
            }
        }

        private void Update()
        {
            // TODO - Tuesday 7 PM, Aug 31
        }
    }
}
