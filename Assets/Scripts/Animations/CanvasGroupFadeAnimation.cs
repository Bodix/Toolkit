using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Evolutex.Evolunity.Components.Animations;
using UnityEngine;
using UnityEngine.Events;

namespace Animations
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CanvasGroupFadeAnimation : MonoBehaviour, IInOutPlayable
    {
        public float Duration = 0.5f;
        public Ease Ease = Ease.Linear;
        public UnityEvent OnComplete;
        
        public CanvasGroup CanvasGroup { get; private set; }

        private void Awake()
        {
            CanvasGroup = GetComponent<CanvasGroup>();
        }

        public void PlayIn()
        {
            PlayInTween();
        }

        public void PlayOut()
        {
            PlayOutTween();
        }

        public TweenerCore<float, float, FloatOptions> PlayInTween()
        {
            return CanvasGroup.DOFade(1, Duration)
                .SetEase(Ease)
                .OnComplete(() => OnComplete.Invoke());
        }
        
        public Tween PlayInTweenFull()
        {
            return PlayInTween().From(0);
        }

        public TweenerCore<float, float, FloatOptions> PlayOutTween()
        {
            return CanvasGroup.DOFade(0, Duration)
                .SetEase(Ease)
                .OnComplete(() => OnComplete.Invoke());
        }
        
        public Tween PlayOutTweenFull()
        {
            return PlayOutTween().From(1);
        }
    }
}
