using Animations;
using UnityEngine;
using UnityEngine.UI;

namespace Dirty.VideoPlayer
{
    // Must be executed before VideoViewer's Awake().
    [DefaultExecutionOrder(-100)]
    [RequireComponent(typeof(Slider))]
    public class VolumeSlider : MonoBehaviour
    {
        public float Sensitivity = 1f;
        
        [SerializeField]
        private new CanvasGroupFadeAnimation animation = null;
        
        private Slider valueSlider;
        
        public Slider ValueSlider => valueSlider;

        private void Awake()
        {
            valueSlider = GetComponent<Slider>();
        }

        private void Start()
        {
            animation.CanvasGroup.alpha = 0;
        }

        public void AddValue(float deltaValue)
        {
            animation.CanvasGroup.alpha = 1;
            
            ValueSlider.value += deltaValue * Sensitivity;
        }

        public void Hide()
        {
            animation.PlayOutTween();
        }
    }
}