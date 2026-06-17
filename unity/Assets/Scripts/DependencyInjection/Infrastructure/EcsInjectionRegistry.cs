using System.Collections.Generic;
using Unity.Entities;

namespace BioSphereLab.DependencyInjection.Infrastructure
{
    public static class EcsInjectionRegistry
    {
        private static readonly Dictionary<World, IInjectionContainer> m_containersByWorld = new();

        public static void Register(World p_world, IInjectionContainer p_container)
        {
            if (p_world == null || p_container == null)
            {
                return;
            }

            m_containersByWorld[p_world] = p_container;
        }

        public static void Unregister(World p_world, IInjectionContainer p_container)
        {
            if (p_world == null)
            {
                return;
            }

            if (m_containersByWorld.TryGetValue(p_world, out IInjectionContainer o_registeredContainer) &&
                ReferenceEquals(o_registeredContainer, p_container))
            {
                m_containersByWorld.Remove(p_world);
            }
        }

        public static bool TryGetContainer(World p_world, out IInjectionContainer o_container)
        {
            if (p_world != null && m_containersByWorld.TryGetValue(p_world, out o_container))
            {
                return true;
            }

            o_container = null;
            return false;
        }

        public static bool TryResolve<TService>(World p_world, out TService o_service, string p_key = null)
            where TService : class
        {
            if (TryGetContainer(p_world, out IInjectionContainer o_container))
            {
                return o_container.TryResolve(out o_service, p_key);
            }

            o_service = null;
            return false;
        }
    }
}
