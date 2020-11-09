using System.Collections.Generic;
using Toolkit.Resettables.Components;
using UnityEngine;

namespace Toolkit.Resettables
{
    public class MonoBehaviourResetter : IResetter
    {
        private readonly MonoBehaviour monoBehaviour;
        private readonly List<IComponentResetter<Component>> resetters;

        public MonoBehaviourResetter(MonoBehaviour monoBehaviour)
        {
            this.monoBehaviour = monoBehaviour;
            resetters = new List<IComponentResetter<Component>>();

            Component[] components = monoBehaviour.GetComponentsInChildren<Component>(true);
            foreach (Component component in components)
            {
                if (Resettables.HasResetterForComponent(component))
                {
                    IComponentResetter<Component> componentResetter = Resettables.AddComponentResetterToGameObject(
                        component.gameObject, component);

                    resetters.Add(componentResetter);
                }
            }
        }

        public void ClearResettersWithoutComponent()
        {
            foreach (IComponentResetter<Component> resetter in resetters)
                if (!resetter.HasResettable)
                {
                    resetters.Remove(resetter);
                    
                    resetter.Destroy();
                }
        }

        public bool HasResettable => resetters.Count > 0;

        public void Reset()
        {
            foreach (IComponentResetter<Component> resetter in resetters)
                resetter.Reset();
        }
    }
}