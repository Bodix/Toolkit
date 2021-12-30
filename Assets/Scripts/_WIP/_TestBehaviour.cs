using Sirenix.OdinInspector;
using UnityEngine;

namespace _WIP
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

        [Button]
        public void Test2()
        {
        }
    }
}