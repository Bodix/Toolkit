using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Dirty.Test
{
    public class DelayedEvent : MonoBehaviour
    {
        public UnityEvent Event;

        [SerializeField]
        private InvokeMethod invokeMethod;
        
        private enum InvokeMethod
        {
            Awake,
            Start
        }

        private void Start()
        {
            StartCoroutine(DelayedCall());   
        }

        private IEnumerator DelayedCall()
        {
            yield return null;
            Event.Invoke();
        }
    }
}
