using BiosphereBalanceLab.Infrastructure.DependencyInjection;
using Unity.Entities;
using UnityEngine;

namespace BiosphereBalanceLab.Examples.DependencyInjection
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [DisableAutoCreation]
    public partial class ExampleSystem : SystemBase
    {
        private bool logged;
        private bool missingDependencyLogged;

        protected override void OnUpdate()
        {
            if (logged)
            {
                Enabled = false;
                return;
            }

            bool resolved = EcsInjectionRegistry.TryResolve(
                World,
                out IBiosphereSpawnPolicy policy);

            if (!resolved)
            {
                if (!missingDependencyLogged)
                {
                    Debug.Log("ExampleSystem did not find DI yet. Make sure GameBootstrapper registered dependencies for the ECS world.");
                    missingDependencyLogged = true;
                }

                return;
            }

            Debug.Log(
                $"ExampleSystem resolved DI policy '{policy.Name}' in {World.Name}: plants={policy.InitialPlants}, animals={policy.InitialAnimals}.");

            logged = true;
            Enabled = false;
        }
    }
}
