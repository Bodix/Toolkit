using System;
using Evolutex.Evolunity.Attributes;
using UnityEngine;

namespace _WIP.Test
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