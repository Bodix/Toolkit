using System;
using Evolutex.Evolunity.Attributes;
using UnityEngine;

namespace _WIP.Test
{
    [Serializable]
    public class _TestClass
    {
        [SerializeField]
        private int _someInt;
        [SerializeReference, TypeSelector]
        private _NestedTestClass _someObj;

        [Serializable]
        public class _NestedTestClass : _TestClass
        {
        }
    }
}