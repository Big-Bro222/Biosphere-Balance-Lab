using System.Collections.Generic;
using Unity.Entities;

namespace BioSphereLab.DependencyInjection.Infrastructure
{
    public static class EcsInjectionRegistry
    {
        private static readonly Dictionary<World, IInjectionContainer> ContainersByWorld = new();

        public static void Register(World world, IInjectionContainer container)
        {
            if (world == null || container == null)
            {
                return;
            }

            ContainersByWorld[world] = container;
        }

        public static void Unregister(World world, IInjectionContainer container)
        {
            if (world == null)
            {
                return;
            }

            if (ContainersByWorld.TryGetValue(world, out IInjectionContainer registeredContainer) &&
                ReferenceEquals(registeredContainer, container))
            {
                ContainersByWorld.Remove(world);
            }
        }

        public static bool TryGetContainer(World world, out IInjectionContainer container)
        {
            if (world != null && ContainersByWorld.TryGetValue(world, out container))
            {
                return true;
            }

            container = null;
            return false;
        }

        public static bool TryResolve<TService>(World world, out TService service, string key = null)
            where TService : class
        {
            if (TryGetContainer(world, out IInjectionContainer container))
            {
                return container.TryResolve(out service, key);
            }

            service = null;
            return false;
        }
    }
}
