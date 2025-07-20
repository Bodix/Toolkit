using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using NaughtyAttributes;
using UnityEngine;

namespace Common.Modules.Tweens
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundEffect : MonoBehaviour
    {
        public float FadeInDuration = 0.3f;
        public float FadeOutDuration = 0.3f;
        public bool OverrideClipDuration = false;

        [SerializeField, ShowIf(nameof(OverrideClipDuration))]
        private float _duration = 0;
        [SerializeField, HideInInspector]
        private AudioSource _audioSource;

        public AudioSource AudioSource => _audioSource;
        public float Duration => OverrideClipDuration ? _duration : _audioSource.clip.length;
        private readonly TweenCallback<float> EmptyVirtualUpdate = _ => { };

        private void Awake()
        {
            if (!_audioSource)
                _audioSource = GetComponent<AudioSource>();
        }

        private void OnValidate()
        {
            if (!_audioSource)
                _audioSource = GetComponent<AudioSource>();

            if (!OverrideClipDuration && _audioSource.clip)
                _duration = _audioSource.clip.length;
        }

        public Tweener Play()
        {
            return Play(Duration);
        }

        public Tweener Play(float duration)
        {
            if (duration <= 0)
                Debug.LogError("Cannot play audio with zero or negative duration");

            return DOVirtual.Float(0, duration, duration, EmptyVirtualUpdate)
                .OnStart(() => _audioSource.Play())
                .OnComplete(() => _audioSource.Stop());
        }

        public TweenerCore<float, float, FloatOptions> PlayFadeIn()
        {
            return PlayFadeIn(FadeInDuration);
        }

        public TweenerCore<float, float, FloatOptions> PlayFadeIn(float fadeDuration)
        {
            float targetVolume = _audioSource.volume;

            _audioSource.volume = 0;
            _audioSource.Play();
            return _audioSource.DOFade(targetVolume, fadeDuration);
        }

        public Sequence PlayAndFadeOut(bool restoreVolumeOnComplete = true)
        {
            return PlayAndFadeOut(Duration, FadeOutDuration, restoreVolumeOnComplete);
        }

        public Sequence PlayAndFadeOut(float duration, float fadeDuration, bool restoreVolumeOnComplete = true)
        {
            float cachedVolume = _audioSource.volume;

            Sequence sequence = DOTween.Sequence();
            sequence.Append(_audioSource.DOFade(0, fadeDuration).SetDelay(duration - fadeDuration));
            sequence.Insert(0, Play(duration));
            sequence.AppendCallback(() =>
            {
                if (restoreVolumeOnComplete)
                    _audioSource.volume = cachedVolume;
            });

            return sequence;
        }

        public TweenerCore<float, float, FloatOptions> FadeOutAndPause(bool restoreVolumeOnComplete = true)
        {
            return FadeOutAndPause(FadeOutDuration, restoreVolumeOnComplete);
        }

        public TweenerCore<float, float, FloatOptions> FadeOutAndPause(
            float fadeDuration, bool restoreVolumeOnComplete = true)
        {
            float cachedVolume = _audioSource.volume;

            return _audioSource.DOFade(0, fadeDuration)
                .OnComplete(() =>
                {
                    _audioSource.Pause();

                    if (restoreVolumeOnComplete)
                        _audioSource.volume = cachedVolume;
                });
        }
    }
}