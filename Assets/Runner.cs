using UnityEngine;

namespace GameU
{
    public class Runner : MonoBehaviour
    {
        public GridGraph graph;
        public float maxSpeed = 2f;
        private Vector3 velocity;

        private void Update()
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            Vector3 pos = transform.position;

            var node = graph.GetNode(pos);
            if (node != null)
            {
                velocity = new Vector3(h, v, 0) * maxSpeed / node.penaltyCost;
            }
            else
            {
                return;
            }
            
            pos += velocity * Time.deltaTime;
            node = graph.GetNode(pos);
            if (node == null) return;

            transform.position = pos;
        }
    }
}
