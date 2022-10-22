using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Dirty.VideoPlayer
{
    // Must be executed before VideoViewer's Awake().
    [DefaultExecutionOrder(-100)]
    [RequireComponent(typeof(Slider))]
    public class Timeline : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private Slider slider;

        public event Action PointerDown;
        public event Action PointerUp;

        public Slider Slider => slider;
        public bool IsUsing { get; private set; }

        private void Awake()
        {
            slider = GetComponent<Slider>();
        }

        public void Reset()
        {
            slider.value = 0;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            IsUsing = true;

            PointerDown?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            IsUsing = false;

            PointerUp?.Invoke();
        }
    }
}