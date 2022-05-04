using System;
using UnityEngine;

namespace AI
{
    public class AiMovement : MonoBehaviour
    {
        public float MoveSpeed = 1;

        private Transform _target;
        private bool _isFollowing;

        private void Update()
        {
            if(_isFollowing)
                MoveTo(_target);
        }

        public void StartFollowing(Transform target)
        {
            _target = target;
            _isFollowing = true;
        }

        public void StopFollowing()
        {
            _target = null;
            _isFollowing = true;
        }

        public void MoveTo(Transform target)
        {
            transform.LookAt(target);
            transform.position = Vector3.MoveTowards(transform.position, target.position, MoveSpeed);
        }
    }
}