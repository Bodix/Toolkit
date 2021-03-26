using System;
using UnityEngine;

namespace Selectable
{
    // TODO:
    // Refactoring.
    
    public class ParentController : MonoBehaviour
    {
        public GameObject ControllableObject { get; private set; }

        private Transform previousParent;

        public event Action<GameObject> Select;
        public event Action<GameObject> Unselect;

        public void SelectObject(GameObject gameObj, bool snapToObject = true)
        {
            if (gameObj == ControllableObject)
                return;
            
            UnselectObject();

            if (snapToObject)
            {
                transform.position = gameObj.transform.position;
                transform.rotation = gameObj.transform.rotation;
            }

            previousParent = gameObj.transform.parent;
            gameObj.transform.SetParent(transform);
            gameObj.GetComponent<ISelectable>()?.Select();

            Select?.Invoke(gameObj);
            ControllableObject = gameObj;
        }

        public void UnselectObject()
        {
            if (ControllableObject)
            {
                ControllableObject.transform.SetParent(previousParent);
                ControllableObject.GetComponent<ISelectable>()?.Unselect();
                
                Unselect?.Invoke(ControllableObject);
                ControllableObject = null;
            }
        }
    }
}