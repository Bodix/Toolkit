using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace _WIP.Editor
{
    // https://gist.github.com/nicoplv/df2d1064be62cf894b202b33d050d54f
    
    [InitializeOnLoad]
    public class CleanDuplicateName
    {
        static bool eventDuplicateAppend;

        static CleanDuplicateName()
        {
            EditorApplication.hierarchyWindowItemOnGUI += (int instanceID, Rect selectionRect) =>
            {
                if(Event.current != null && Event.current.type == EventType.ExecuteCommand && (Event.current.commandName == "Duplicate" || Event.current.commandName == "Paste"))
                    eventDuplicateAppend = true;
            };

            EditorApplication.hierarchyChanged += () =>
            {
                if(eventDuplicateAppend)
                {
                    foreach (GameObject iGamebject in Selection.gameObjects)
                        iGamebject.name = Regex.Replace(iGamebject.name, @"(.*)\s\([0-9]*\)", @"$1");
                }
                eventDuplicateAppend = false;
            };
        }
    }
}