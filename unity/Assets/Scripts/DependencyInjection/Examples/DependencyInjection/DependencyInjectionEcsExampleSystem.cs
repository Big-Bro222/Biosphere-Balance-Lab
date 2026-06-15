using BiosphereBalanceLab.Infrastructure.DependencyInjection;
using Unity.Entities;
using UnityEngine;

namespace BiosphereBalanceLab.Examples.DependencyInjection
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class DependencyInjectionEcsExampleSystem : InjectedSystemBase
    {
        private bool logged;

        protected override void OnUpdate()
        {
            if (logged || !TryResolve<IBiosphereSpawnPolicy>(out IBiosphereSpawnPolicy policy))
            {
                return;
            }

            Debug.Log(
                $"ECS DI example resolved '{policy.Name}' policy in {World.Name}: plants={policy.InitialPlants}, animals={policy.InitialAnimals}.");
            logged = true;
            Enabled = false;
        }
    }
}
