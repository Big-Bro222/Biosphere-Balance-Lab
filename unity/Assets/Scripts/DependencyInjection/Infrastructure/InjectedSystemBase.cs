using System;
using Unity.Entities;

namespace BioSphereLab.DependencyInjection.Infrastructure
{
    public abstract partial class InjectedSystemBase : SystemBase
    {
        private IInjectionContainer container;

        protected bool HasContainer
        {
            get { return container != null; }
        }

        protected bool TryResolve<TService>(out TService service, string key = null) where TService : class
        {
            if (container != null)
            {
                return container.TryResolve(out service, key);
            }

            service = null;
            return false;
        }

        protected TService Resolve<TService>(string key = null) where TService : class
        {
            if (TryResolve<TService>(out TService service, key))
            {
                return service;
            }

            throw new InvalidOperationException(
                $"No dependency container or service binding is available for {typeof(TService).Name}.");
        }

        internal void SetContainer(IInjectionContainer injectionContainer)
        {
            container = injectionContainer;
        }
    }
}
