using Unity.Entities;

namespace BiosphereBalanceLab.Infrastructure.DependencyInjection
{
    public static class EcsInjectionExtensions
    {
        public static bool TryGetInjectionContainer(this World world, out IInjectionContainer container)
        {
            return EcsInjectionRegistry.TryGetContainer(world, out container);
        }

        public static bool TryResolve<TService>(this World world, out TService service, string key = null)
            where TService : class
        {
            return EcsInjectionRegistry.TryResolve(world, out service, key);
        }
    }
}
