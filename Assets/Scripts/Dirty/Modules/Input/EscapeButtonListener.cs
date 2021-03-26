using UnityEngine;
using UnityEngine.Events;

namespace Dirty.Input
{
    public class EscapeButtonListener : MonoBehaviour
    {
        public UnityEvent onEscapeButton;
        
        private void Update()
        {
            if (UnityEngine.Input.GetKey(KeyCode.Escape) 
                && enabled && gameObject.activeInHierarchy)
                onEscapeButton?.Invoke();
        }
    }
}