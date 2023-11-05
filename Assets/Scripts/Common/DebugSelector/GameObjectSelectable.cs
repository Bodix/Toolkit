using System;
using UnityEngine;

namespace DebugSelector
{
    [Serializable]
    public struct GameObjectSelectable : ISelectable
    {
        public GameObject GameObject;

        public void Select()
        {
            GameObject.SetActive(true);
        }

        public void Unselect()
        {
            GameObject.SetActive(false);
        }
    }
}