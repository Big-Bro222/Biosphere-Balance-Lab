using System;
using System.Collections.Generic;
using Unity.Entities;

namespace BioSphereLab.DependencyInjection.Infrastructure
{
    public sealed class InjectionContainer : IInjectionContainer
    {
        public const string DefaultKey = "";

        private readonly Dictionary<BindingId, Func<IInjectionContainer, object>> m_bindings = new();
        private readonly Dictionary<Type, string> m_activeKeys = new();

        public void Register<TService>(TService p_instance, string p_key = null) where TService : class
        {
            if (p_instance is null)
            {
                throw new ArgumentNullException(nameof(p_instance));
            }

            BindingId bindingId = BindingId.Create<TService>(p_key);
            m_bindings[bindingId] = _ => p_instance;

            EnsureActiveKey<TService>(bindingId.Key);
        }

        public void Register<TService, TImplementation>(string p_key = null)
            where TService : class
            where TImplementation : class, TService, new()
        {
            RegisterFactory<TService>(_ => new TImplementation(), p_key);
        }

        public void RegisterFactory<TService>(Func<IInjectionContainer, TService> p_factory, string p_key = null)
            where TService : class
        {
            if (p_factory is null)
            {
                throw new ArgumentNullException(nameof(p_factory));
            }

            BindingId bindingId = BindingId.Create<TService>(p_key);
            m_bindings[bindingId] = p_container => p_factory(p_container);

            EnsureActiveKey<TService>(bindingId.Key);
        }

        public TSystem RegisterSystem<TService, TSystem, TSystemGroup>(World p_world, string p_key = null)
            where TService : class
            where TSystem : SystemBase, TService
            where TSystemGroup : ComponentSystemGroup
        {
            if (p_world == null)
            {
                throw new ArgumentNullException(nameof(p_world));
            }

            TSystem system = p_world.GetOrCreateSystemManaged<TSystem>();
            Register<TService>(system, p_key);

            if (system is InjectedSystemBase injectedSystem)
            {
                injectedSystem.SetContainer(this);
            }

            TSystemGroup systemGroup = p_world.GetOrCreateSystemManaged<TSystemGroup>();
            systemGroup.AddSystemToUpdateList(system);
            systemGroup.SortSystems();

            return system;
        }

        public bool Has<TService>(string p_key = null) where TService : class
        {
            BindingId bindingId = BindingId.Create<TService>(ResolveLookupKey<TService>(p_key));
            return m_bindings.ContainsKey(bindingId);
        }

        public bool TryResolve<TService>(out TService o_service, string p_key = null) where TService : class
        {
            BindingId bindingId = BindingId.Create<TService>(ResolveLookupKey<TService>(p_key));

            if (m_bindings.TryGetValue(bindingId, out Func<IInjectionContainer, object> o_factory))
            {
                o_service = (TService)o_factory(this);
                return true;
            }

            o_service = null;
            return false;
        }

        public TService Resolve<TService>(string p_key = null) where TService : class
        {
            if (TryResolve<TService>(out TService o_service, p_key))
            {
                return o_service;
            }

            string lookupKey = ResolveLookupKey<TService>(p_key);
            string label = string.IsNullOrWhiteSpace(lookupKey) ? "default" : lookupKey;
            throw new InvalidOperationException($"No dependency registered for {typeof(TService).Name} with key '{label}'.");
        }

        public void Switch<TService>(string p_key) where TService : class
        {
            string normalizedKey = NormalizeKey(p_key);
            BindingId bindingId = BindingId.Create<TService>(normalizedKey);

            if (!m_bindings.ContainsKey(bindingId))
            {
                string label = string.IsNullOrWhiteSpace(normalizedKey) ? "default" : normalizedKey;
                throw new InvalidOperationException($"Cannot switch {typeof(TService).Name} to missing key '{label}'.");
            }

            m_activeKeys[typeof(TService)] = normalizedKey;
        }

        public string GetActiveKey<TService>() where TService : class
        {
            return m_activeKeys.TryGetValue(typeof(TService), out string o_key) ? o_key : DefaultKey;
        }

        private void EnsureActiveKey<TService>(string p_key) where TService : class
        {
            Type serviceType = typeof(TService);

            if (!m_activeKeys.ContainsKey(serviceType))
            {
                m_activeKeys[serviceType] = p_key;
            }
        }

        private string ResolveLookupKey<TService>(string p_key) where TService : class
        {
            if (!string.IsNullOrWhiteSpace(p_key))
            {
                return NormalizeKey(p_key);
            }

            return m_activeKeys.TryGetValue(typeof(TService), out string o_activeKey) ? o_activeKey : DefaultKey;
        }

        private static string NormalizeKey(string p_key)
        {
            return string.IsNullOrWhiteSpace(p_key) ? DefaultKey : p_key.Trim();
        }

        private readonly struct BindingId : IEquatable<BindingId>
        {
            public BindingId(Type p_serviceType, string p_key)
            {
                ServiceType = p_serviceType;
                Key = NormalizeKey(p_key);
            }

            public Type ServiceType { get; }

            public string Key { get; }

            public static BindingId Create<TService>(string p_key)
            {
                return new BindingId(typeof(TService), p_key);
            }

            public bool Equals(BindingId p_other)
            {
                return ServiceType == p_other.ServiceType && Key == p_other.Key;
            }

            public override bool Equals(object p_obj)
            {
                return p_obj is BindingId other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((ServiceType != null ? ServiceType.GetHashCode() : 0) * 397) ^ Key.GetHashCode();
                }
            }
        }
    }
}
