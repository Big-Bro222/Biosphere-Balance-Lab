using UnityEngine;

namespace BioSphereLab.DependencyInjection.Infrastructure
{
    public interface IAssetStorage
    {
        bool TryGetPrefab(string p_key, out GameObject o_prefab);

        GameObject GetPrefab(string p_key);

        GameObject Instantiate(string p_key, Vector3 p_position, Quaternion p_rotation, Transform p_parent = null);

        bool TryGetMaterial(string p_key, out Material o_material);

        Material GetMaterial(string p_key);

        bool TryGetMesh(string p_key, out Mesh o_mesh);

        Mesh GetMesh(string p_key);
    }
}
