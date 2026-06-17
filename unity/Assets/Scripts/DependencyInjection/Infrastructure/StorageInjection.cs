using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace BioSphereLab.DependencyInjection.Infrastructure
{
    [CreateAssetMenu(
        fileName = "StorageInjection",
        menuName = "BioSphereLab/Dependency Injection/Storage Injection")]
    public sealed class StorageInjection : ScriptableObject, IAssetStorage
    {
        [FormerlySerializedAs("prefabs")]
        [Header("Prefabs")]
        [SerializeField] private List<PrefabEntry> m_prefabs = new();
        [Header("Materials")]
        [SerializeField] private List<MaterialEntry> m_materials = new();
        [Header("Meshes")]
        [SerializeField] private List<MeshEntry> m_meshes = new();

        public bool TryGetPrefab(string p_key, out GameObject o_prefab)
        {
            string normalizedKey = NormalizeKey(p_key);

            foreach (PrefabEntry entry in m_prefabs)
            {
                if (entry.Matches(normalizedKey))
                {
                    o_prefab = entry.Prefab;
                    return o_prefab != null;
                }
            }

            o_prefab = null;
            return false;
        }

        public GameObject GetPrefab(string p_key)
        {
            if (TryGetPrefab(p_key, out GameObject o_prefab))
            {
                return o_prefab;
            }

            throw new KeyNotFoundException($"No prefab registered in StorageInjection with key '{p_key}'.");
        }

        public GameObject Instantiate(string p_key, Vector3 p_position, Quaternion p_rotation, Transform p_parent = null)
        {
            GameObject prefab = GetPrefab(p_key);
            return UnityEngine.Object.Instantiate(prefab, p_position, p_rotation, p_parent);
        }

        public bool TryGetMaterial(string p_key, out Material o_material)
        {
            string normalizedKey = NormalizeKey(p_key);

            foreach (MaterialEntry entry in m_materials)
            {
                if (entry.Matches(normalizedKey))
                {
                    o_material = entry.Material;
                    return o_material != null;
                }
            }

            o_material = null;
            return false;
        }

        public Material GetMaterial(string p_key)
        {
            if (TryGetMaterial(p_key, out Material o_material))
            {
                return o_material;
            }

            throw new KeyNotFoundException($"No material registered in StorageInjection with key '{p_key}'.");
        }

        public bool TryGetMesh(string p_key, out Mesh o_mesh)
        {
            string normalizedKey = NormalizeKey(p_key);

            foreach (MeshEntry entry in m_meshes)
            {
                if (entry.Matches(normalizedKey))
                {
                    o_mesh = entry.Mesh;
                    return o_mesh != null;
                }
            }

            o_mesh = null;
            return false;
        }

        public Mesh GetMesh(string p_key)
        {
            if (TryGetMesh(p_key, out Mesh o_mesh))
            {
                return o_mesh;
            }

            throw new KeyNotFoundException($"No mesh registered in StorageInjection with key '{p_key}'.");
        }

        private static string NormalizeKey(string p_key)
        {
            return string.IsNullOrWhiteSpace(p_key) ? string.Empty : p_key.Trim();
        }

        [Serializable]
        private sealed class PrefabEntry
        {
            [FormerlySerializedAs("key")]
            [SerializeField] private string m_key;
            [FormerlySerializedAs("prefab")]
            [SerializeField] private GameObject m_prefab;

            public GameObject Prefab => m_prefab;

            public bool Matches(string p_candidateKey)
            {
                return string.Equals(NormalizeKey(m_key), p_candidateKey, StringComparison.Ordinal);
            }
        }

        [Serializable]
        private sealed class MaterialEntry
        {
            [SerializeField] private string m_key;
            [SerializeField] private Material m_material;

            public Material Material => m_material;

            public bool Matches(string p_candidateKey)
            {
                return string.Equals(NormalizeKey(m_key), p_candidateKey, StringComparison.Ordinal);
            }
        }

        [Serializable]
        private sealed class MeshEntry
        {
            [SerializeField] private string m_key;
            [SerializeField] private Mesh m_mesh;

            public Mesh Mesh => m_mesh;

            public bool Matches(string p_candidateKey)
            {
                return string.Equals(NormalizeKey(m_key), p_candidateKey, StringComparison.Ordinal);
            }
        }
    }
}
