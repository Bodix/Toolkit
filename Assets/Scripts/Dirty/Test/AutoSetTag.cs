using UnityEngine;

namespace Dirty.Test
{
    [ExecuteAlways]
    [RequireComponent(typeof(Collider))]
    public class AutoSetTag : MonoBehaviour
    {
        private bool markSetTag;
        
        private void OnValidate()
        {
            if (Application.isEditor && !Application.isPlaying)
            {
                if (!CompareTag("SomeTag"))
                    markSetTag = true;
            }
        }

        private void LateUpdate()
        {
            // We cant just set tag in OnValidate due to this warning:
            // SendMessage cannot be called during Awake, CheckConsistency, or OnValidate
            // https://forum.unity.com/threads/sendmessage-cannot-be-called-during-awake-checkconsistency-or-onvalidate-can-we-suppress.537265/
            if (markSetTag)
            {
                tag = "SomeTag";

                markSetTag = false;
            }
        }
    }
}