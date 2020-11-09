using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Resettables.Components
{
    public abstract class ComponentResetter<T> : MonoBehaviour, IComponentResetter<T> where T : Component
    {
        public T Component { get; private set; }

        private List<IResettable> resettables;

        protected void Awake()
        {
            Component = GetComponent<T>();
            
            resettables = GetResettables();
        }

        protected abstract List<IResettable> GetResettables();

        public bool HasResettable => Component;

        public void Reset()
        {
            foreach (IResettable resettable in resettables) 
                resettable.Reset();
        }

        public void Destroy()
        {
            Destroy(this);
        }
    }
}