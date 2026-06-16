using BioSphereLab.DependencyInjection.Infrastructure;
using UnityEngine;

namespace BioSphereLab.DependencyInjection.Examples
{
    public sealed class DependencyInjectionUsageExample : MonoBehaviour
    {
        [SerializeField] private InjectionScope scope;
        [SerializeField] private bool useStressPolicy;
        [SerializeField] private string examplePrefabKey = "plant";

        private void Start()
        {
            if (scope == null)
            {
                scope = FindAnyObjectByType<InjectionScope>();
            }

            if (scope == null)
            {
                Debug.LogWarning("No InjectionScope found. Add one to the scene before running this example.");
                return;
            }

            IInjectionContainer container = scope.Container;

            if (useStressPolicy)
            {
                container.Switch<IBiosphereSpawnPolicy>("stress");
            }

            IBiosphereSpawnPolicy policy = container.Resolve<IBiosphereSpawnPolicy>();
            Debug.Log(
                $"DI example using '{policy.Name}' policy: plants={policy.InitialPlants}, animals={policy.InitialAnimals}.");

            if (container.TryResolve<IPrefabStorage>(out IPrefabStorage prefabStorage) &&
                prefabStorage.TryGetPrefab(examplePrefabKey, out GameObject prefab))
            {
                Debug.Log($"StorageInjection resolved prefab '{prefab.name}' with key '{examplePrefabKey}'.");
            }
        }
    }
}
