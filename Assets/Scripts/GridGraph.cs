using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameU
{
    [RequireComponent(typeof(Grid))]
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

        public Vector2Int gridSize = Vector2Int.one * 100;
        public LayerMask obstacleLayers;
        private NavNode[,] nodes;

        private void Awake()
        {
            pathfinder = new AStarSearch(this);
            if (!grid) grid = GetComponent<Grid>();
            nodes = new NavNode[gridSize.x, gridSize.y];
            for (int y = 0; y < gridSize.y; y++)
            {
                for (int x = 0; x < gridSize.x; x++)
                {
                    Vector3Int coords = new Vector3Int(x, y, 0);
                    Vector3 p = grid.GetCellCenterWorld(coords);
                    if (Physics.CheckBox(p, grid.cellSize/2f, Quaternion.identity, obstacleLayers)) continue;
                    nodes[x, y] = new NavNode((Vector2Int)coords);
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (!grid) return;
            Vector3 worldSize = grid.CellToLocal((Vector3Int)gridSize);
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(transform.position + worldSize / 2f, worldSize);
        }

        private void OnDrawGizmosSelected()
        {
            if (!grid) return;
            Vector3 worldSize = grid.CellToLocal((Vector3Int)gridSize);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position + worldSize / 2f, worldSize);

            if (nodes == null) return;

            Gizmos.color = Color.red;
            for (int y = 0; y < gridSize.y; y++)
            {
                for (int x = 0; x < gridSize.x; x++)
                {
                    Vector3Int coords = new Vector3Int(x, y, 0);
                    Vector3 p = grid.GetCellCenterWorld(coords);
                    NavNode node = GetNode((Vector2Int)coords);
                    if (node != null) continue;
                    Gizmos.DrawWireCube(p, grid.cellSize * 0.9f);
                }
            }
        }

        private void Update()
        {
            pathRenderer.positionCount = 0;
            var path = FindPath(start.position, goal.position);
            if (path != null && path.Count > 0)
            {
                Vector3 s = GridToWorld(path[0].coords);
                if (pathRenderer)
                {
                    pathRenderer.positionCount = path.Count;
                    pathRenderer.SetPosition(0, s);
                }
                for (int n=1; n<path.Count; n++)
                {
                    Vector3 e = GridToWorld(path[n].coords);
                    if (pathRenderer)
                    {
                        pathRenderer.SetPosition(n, e);
                    }
                    Debug.DrawLine(s, e, Color.magenta);
                    s = e;
                }
            }
        }

        public Vector3 GridToWorld(Vector2Int coords) => grid.CellToWorld((Vector3Int)coords);

        public Vector2Int WorldToGrid(Vector3 p) => (Vector2Int)grid.WorldToCell(p);

        public NavNode GetNode(Vector2Int coords)
        {
            if (coords.x < 0 || coords.x >= gridSize.x ||
                coords.y < 0 || coords.y >= gridSize.y) return null;
            
            return nodes[coords.x, coords.y];
        }

        public NavNode GetNode(Vector3 posWS)
        {
            var coords = WorldToGrid(posWS);
            return GetNode(coords);
        }

        public void ForEachNeighbor(NavNode node, Action<NavNode, float> visitor)
        {
            for (int b = -1; b <= 1; b++)
            {
                for (int a = -1; a <= 1; a++)
                {
                    if (a == 0 && b == 0) continue;
                    var offset = new Vector2Int(a, b);
                    NavNode neighbor = GetNode(node.coords + offset);
                    if (neighbor == null) continue;
                    float edgeCost = offset.magnitude;
                    visitor(neighbor, edgeCost);
                }
            }
        }

        public List<NavNode> FindPath(Vector3 startPosWS, Vector3 endPosWS)
        {
            var startNode = GetNode(startPosWS);
            if (startNode == null) return null;

            var endNode = GetNode(endPosWS);
            if (endNode == null) return null;

            return pathfinder.FindPath(startNode, endNode); // TODO convert nodes to coordinates
        }

        private AStarSearch pathfinder;
    }
}
