using Sirenix.OdinInspector;
using UnityEngine;

namespace Dirty.Test
{
    public class _TestBehaviour : MonoBehaviour
    {
        [SerializeField]
        private _Test test;

        private void Awake()
        {
            Test();
        }

        [Button]
        public void Test()
        {
        }
    }
}