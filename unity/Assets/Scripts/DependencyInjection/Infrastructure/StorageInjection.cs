using System;
using System.Collections.Generic;
using UnityEngine;

namespace BioSphereLab.DependencyInjection.Infrastructure
{
    [CreateAssetMenu(
        fileName = "StorageInjection",
        menuName = "BioSphereLab/Dependency Injection/Storage Injection")]
    public sealed class StorageInjection : ScriptableObject, IPrefabStorage
    {
        [SerializeField] private List<PrefabEntry> prefabs = new();

        public bool TryGetPrefab(string key, out GameObject prefab)
        {
            string normalizedKey = NormalizeKey(key);

            foreach (PrefabEntry entry in prefabs)
            {
                if (entry.Matches(normalizedKey))
                {
                    prefab = entry.Prefab;
                    return prefab != null;
                }
            }

            prefab = null;
            return false;
        }

        public GameObject GetPrefab(string key)
        {
            if (TryGetPrefab(key, out GameObject prefab))
            {
                return prefab;
            }

            throw new KeyNotFoundException($"No prefab registered in StorageInjection with key '{key}'.");
        }

        public GameObject Instantiate(string key, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            GameObject prefab = GetPrefab(key);
            return UnityEngine.Object.Instantiate(prefab, position, rotation, parent);
        }

        private static string NormalizeKey(string key)
        {
            return string.IsNullOrWhiteSpace(key) ? string.Empty : key.Trim();
        }

        [Serializable]
        private sealed class PrefabEntry
        {
            [SerializeField] private string key;
            [SerializeField] private GameObject prefab;

            public GameObject Prefab => prefab;

            public bool Matches(string candidateKey)
            {
                return string.Equals(NormalizeKey(key), candidateKey, StringComparison.Ordinal);
            }
        }
    }
}
