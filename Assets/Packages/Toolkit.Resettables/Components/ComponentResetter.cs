using System;
using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Resettables.Components
{
    public abstract class ComponentResetter<T> : MonoBehaviour, IResetter<T> where T : Component
    {
        protected T Component;

        private List<IResettable> resettables;

        protected void Awake()
        {
            Component = GetComponent<T>();
            
            resettables = GetResettables();
        }

        protected abstract List<IResettable> GetResettables();

        public void Reset()
        {
            foreach (IResettable resettable in resettables) 
                resettable.Reset();
        }
    }
}