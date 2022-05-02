using System;
using NaughtyAttributes;
using UnityEngine;

namespace _WIP.UI
{
    public abstract class UiElement : MonoBehaviour
    {
        [Foldout("Animations")]
        public AnimationBehaviour ShowAnimation;
        [Foldout("Animations")]
        public AnimationBehaviour HideAnimation;
        // ClickAnimation

        public event Action Showing;
        public event Action Hiding;
        public event Action Shown;
        public event Action Hidden;

        public virtual bool IsShown => gameObject.activeSelf;

        [ContextMenu(nameof(Show))]
        public void Show()
        {
            Show(false);
        }

        [ContextMenu(nameof(Hide))]
        public void Hide()
        {
            Hide(false);
        }
        
        [ContextMenu(nameof(ShowInstantly))]
        public void ShowInstantly()
        {
            Show(true);
        }

        [ContextMenu(nameof(HideInstantly))]
        public void HideInstantly()
        {
            Hide(true);
        }

        protected virtual void Show(bool instantly)
        {
            if (IsShown)
                return;
            
            if (!ShowAnimation || instantly)
            {
                OnShowAnimationStart();
                OnShowAnimationComplete();
            }
            else
            {
                ShowAnimation.Play(
                    onStart: OnShowAnimationStart,
                    onComplete: OnShowAnimationComplete);
            }
        }

        protected virtual void Hide(bool instantly)
        {
            if (!IsShown)
                return;
            
            if (!HideAnimation || instantly)
            {
                OnHideAnimationStart();
                OnHideAnimationComplete();
            }
            else
            {
                HideAnimation.Play(
                    onStart: OnHideAnimationStart,
                    onComplete: OnHideAnimationComplete);
            }
        }

        protected virtual void OnShowAnimationStart()
        {
            gameObject.SetActive(true);

            Showing?.Invoke();
        }

        protected virtual void OnShowAnimationComplete()
        {
            Shown?.Invoke();
        }

        protected virtual void OnHideAnimationStart()
        {
            Hiding?.Invoke();
        }

        protected virtual void OnHideAnimationComplete()
        {
            gameObject.SetActive(false);

            Hidden?.Invoke();
        }
    }
}