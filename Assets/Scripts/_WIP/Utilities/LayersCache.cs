using System.Collections.Generic;
using _WIP.Utilities.Dictionary;
using NaughtyAttributes;
using UnityEngine;

namespace _WIP.Utilities
{
    // TODO: Make handling null references.
    // TODO: Make generic base class.
    // TODO: Make layer drawer.

    public class LayersCache : MonoBehaviour
    {
        public GameObject[] Targets;
        public LayersCacheMethod CacheMethod = LayersCacheMethod.Start;
        [SerializeField, Unity.Collections.ReadOnly]
        private GenericDictionary<GameObject, int> _cache = new GenericDictionary<GameObject, int>();

        private void Awake()
        {
            if (CacheMethod == LayersCacheMethod.Awake)
                CacheLayers();
        }

        private void Start()
        {
            if (CacheMethod == LayersCacheMethod.Start)
                CacheLayers();
        }

        private void OnValidate()
        {
            if (CacheMethod == LayersCacheMethod.OnValidate)
                CacheLayers();
        }

        [ContextMenu("Cache")]
        [Button("Cache")]
        public void CacheLayers()
        {
            _cache.Clear();
            foreach (GameObject target in Targets)
                _cache.Add(target, target.layer);
        }

        [ContextMenu("Restore")]
        [Button("Restore")]
        public void RestoreLayers()
        {
            foreach (KeyValuePair<GameObject, int> pair in _cache)
                pair.Key.layer = pair.Value;
        }

        public void SetLayers(int layer)
        {
            foreach (GameObject target in Targets)
                target.layer = layer;
        }
    }

    public enum LayersCacheMethod
    {
        Awake,
        Start,
        OnValidate,
        Manual
    }
}