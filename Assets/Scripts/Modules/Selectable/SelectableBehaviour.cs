using UnityEngine;

namespace Selectable
{
    public abstract class SelectableBehaviour : MonoBehaviour, ISelectable
    {
        public abstract void Select();

        public abstract void Unselect();
    }
}