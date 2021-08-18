using System.Collections;
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

        private void OnDrawGizmos()
        {
            if (grid == null) return;

            Vector3 worldSize = grid.CellToLocal( (Vector3Int) gridSize);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position + worldSize / 2f, worldSize);
        }
    }
}
