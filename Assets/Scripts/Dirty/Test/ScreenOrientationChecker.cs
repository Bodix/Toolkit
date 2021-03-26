using UnityEngine;

namespace Dirty.Test
{
    public class ScreenOrientationChecker : MonoBehaviour
    {
        [SerializeField] private ScreenOrientationManager[] screenOrientationManagers;
        private ScreenOrientations _currentScreenOrientation;

        private void Awake()
        {
            _currentScreenOrientation = ScreenOrientation;
        }

        void Update()
        {
            OnScreenOrientationChange();
        }

        public ScreenOrientations ScreenOrientation
        {
            get
            {
                return Screen.width > Screen.height ? ScreenOrientations.LANDSCAPE : ScreenOrientations.PORTRAIT;
            }
        }

        private void OnScreenOrientationChange()
        {
            if (_currentScreenOrientation != ScreenOrientation)
            {
                for (int i = 0; i <= screenOrientationManagers.Length - 1; i++)
                {
                    if (screenOrientationManagers[i].isNeedToCallActionInRuntime)
                    {
                        screenOrientationManagers[i].InvokeAction(true);
                    }
                }
                _currentScreenOrientation = ScreenOrientation;
            }
        }
    }
}
