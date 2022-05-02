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

        public void Show(Action<Result> resultCallback, bool instant = false)
        {
            SetResultCallback(resultCallback);

            base.Show(instant);
        }

        public void Hide(Result result, bool instant = false)
        {
            InvokeAndClearResultCallback(result);

            base.Hide(instant);
        }

        protected sealed override void Show(bool instantly)
        {
            Show(null, instantly);
        }

        protected sealed override void Hide(bool instantly)
        {
            Hide(Result.Close, instantly);
        }

        protected void Accept()
        {
            Accepted?.Invoke();

            Hide(Result.Accept);
        }

        protected void Decline()
        {
            Declined?.Invoke();

            Hide(Result.Decline);
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
            Close
        }
    }
}