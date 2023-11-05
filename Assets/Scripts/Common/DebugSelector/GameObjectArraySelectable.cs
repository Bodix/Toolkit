using System;
using UnityEngine;

namespace DebugSelector
{
    [Serializable]
    public struct GameObjectArraySelectable : ISelectable
    {
        public GameObject[] GameObjects;

        public void Select()
        {
            foreach (GameObject gameObject in GameObjects)
                gameObject.SetActive(true);
        }

        public void Unselect()
        {
            foreach (GameObject gameObject in GameObjects)
                gameObject.SetActive(false);
        }
    }
}