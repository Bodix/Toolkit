using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AI
{
    public class AiSmartVision : AiVision
    {
        public IEnumerable<Character> CharactersInRange;
        
        protected override void OnCollidersOverlap(IEnumerable<Collider> colliders)
        {
            CharactersInRange = colliders.Select(x => x.GetComponent<Character>());
        }
    }
}