using System;
using System.Collections.Generic;
using UnityEngine;

namespace _WIP.Utilities
{
    /// <summary>
    /// http://answers.unity.com/answers/809221/view.html
    ///
    /// Unity can't serialize Dictionary so here's a custom wrapper that does. Note that you have to
    /// extend it before it can be serialized as Unity won't serialized generic-based types either.
    /// </summary>
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<TKey> _keys = new List<TKey>();
        [SerializeField]
        private List<TValue> _values = new List<TValue>();

        public void OnBeforeSerialize()
        {
            _keys.Clear();
            _values.Clear();

            foreach (KeyValuePair<TKey, TValue> pair in this)
            {
                _keys.Add(pair.Key);
                _values.Add(pair.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            Clear();
            for (int i = 0; i < _keys.Count; i++)
                Add(_keys[i], _values[i]);

            _keys.Clear();
            _values.Clear();
        }
    }

    [Serializable]
    public class GameObjectIntDictionary : SerializableDictionary<GameObject, int>
    {
    }
}