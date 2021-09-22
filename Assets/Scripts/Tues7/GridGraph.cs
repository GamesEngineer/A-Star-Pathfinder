using System;
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
            FindPath(start.position, goal.position);

            // Update the path renderer
            if (IsPathValid)
            {
                pathRenderer.positionCount = path.Count;
                for (int i = 0; i < path.Count; i++)
                {
                    Vector3 nodePosition = GridToWorld(path[i].coords);
                    pathRenderer.SetPosition(i, nodePosition);
                }
            }
            else
            {
                pathRenderer.positionCount = 0;
            }
        }

        public Vector3 GridToWorld(Vector2Int coords) => grid.CellToWorld((Vector3Int)coords);
        public Vector2Int WorldToGrid(Vector3 posWS) => (Vector2Int)grid.WorldToCell(posWS);

        public bool IsPathValid => path != null && path.Count >= 2;

        public bool FindPath(Vector3 startPosWS, Vector3 goalPosWS)
        {
            NavNode startNode = GetNode(startPosWS);
            NavNode goalNode = GetNode(goalPosWS);
            path = pathfinder.FindPath(startNode, goalNode, out pathCost);
            return IsPathValid;
        }

        public NavNode GetNode(Vector3 posWS) => GetNode(WorldToGrid(posWS));
        
        public NavNode GetNode(Vector2Int coords)
        {
            if (coords.x < 0 || coords.x >= gridSize.x ||
                coords.y < 0 || coords.y >= gridSize.y)
            {
                return null;
            }

            return nodes[coords.x, coords.y];
        }

        public int TotalNodeCount => nodes.Length;

        public void ForEachNeighbor(NavNode node, Action<NavNode, float /*edgeCost*/> vistorFunc)
        {
            for (int b = -1; b <= 1; b++)
            {
                for (int a = -1; a <= 1; a++)
                {
                    if (a == 0 && b == 0) continue; // not a neighbor
                    var offset = new Vector2Int(a, b);
                    NavNode neighbor = GetNode(node.coords + offset);
                    if (neighbor == null) continue; // invalid neighbor
                    float edgeCost = offset.magnitude;
                    vistorFunc(neighbor, edgeCost);
                }
            }
        }
    }
}
