using UnityEngine;
using UnityEngine.Events;

namespace Dirty.Test
{
    public class ScreenOrientationManager : MonoBehaviour
    {
        public bool isNeedToCallActionInRuntime = false;
        [SerializeField] private UnityEvent _lanscapeAction;
        [SerializeField] private UnityEvent _portraitAction;

        private ScreenOrientationChecker _screenOrientationChecker;

        private void Start()
        {
            _screenOrientationChecker = FindObjectOfType<ScreenOrientationChecker>();
        }

        public void InvokeAction(bool isFromCheckerCall = false)
        {
            UnityEvent actionToInvoke = _screenOrientationChecker.ScreenOrientation == ScreenOrientations.LANDSCAPE ? _lanscapeAction : _portraitAction;
            actionToInvoke?.Invoke();
            if(isFromCheckerCall)
            {
                isNeedToCallActionInRuntime = true;
            }
        }

        public void ChangeIsNeedToCallActionInRuntime()
        {
            isNeedToCallActionInRuntime = !isNeedToCallActionInRuntime;
        }
    }
}
