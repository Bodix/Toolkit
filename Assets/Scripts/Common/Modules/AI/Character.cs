using UnityEngine;

namespace AI
{
    public class Character : MonoBehaviour
    {
        public float MoveSpeed;
        
        public void MoveTo(Transform target)
        {
            transform.LookAt(target);
            transform.position = Vector3.MoveTowards(transform.position, target.position, MoveSpeed);
        }
    }
}