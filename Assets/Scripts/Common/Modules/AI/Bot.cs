using UnityEngine;

namespace AI
{
    public class Bot : Character
    {
        [SerializeField]
        private AiSmartVision _vision;
        [SerializeField]
        private AiMovement _movement;
    }
}