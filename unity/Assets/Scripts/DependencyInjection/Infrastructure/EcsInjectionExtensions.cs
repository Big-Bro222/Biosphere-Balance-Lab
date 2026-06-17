using Unity.Entities;

namespace BioSphereLab.DependencyInjection.Infrastructure
{
    public static class EcsInjectionExtensions
    {
        public static bool TryGetInjectionContainer(this World p_world, out IInjectionContainer o_container)
        {
            return EcsInjectionRegistry.TryGetContainer(p_world, out o_container);
        }

        public static bool TryResolve<TService>(this World p_world, out TService o_service, string p_key = null)
            where TService : class
        {
            return EcsInjectionRegistry.TryResolve(p_world, out o_service, p_key);
        }
    }
}
