using System.Collections.Generic;
using BioSphereLab.DependencyInjection.Examples;
using BioSphereLab.DependencyInjection.Infrastructure;
using BioSphereLab.Systems;
using BioSphereLab.Systems.Presentation;
using Unity.Entities;
using UnityEngine;

namespace BioSphereLab
{
    public static class GameBootstrapper
    {
        private static readonly Dictionary<World, IInjectionContainer> m_containersByWorld = new();

        // Unity can keep static fields alive between Play Mode sessions when Domain Reload is disabled.
        // Reset bootstrap state before scene loading so each Play Mode session starts cleanly.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetBootstrapState()
        {
            m_containersByWorld.Clear();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Bootstrap()
        {
            World world = GetOrCreateDefaultWorld();
            RegisterWorld(world);
        }

        private static World GetOrCreateDefaultWorld()
        {
            World world = World.DefaultGameObjectInjectionWorld;

            if (world == null)
            {
                world = DefaultWorldInitialization.Initialize("Default World");
            }
            return world;
        }

        private static void RegisterWorld(World p_world)
        {
            if (m_containersByWorld.ContainsKey(p_world))
            {
                return;
            }

            IInjectionContainer container = GetOrCreateContainer();
            RegisterDependencies(container);
            RegisterSystems(p_world, container);

            m_containersByWorld[p_world] = container;
        }

        private static IInjectionContainer GetOrCreateContainer()
        {
            InjectionScope scope = Object.FindAnyObjectByType<InjectionScope>();
            return scope != null ? scope.Container : new InjectionContainer();
        }

        private static void RegisterDependencies(IInjectionContainer p_container)
        {
            p_container.Register<ICreatureSpawnConfig, ConstCreatureSpawnConfig>();
            p_container.Register<IBiosphereSpawnPolicy, BalancedBiosphereSpawnPolicy>();
        }
        
        private static void RegisterSystems(World p_world, IInjectionContainer p_container)
        {
            p_container.RegisterSystem<ICreatureMetadataSystem, CreatureMetadataSystem, InitializationSystemGroup>(p_world);
            p_container.RegisterSystem<ICreatureInitSystem, CreatureInitSystem, InitializationSystemGroup>(p_world);
            p_container.RegisterSystem<ICreaturePresentationSystem, CreaturePresentationSystem, PresentationSystemGroup>(p_world);
        }
    }
}
