using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Other;
using UnityEngine;

namespace Providers
{
    [CreateAssetMenu(fileName = "PrefabsHolderSo", menuName = "ScriptableObjects/PrefabsHolderSo")]
    public class PrefabsHolderSo : ScriptableObject, IPrefabHolder
    {
        [LabeledArray(nameof(PrefabHolderItem.Key))] [SerializeField] private PrefabHolderItem[] _items;

        private readonly Dictionary<string, GameObject> _coldPrefabsCache = new Dictionary<string, GameObject>();
        
        private Dictionary<PrefabKey, GameObject> _prefabsCache;
        
        public PrefabHolderItem[] Items => _items;

        public void Init()
        {
            _prefabsCache = _items.ToDictionary(i => i.Key, i => i.Prefab);
        }

        public GameObject GetPrefabByKey(PrefabKey key)
        {
            return _prefabsCache[key];
        }
        
        public GameObject GetColdPrefab(string coldPrefabPath)
        {
            if (_coldPrefabsCache.TryGetValue(coldPrefabPath, out var coldPrefab))
            {
                return coldPrefab;
            }
            
            var prefab = Resources.Load<GameObject>(coldPrefabPath);
            if (prefab != null)
            {
                _coldPrefabsCache.Add(coldPrefabPath, prefab);
            }
            else
            {
                UnityEngine.Debug.LogError("Prefab not found at path: " + coldPrefabPath);
            }

            return _coldPrefabsCache[coldPrefabPath];
        }

        public void UnloadColdPrefabs()
        {
            Resources.UnloadUnusedAssets();
            
            _coldPrefabsCache.Clear();
        }

        [Serializable]
        public struct PrefabHolderItem
        {
            public PrefabKey Key;
            public GameObject Prefab;
        }
    }
        
    public interface IPrefabHolder
    {
        public GameObject GetPrefabByKey(PrefabKey key);
        
        public GameObject GetColdPrefab(string coldPrefabPath);
        public void UnloadColdPrefabs();
    }
}
