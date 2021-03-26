using UnityEngine;

namespace ArNavigation
{
    public class ScreenSleep : MonoBehaviour
    {
        private void Awake()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
    }
}