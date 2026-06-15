using System;

namespace BiosphereBalanceLab.Infrastructure.DependencyInjection
{
    public interface IInjectionContainer
    {
        void Register<TService>(TService instance, string key = null) where TService : class;

        void Register<TService, TImplementation>(string key = null)
            where TService : class
            where TImplementation : class, TService, new();

        void RegisterFactory<TService>(Func<IInjectionContainer, TService> factory, string key = null)
            where TService : class;

        bool Has<TService>(string key = null) where TService : class;

        bool TryResolve<TService>(out TService service, string key = null) where TService : class;

        TService Resolve<TService>(string key = null) where TService : class;

        void Switch<TService>(string key) where TService : class;

        string GetActiveKey<TService>() where TService : class;
    }
}
