using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AI
{
    public class AiSmartVision : AiVision
    {
        public HashSet<Character> CharactersInRange;
        private HashSet<Character> _prevCharactersInRange = new HashSet<Character>();

        public event Action<Character> CharacterFound;
        public event Action<Character> CharacterLost;
        
        protected override void OnCollidersOverlap(IEnumerable<Collider> colliders)
        {
            CharactersInRange = new HashSet<Character>(colliders.Select(x => x.GetComponent<Character>()));

            IEnumerable<Character> removedCharacters = _prevCharactersInRange.Except(CharactersInRange);
            foreach (Character character in removedCharacters)
                CharacterLost?.Invoke(character);
            
            IEnumerable<Character> addedCharacters = CharactersInRange.Except(_prevCharactersInRange);
            foreach (Character character in addedCharacters)
                CharacterFound?.Invoke(character);

            _prevCharactersInRange = CharactersInRange;
        }
    }
}