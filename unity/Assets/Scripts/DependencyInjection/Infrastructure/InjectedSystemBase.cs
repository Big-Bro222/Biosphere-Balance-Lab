using System;
using Unity.Entities;

namespace BioSphereLab.DependencyInjection.Infrastructure
{
    public abstract partial class InjectedSystemBase : SystemBase
    {
        private IInjectionContainer m_container;

        protected bool HasContainer
        {
            get { return m_container != null; }
        }

        protected bool TryResolve<TService>(out TService o_service, string p_key = null) where TService : class
        {
            if (m_container != null)
            {
                return m_container.TryResolve(out o_service, p_key);
            }

            o_service = null;
            return false;
        }

        protected TService Resolve<TService>(string p_key = null) where TService : class
        {
            if (TryResolve<TService>(out TService o_service, p_key))
            {
                return o_service;
            }

            throw new InvalidOperationException(
                $"No dependency container or service binding is available for {typeof(TService).Name}.");
        }

        internal void SetContainer(IInjectionContainer p_injectionContainer)
        {
            m_container = p_injectionContainer;
        }
    }
}
