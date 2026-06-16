using BioSphereLab.DependencyInjection.Examples;
using BioSphereLab.DependencyInjection.Infrastructure;
using Unity.Entities;
using UnityEngine;

namespace BioSphereLab
{
    public static class GameBootstrapper
    {
        private static bool dependenciesRegistered;
        private static bool exampleSystemRegistered;

        // Unity can keep static fields alive between Play Mode sessions when Domain Reload is disabled.
        // Reset these bootstrap guards before scene loading so each new ECS World receives a fresh DI
        // container registration and a fresh ExampleSystem registration.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetBootstrapState()
        {
            dependenciesRegistered = false;
            exampleSystemRegistered = false;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Bootstrap()
        {
            World world = GetOrCreateDefaultWorld();

            RegisterDependencies(world);
            RegisterExampleSystem(world);
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

        private static void RegisterDependencies(World world)
        {
            if (dependenciesRegistered)
            {
                return;
            }

            InjectionContainer container = new InjectionContainer();
            container.Register<IInjectionContainer>(container);
            container.Register<IBiosphereSpawnPolicy>(new BalancedBiosphereSpawnPolicy(), "balanced");
            container.Register<IBiosphereSpawnPolicy>(new StressTestBiosphereSpawnPolicy(), "stress");
            container.Switch<IBiosphereSpawnPolicy>("balanced");

            EcsInjectionRegistry.Register(world, container);

            dependenciesRegistered = true;
        }

        private static void RegisterExampleSystem(World world)
        {
            if (exampleSystemRegistered)
            {
                return;
            }

            ExampleSystem exampleSystem = world.GetOrCreateSystemManaged<ExampleSystem>();
            InitializationSystemGroup initializationSystemGroup = world.GetOrCreateSystemManaged<InitializationSystemGroup>();
            initializationSystemGroup.AddSystemToUpdateList(exampleSystem);
            initializationSystemGroup.SortSystems();

            exampleSystemRegistered = true;
        }
    }
}
