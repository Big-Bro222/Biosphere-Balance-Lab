using BioSphereLab.DependencyInjection.Infrastructure;
using Unity.Entities;
using UnityEngine;

namespace BioSphereLab.DependencyInjection.Examples
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class DependencyInjectionEcsExampleSystem : InjectedSystemBase
    {
        private bool m_logged;

        protected override void OnUpdate()
        {
            if (m_logged || !TryResolve<IBiosphereSpawnPolicy>(out IBiosphereSpawnPolicy o_policy))
            {
                return;
            }

            Debug.Log(
                $"ECS DI example resolved '{o_policy.Name}' policy in {World.Name}: plants={o_policy.InitialPlants}, animals={o_policy.InitialAnimals}.");
            m_logged = true;
            Enabled = false;
        }
    }
}
