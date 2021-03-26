using Animations;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Dirty.VideoPlayer
{
    // TODO:
    // 1. Remove dependency from App and injects.
    // 2. Move to Core?

    [RequireComponent(typeof(UnityEngine.Video.VideoPlayer))]
    public class VideoViewer : MonoBehaviour
    {
        [SerializeField]
        private Timeline timeline = null;
        [SerializeField]
        private VolumeSlider volumeSlider = null;
        [SerializeField]
        private Toggle playPauseToggle = null;
        [SerializeField]
        private new CanvasGroupFadeAnimation animation = null;
        [SerializeField]
        private Color backgroundColor = Color.black;

        private UnityEngine.Video.VideoPlayer videoPlayer;
        private string lastVideoPath;

        private float Duration => videoPlayer.frameCount / videoPlayer.frameRate;
        private double NormalizedTime => videoPlayer.time / Duration;

        private void Awake()
        {
            videoPlayer = GetComponent<UnityEngine.Video.VideoPlayer>();
            
            // TODO:
            // Move to extension method.
            videoPlayer.targetTexture.Release();
            videoPlayer.targetTexture.width = Screen.width;
            videoPlayer.targetTexture.height = Screen.height;

            timeline.PointerDown += () => videoPlayer.Pause();
            timeline.PointerUp += () => videoPlayer.Play();
            timeline.Slider.onValueChanged
                .AddListener(value => videoPlayer.time = value * Duration);

            volumeSlider.ValueSlider.onValueChanged
                .AddListener(value => videoPlayer.SetDirectAudioVolume(0, value));

            playPauseToggle.onValueChanged.AddListener(isOn =>
            {
                if (isOn)
                    videoPlayer.Play();
                else
                    videoPlayer.Pause();
            });
        }

        private void Update()
        {
            if (!videoPlayer.isPrepared || timeline.IsUsing)
                return;

            // timeline.Slider.SetValueWithoutNotify((float) NormalizedTime);
        }

        public void OpenVideo(string videoPath)
        {
            gameObject.SetActive(true);

            // Fix transparent image on opening.
            RenderTexture.active = videoPlayer.targetTexture;
            GL.Clear(true, true, backgroundColor);
            RenderTexture.active = null;
            
            videoPlayer.url = videoPath;
            lastVideoPath = videoPath;

            animation.PlayInTweenFull()
                .OnComplete(() => { videoPlayer.Play(); });
        }

        public void Close()
        {
            videoPlayer.Stop();

            animation.PlayOutTween()
                .OnComplete(() =>
                {
                    timeline.Reset();

                    gameObject.SetActive(false);
                });
        }
    }
}