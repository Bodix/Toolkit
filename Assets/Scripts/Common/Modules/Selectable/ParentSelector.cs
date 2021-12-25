using System;
using UnityEngine;

namespace Selectable
{
    public class ParentSelector : MonoBehaviour
    {
        public GameObject SelectedObject { get; private set; }

        private Transform previousParent;

        public event Action<GameObject> Select;
        public event Action<GameObject> Unselect;

        public void SelectObject(GameObject gameObj, bool stickToObject = true)
        {
            if (gameObj == SelectedObject)
                return;
            
            UnselectObject();

            if (stickToObject)
            {
                transform.position = gameObj.transform.position;
                transform.rotation = gameObj.transform.rotation;
            }

            previousParent = gameObj.transform.parent;
            gameObj.transform.SetParent(transform);
            gameObj.GetComponent<ISelectable>()?.Select();

            Select?.Invoke(gameObj);
            SelectedObject = gameObj;
        }

        public void UnselectObject()
        {
            if (SelectedObject)
            {
                SelectedObject.transform.SetParent(previousParent);
                SelectedObject.GetComponent<ISelectable>()?.Unselect();
                
                Unselect?.Invoke(SelectedObject);
                SelectedObject = null;
            }
        }
    }
}