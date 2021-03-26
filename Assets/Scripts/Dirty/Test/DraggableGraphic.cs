using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Dirty.Test
{
    [RequireComponent(typeof(Graphic))]
    public class DraggableGraphic : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        public float DecelerationRate = 0.135f;
        public float MaxInertiaSpeed = 50f;

        private RectTransform rectTransform;
        private new Rigidbody2D rigidbody2D;
        private Vector2 prevDelta;
        private bool isInertia;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            rigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            if (isInertia)
            {
                prevDelta *= 1 - DecelerationRate;

                rigidbody2D.velocity = Vector2.zero;
                rigidbody2D.MovePosition(rigidbody2D.position + prevDelta);
                // rigidbody2D.Add(rigidbody2D.position + prevDelta);
            }

            if (prevDelta.magnitude <= 0.01)
                isInertia = false;
        }

        public void OnDrag(PointerEventData eventData)
        {
            // rigidbody2D.
            rigidbody2D.velocity = Vector2.zero;
            rigidbody2D.MovePosition(rigidbody2D.position + eventData.delta);
            // rigidbody2D.AddForce(rigidbody2D.position + eventData.delta);

            prevDelta = eventData.delta;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isInertia = false;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isInertia = true;

            prevDelta = Vector2.ClampMagnitude(prevDelta, MaxInertiaSpeed);
        }
    }
}