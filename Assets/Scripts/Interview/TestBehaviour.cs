using System;
using System.Collections.Generic;
using System.Linq;
using Evolutex.Evolunity.Extensions;
using UnityEngine;

namespace Interview
{
    public class TestBehaviour : MonoBehaviour
    {
        [SerializeField]
        private double _currentExperience = 3800;
        private Dictionary<int, double> _experienceLevelsDictionary = new Dictionary<int, double>();

        private void Awake()
        {
            _experienceLevelsDictionary = new Dictionary<int, double>()
            {
                { 1, 200 },
                { 2, 400 },
                { 3, 800 },
                { 4, 1600 },
                { 5, 3200 },
            };

            Debug.Log(GetCurrentLevel(_currentExperience, _experienceLevelsDictionary));
        }

        public int GetCurrentLevel(double currentExperience, Dictionary<int, double> experienceLevelsDictionary)
        {
            if (IsExperienceLevelsValid(experienceLevelsDictionary))
            {
                experienceLevelsDictionary = PrepareExperienceLevels(experienceLevelsDictionary);

                if (currentExperience < experienceLevelsDictionary.First().Value)
                    return 0;

                return experienceLevelsDictionary.Last(x => x.Value <= currentExperience).Key;
            }
            else
            {
                throw new InvalidOperationException("Experience levels dictionary is invalid");
            }
        }

        private Dictionary<int, double> PrepareExperienceLevels(Dictionary<int, double> experienceLevelsDictionary)
        {
            return experienceLevelsDictionary
                .OrderBy(x => x.Key)
                .ToDictionary(x => x.Key, x => x.Value);
        }

        private bool IsExperienceLevelsValid(Dictionary<int, double> experienceLevelsDictionary)
        {
            if (experienceLevelsDictionary == null)
                return false;

            if (experienceLevelsDictionary.IsEmpty())
                return false;

            // TODO: Fix double preparing dictionary in PrepareExperienceLevels. [#optimization]
            KeyValuePair<int, double>[] ex = PrepareExperienceLevels(experienceLevelsDictionary).ToArray();
            for (int i = 1; i < ex.Length; i++)
                if (ex[i].Value <= ex[i - 1].Value)
                    return false;

            return true;
        }
    }
}