using UnityEngine;

namespace Toolkit.Resettables.Test
{
    public class Test : MonoBehaviour
    {
        private MonoBehaviourResetter resetter;
        
        private void Awake()
        {
            resetter = new MonoBehaviourResetter(this);
        }

        private void Start()
        {
            GetComponent<TrailRenderer>().startColor = Color.black;
        }

        [ContextMenu("Reset")]
        public void DoReset()
        {
            resetter.Reset();
        }

        [ContextMenu("Remove")]
        public void Remove()
        {
            Destroy(GetComponent<TrailRenderer>());
        }
    }
}