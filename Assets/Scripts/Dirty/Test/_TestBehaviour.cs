using Sirenix.OdinInspector;
using UnityEngine;

namespace Dirty.Test
{
    public class _TestBehaviour : MonoBehaviour
    {
        [SerializeField]
        private _Test test;

        [Button]
        public void Test()
        {
        }

        public void Log()
        {
            Debug.Log("Log");
        }
    }
}