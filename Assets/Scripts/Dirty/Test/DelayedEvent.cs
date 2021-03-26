using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Dirty.Test
{
    public class DelayedEvent : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent _event;

        void Start()
        {
            StartCoroutine(DelayedCall());   
        }

        private IEnumerator DelayedCall()
        {
            yield return null;
            _event.Invoke();
        }
    }
}
