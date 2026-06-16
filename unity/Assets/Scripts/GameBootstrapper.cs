using System.Collections.Generic;
using BioSphereLab.DependencyInjection.Examples;
using BioSphereLab.DependencyInjection.Infrastructure;
using BioSphereLab.Systems;
using Unity.Entities;
using UnityEngine;

namespace BioSphereLab
{
    public static class GameBootstrapper
    {
        private static readonly Dictionary<World, InjectionContainer> ContainersByWorld = new();

        // Unity can keep static fields alive between Play Mode sessions when Domain Reload is disabled.
        // Reset bootstrap state before scene loading so each Play Mode session starts cleanly.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetBootstrapState()
        {
            ContainersByWorld.Clear();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
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

        private static void RegisterWorld(World world)
        {
            if (ContainersByWorld.ContainsKey(world))
            {
                return;
            }

            InjectionContainer container = new InjectionContainer();
            RegisterDependencies(container);
            RegisterSystems(world, container);

            ContainersByWorld[world] = container;
        }

        private static void RegisterDependencies(InjectionContainer container)
        {
            container.Register<ICreatureSpawnConfig, ConstCreatureSpawnConfig>();
            container.Register<IBiosphereSpawnPolicy, BalancedBiosphereSpawnPolicy>();
        }
        
        private static void RegisterSystems(World world, InjectionContainer container)
        {
            container.RegisterSystem<ICreatureInitSystem, CreatureInitSystem, InitializationSystemGroup>(world);
        }
    }
}
