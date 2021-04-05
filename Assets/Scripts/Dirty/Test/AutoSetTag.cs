using UnityEngine;

namespace Dirty.Test
{
    [ExecuteAlways]
    public class AutoSetTag : MonoBehaviour
    {
        public string Tag;
        
        private bool setTag;
        
        private void OnValidate()
        {
            if (Application.isEditor && !Application.isPlaying)
            {
                if (!CompareTag(Tag))
                    setTag = true;
            }
        }

        private void LateUpdate()
        {
            // We can't just set tag in OnValidate due to the following warning:
            // SendMessage cannot be called during Awake, CheckConsistency, or OnValidate
            // https://forum.unity.com/threads/sendmessage-cannot-be-called-during-awake-checkconsistency-or-onvalidate-can-we-suppress.537265/
            if (setTag)
            {
                tag = Tag;

                setTag = false;
            }
        }
    }
}