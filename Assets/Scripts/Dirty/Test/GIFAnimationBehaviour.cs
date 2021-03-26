using UnityEngine;
using UnityEngine.UI;

namespace Dirty.Test
{
    public class GIFAnimationBehaviour : MonoBehaviour
    {
        [SerializeField] private Sprite[] _gifSprites;
        [SerializeField] private Image animationImage;

        private const int TIME_OFFEST = 5;

        void Update()
        {
            int index = (int)(Time.time * TIME_OFFEST) % _gifSprites.Length;
            animationImage.sprite = _gifSprites[index];
        }
    }
}
