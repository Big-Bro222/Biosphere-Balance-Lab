using BioSphereLab.DependencyInjection.Infrastructure;
using UnityEngine;
using UnityEngine.Serialization;

namespace BioSphereLab.DependencyInjection.Examples
{
    public sealed class DependencyInjectionUsageExample : MonoBehaviour
    {
        [FormerlySerializedAs("scope")]
        [SerializeField] private InjectionScope m_scope;
        [FormerlySerializedAs("useStressPolicy")]
        [SerializeField] private bool m_useStressPolicy;
        [FormerlySerializedAs("examplePrefabKey")]
        [SerializeField] private string m_examplePrefabKey = "plant";

        private void Start()
        {
            if (m_scope == null)
            {
                m_scope = FindAnyObjectByType<InjectionScope>();
            }

            if (m_scope == null)
            {
                Debug.LogWarning("No InjectionScope found. Add one to the scene before running this example.");
                return;
            }

            IInjectionContainer container = m_scope.Container;

            if (m_useStressPolicy)
            {
                container.Switch<IBiosphereSpawnPolicy>("stress");
            }

            IBiosphereSpawnPolicy policy = container.Resolve<IBiosphereSpawnPolicy>();
            Debug.Log(
                $"DI example using '{policy.Name}' policy: plants={policy.InitialPlants}, animals={policy.InitialAnimals}.");

            if (container.TryResolve<IAssetStorage>(out IAssetStorage o_assetStorage) &&
                o_assetStorage.TryGetPrefab(m_examplePrefabKey, out GameObject o_prefab))
            {
                Debug.Log($"StorageInjection resolved prefab '{o_prefab.name}' with key '{m_examplePrefabKey}'.");
            }
        }
    }
}
