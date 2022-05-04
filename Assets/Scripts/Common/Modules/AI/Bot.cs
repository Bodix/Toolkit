using System;
using UnityEngine;

namespace AI
{
    public class Bot : Character
    {
        [SerializeField]
        private AiSmartVision _vision;
        [SerializeField]
        private AiMovement _movement;

        private Character _target;

        private void Awake()
        {
            _vision.CharacterLost += character =>
                Debug.Log("lost" + character);
            
            _vision.CharacterFound += character =>
            {
                Debug.Log("found" + character);
                
                _movement.StartFollowing(character.transform);
            };
        }
    }
}