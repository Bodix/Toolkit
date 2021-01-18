using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Dirty.Editor
{
    // https://gist.github.com/yasirkula/0b541b0865eba11b55518ead45fba8fc
    
    [InitializeOnLoad]
    public static class UnfoldHierarchyObjects
    {
        private static readonly List<string> _gameObjectsToUnfold = new List<string>()
        {
            "Player",
            "Logic",
            "Environment"
        };

        static UnfoldHierarchyObjects()
        {
            if (!SessionState.GetBool("ProjectOpened", false))
            {
                SessionState.SetBool("ProjectOpened", true);
            
                // SubscribeToSceneLoaded();
            }
        }

        private static void SubscribeToSceneLoaded()
        {
            EditorSceneManager.sceneOpened += (scene, mode) =>
            {
                List<GameObject> gameObjects = new List<GameObject>();
                _gameObjectsToUnfold.ForEach(gameObjectToUnfold => gameObjects.Add(GameObject.Find(gameObjectToUnfold)));
            
                foreach (GameObject gameObject in gameObjects) 
                    SceneHierarchyUtility.SetExpanded(gameObject, true);
            };
        }
    }
}