using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AI.Vision
{
    public class AiSmartVision : AiVision
    {
        public void LookForCharacters(out Character[] characters)
        {
            LookConical(out IEnumerable<Collider> colliders);

            characters = colliders.Select(x => x.GetComponent<Character>()).ToArray();
        }
    }
}