using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _WIP.Test
{
    public class _TestBehaviour : MonoBehaviour
    {
        [SerializeField]
        private _Test test;

        private void Awake()
        {
            // Test();
        }

        [Button]
        public void Test()
        {

        }

        [Button]
        public void Test2()
        {
            
        }


        [Serializable]
        public class _NestedTestBehaviour : _TestBehaviour
        {
        }
    }
}