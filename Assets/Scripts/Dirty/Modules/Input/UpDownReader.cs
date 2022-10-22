using UnityEngine;
using UnityEngine.EventSystems;

namespace Dirty.Input
{
    public delegate void PositionHandler(Vector2 position);

    public class UpDownReader : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public event PositionHandler Down;
        public event PositionHandler Up;

        public void OnPointerDown(PointerEventData eventData)
        {
            Down?.Invoke(eventData.position);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Up?.Invoke(eventData.position);
        }
    }
}