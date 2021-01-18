using Evolutex.Evolunity.Structs;
using UnityEngine;

namespace Dirty.Test
{
    public class _TestBehaviour : MonoBehaviour
    {
        [SerializeField]
        private _Test test;
        [SerializeField]
        private IntRange range1;
        [SerializeField]
        private IntRange range2;

        [Sirenix.OdinInspector.Button]
        public void Test()
        {
            Debug.Log(range1 == range2);
        }
    }
}