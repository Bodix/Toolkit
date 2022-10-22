using UnityEngine;

namespace _WIP.FindInParent
{
    public class FindInParentAttribute : PropertyAttribute
    {
        public bool IncludeInactive { get; }

        public FindInParentAttribute(bool includeInactive = false)
        {
            IncludeInactive = includeInactive;
        }
    }
}