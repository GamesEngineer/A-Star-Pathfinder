using UnityEngine;

namespace GameU
{
    public class Navigator : MonoBehaviour
    {
        public float speed = 10f;
        public Transform goal;
        public GridGraph graph;
        public Grid grid;

        private Vector3 velocity;

        void Update()
        {
            if (Vector3.Distance(transform.position, goal.position) < grid.cellSize.magnitude * 0.5) return;

            bool foundPath = graph.FindPath(transform.position, goal.position);
            if (foundPath)
            {
                var here = graph.CurrentPath[0];
                var next = graph.CurrentPath[1];
                var destination = graph.GridToWorld(next.coords);
                Vector3 delta = destination - transform.position;
                velocity = delta.normalized * (speed / here.penaltyCost);
            }
            else
            {
                velocity *= 1f - Time.deltaTime * 0.1f;
            }

            transform.Translate(velocity * Time.deltaTime);
        }
    }
}
