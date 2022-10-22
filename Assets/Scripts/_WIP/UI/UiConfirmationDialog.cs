using System;
using UnityEngine;
using UnityEngine.UI;

namespace _WIP.UI
{
    public class UiConfirmationDialog : UiElement
    {
        [SerializeField]
        protected Button acceptButton;
        [SerializeField]
        protected Button declineButton;

        protected Action<Result> _resultCallback;

        public event Action Accepted;
        public event Action Declined;

        protected virtual void Awake()
        {
            acceptButton.onClick.AddListener(Accept);
            declineButton.onClick.AddListener(Decline);
        }

        public void Show(Action<Result> resultCallback, Action onShowComplete, bool instantly = false)
        {
            SetResultCallback(resultCallback);

            base.Show(instantly, onShowComplete);
        }

        public void Hide(Result result, Action onHideComplete, bool instantly = false)
        {
            InvokeAndClearResultCallback(result);

            base.Hide(instantly, onHideComplete);
        }

        protected sealed override void Show(bool instantly, Action onComplete)
        {
            Debug.LogWarning("Trying to show " + nameof(UiConfirmationDialog) + " without result callback. " +
                "Use the overload of \"Show()\" method with \"resultCallback\" parameter instead.");

            Show(null, onComplete, instantly);
        }

        protected sealed override void Hide(bool instantly, Action onComplete)
        {
            Hide(Result.Hide, onComplete, instantly);
        }

        // [ContextMenu("Accept")]
        protected void Accept()
        {
            Accepted?.Invoke();

            Hide(Result.Accept, null);
        }

        // [ContextMenu("Decline")]
        protected void Decline()
        {
            Declined?.Invoke();

            Hide(Result.Decline, null);
        }

        protected void SetResultCallback(Action<Result> resultCallback)
        {
            _resultCallback = resultCallback;
        }

        protected void InvokeAndClearResultCallback(Result result)
        {
            _resultCallback?.Invoke(result);
            _resultCallback = null;
        }

        public enum Result
        {
            Accept,
            Decline,
            Hide
        }
    }
}