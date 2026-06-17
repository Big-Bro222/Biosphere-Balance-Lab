using System;
using Unity.Entities;

namespace BioSphereLab.DependencyInjection.Infrastructure
{
    public interface IInjectionContainer
    {
        void Register<TService>(TService p_instance, string p_key = null) where TService : class;

        void Register<TService, TImplementation>(string p_key = null)
            where TService : class
            where TImplementation : class, TService, new();

        void RegisterFactory<TService>(Func<IInjectionContainer, TService> p_factory, string p_key = null)
            where TService : class;

        TSystem RegisterSystem<TService, TSystem, TSystemGroup>(World p_world, string p_key = null)
            where TService : class
            where TSystem : SystemBase, TService
            where TSystemGroup : ComponentSystemGroup;

        bool Has<TService>(string p_key = null) where TService : class;

        bool TryResolve<TService>(out TService o_service, string p_key = null) where TService : class;

        TService Resolve<TService>(string p_key = null) where TService : class;

        void Switch<TService>(string p_key) where TService : class;

        string GetActiveKey<TService>() where TService : class;
    }
}
