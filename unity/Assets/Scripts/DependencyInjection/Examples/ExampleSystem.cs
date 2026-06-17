using BioSphereLab.DependencyInjection.Infrastructure;
using Unity.Entities;
using UnityEngine;

namespace BioSphereLab.DependencyInjection.Examples
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [DisableAutoCreation]
    public partial class ExampleSystem : SystemBase
    {
        private bool m_logged;
        private bool m_missingDependencyLogged;

        protected override void OnUpdate()
        {
            if (m_logged)
            {
                Enabled = false;
                return;
            }

            bool resolved = EcsInjectionRegistry.TryResolve(
                World,
                out IBiosphereSpawnPolicy o_policy);

            if (!resolved)
            {
                if (!m_missingDependencyLogged)
                {
                    Debug.Log("ExampleSystem did not find DI yet. Make sure GameBootstrapper registered dependencies for the ECS world.");
                    m_missingDependencyLogged = true;
                }

                return;
            }

            Debug.Log(
                $"ExampleSystem resolved DI policy '{o_policy.Name}' in {World.Name}: plants={o_policy.InitialPlants}, animals={o_policy.InitialAnimals}.");

            m_logged = true;
            Enabled = false;
        }
    }
}
