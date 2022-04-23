using UnityEngine;
using Evolutex.Evolunity.Extensions;
using AI.Vision;

namespace AI
{
    public class Bot : Character
    {
        [SerializeField]
        private AiSmartVision _smartVision;

        private void Update()
        {
            _smartVision.LookForCharacters(out Character[] characters);

            Debug.Log(characters.AsString());
        }
    }
}