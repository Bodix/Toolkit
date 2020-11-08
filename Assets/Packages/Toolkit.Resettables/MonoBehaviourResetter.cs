using System.Collections.Generic;
using UnityEngine;

namespace Toolkit.Resettables
{
    public class MonoBehaviourResetter : IResetter
    {
        private readonly MonoBehaviour monoBehaviour;
        private readonly List<IResetter> resetters;

        public MonoBehaviourResetter(MonoBehaviour monoBehaviour)
        {
            this.monoBehaviour = monoBehaviour;
            resetters = new List<IResetter>();

            Component[] components = monoBehaviour.GetComponentsInChildren<Component>(true);
            foreach (Component component in components)
            {
                if (Resettables.HasResetterForComponent(component))
                {
                    IResetter<Component> resetter =
                        Resettables.AddResetterToGameObject(component.gameObject, component);
                    resetters.Add(resetter);
                }
            }
        }

        public void Reset()
        {
            foreach (IResetter resetter in resetters)
                resetter.Reset();
        }
    }
}