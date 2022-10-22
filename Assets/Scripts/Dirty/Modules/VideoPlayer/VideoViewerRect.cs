using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Dirty.VideoPlayer
{
    [RequireComponent(typeof(Graphic))]
    public class VideoViewerRect : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        private VolumeSlider volumeSlider = null;
        [SerializeField]
        private Toggle playPauseToggle = null;

        private bool isDragging;

        public void OnDrag(PointerEventData eventData)
        {
            isDragging = true;

            volumeSlider.AddValue(eventData.delta.y / Screen.height);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!isDragging)
                playPauseToggle.isOn = !playPauseToggle.isOn;

            isDragging = false;

            volumeSlider.Hide();
        }

        // Need for correct working of OnPointerUp method.
        public void OnPointerDown(PointerEventData eventData)
        {
        }
    }
}