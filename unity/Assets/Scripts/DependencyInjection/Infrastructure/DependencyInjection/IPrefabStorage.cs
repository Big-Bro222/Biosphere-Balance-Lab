using UnityEngine;

namespace BiosphereBalanceLab.Infrastructure.DependencyInjection
{
    public interface IPrefabStorage
    {
        bool TryGetPrefab(string key, out GameObject prefab);

        GameObject GetPrefab(string key);

        GameObject Instantiate(string key, Vector3 position, Quaternion rotation, Transform parent = null);
    }
}
