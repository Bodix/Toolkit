using System;
using UnityEngine;
using Evolutex.Evolunity.Attributes;

namespace _WIP
{
    [Serializable]
    public class _Test
    {
        [SerializeField]
        private int _someInt;
        [SerializeReference, TypeSelector]
        private _Test2 _someObj;
        
        [Serializable]
        public class _Test2 : _Test
        {
        
        }
    }
}