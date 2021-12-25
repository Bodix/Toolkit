using System.Collections.Generic;
using Evolutex.Evolunity.Extensions;
using UnityEngine;

namespace AI
{
    public class Bot : MonoBehaviour
    {
        [SerializeField]
        private AiSmartVision _smartVision;

        private void Update()
        {
            // _smartVision.LookSpherical(out IEnumerable<Collider> result);
            // _smartVision.LookConical(out IEnumerable<Collider> result);
            _smartVision.LookForCharacters(out BaseCharacter[] result);
            
            Debug.Log(result.AsString());
        }
    }
}