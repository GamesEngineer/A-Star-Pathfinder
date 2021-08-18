using UnityEngine;
using UnityEngine.EventSystems;

namespace GameU
{
    public class DragWithMouse : MonoBehaviour, IDragHandler
    {
        public void OnDrag(PointerEventData eventData)
        {
            transform.position = eventData.pointerCurrentRaycast.worldPosition;
        }
    }
}
