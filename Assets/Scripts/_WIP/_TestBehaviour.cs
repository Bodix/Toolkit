using System;
using UnityEngine;
using Sirenix.OdinInspector;

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
        
                
        [Serializable]
        public class _TestBehaviour2 : _TestBehaviour
        {
        
        }
    }
}