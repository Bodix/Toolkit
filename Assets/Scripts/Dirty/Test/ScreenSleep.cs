using UnityEngine;

namespace Dirty.Test
{
    public class ScreenSleep : MonoBehaviour
    {
        private void Awake()
        {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
    }
}