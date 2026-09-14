using UnityEngine;
using UnityEngine.Events;

namespace Dirty.Modules.Input
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