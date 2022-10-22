using UnityEngine;

namespace Dirty.Test
{
    public class PositionCopier : MonoBehaviour
    {
        [SerializeField]
        private Transform _transformToCopy;
        [SerializeField]
        private Space _space = Space.World;
        [SerializeField]
        private Vector3 _offset;
        [SerializeField]
        private bool _onUpdate = true;
        [SerializeField]
        private bool _onStart = false;
        [SerializeField]
        private bool _x = true;
        [SerializeField]
        private bool _y = false;
        [SerializeField]
        private bool _z = true;

        private void Start()
        {
            if (_onStart)
                Copy();
        }

        private void Update()
        {
            if (_onUpdate)
                Copy();
        }

        public void Copy()
        {
            if (_space == Space.World)
                transform.position = new Vector3(
                    _x ? _transformToCopy.position.x + _offset.x : transform.position.x,
                    _y ? _transformToCopy.position.y + _offset.y : transform.position.y,
                    _z ? _transformToCopy.position.z + _offset.z : transform.position.z);
            else
                transform.localPosition = new Vector3(
                    _x ? _transformToCopy.localPosition.x + _offset.x : transform.localPosition.x,
                    _y ? _transformToCopy.localPosition.y + _offset.y : transform.localPosition.y,
                    _z ? _transformToCopy.localPosition.z + _offset.z : transform.localPosition.z);
        }
    }
}