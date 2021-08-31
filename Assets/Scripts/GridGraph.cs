using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GameU
{
    public class GridGraph : MonoBehaviour, INavNodeGraph
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

        [SerializeField]
        protected TextMeshProUGUI pathCostText;

        private NavNode[,] nodes;
        private List<NavNode> path;
        private float pathCost;
        private AStarSearch pathfinder;

        public bool IsPathValid => path != null && path.Count >= 2;
        public IReadOnlyList<NavNode> CurrentPath => path;
        public int TotalNodeCount => nodes.Length;

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
            FindPath(start.position, goal.position);

            // Update the path renderer and cost label
            if (IsPathValid)
            {
                pathRenderer.positionCount = path.Count;
                for (int i = 0; i < path.Count; i++)
                {
                    Vector3 nodePosition = GridToWorld(path[i].coords);
                    pathRenderer.SetPosition(i, nodePosition);
                }
                pathCostText.text = $"Cost: {pathCost}";
            }
            else
            {
                pathRenderer.positionCount = 0;
                pathCostText.text = $"INVALID PATH";
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

        public NavNode GetNode(Vector3 posWS) => GetNode( WorldToGrid(posWS) );

        public bool FindPath(Vector3 startPosWS, Vector3 endPosWS)
        {
            var startNode = GetNode(startPosWS);
            var goalNode = GetNode(endPosWS);
            #region OPTIONAL CODING CHALLENGE
            // Optimization: avoid recomputing the path when it is still valid
            if (IsPathValid && path[0] == startNode && path[path.Count - 1] == goalNode)
            {
                return true;
            }
            #endregion
            path = pathfinder.FindPath(startNode, goalNode, out pathCost);
            return IsPathValid;
        }
        
        public void ForEachNeighbor(NavNode node, Action<NavNode, float> visitorFunc)
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
                    visitorFunc(neighbor, edgeCost);
                }
            }
        }
    
    }
}
