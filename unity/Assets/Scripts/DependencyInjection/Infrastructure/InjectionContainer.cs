using System;
using System.Collections.Generic;

namespace BioSphereLab.DependencyInjection.Infrastructure
{
    public sealed class InjectionContainer : IInjectionContainer
    {
        public const string DefaultKey = "";

        private readonly Dictionary<BindingId, Func<IInjectionContainer, object>> bindings = new();
        private readonly Dictionary<Type, string> activeKeys = new();

        public void Register<TService>(TService instance, string key = null) where TService : class
        {
            if (instance is null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            BindingId bindingId = BindingId.Create<TService>(key);
            bindings[bindingId] = _ => instance;

            EnsureActiveKey<TService>(bindingId.Key);
        }

        public void Register<TService, TImplementation>(string key = null)
            where TService : class
            where TImplementation : class, TService, new()
        {
            RegisterFactory<TService>(_ => new TImplementation(), key);
        }

        public void RegisterFactory<TService>(Func<IInjectionContainer, TService> factory, string key = null)
            where TService : class
        {
            if (factory is null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            BindingId bindingId = BindingId.Create<TService>(key);
            bindings[bindingId] = container => factory(container);

            EnsureActiveKey<TService>(bindingId.Key);
        }

        public bool Has<TService>(string key = null) where TService : class
        {
            BindingId bindingId = BindingId.Create<TService>(ResolveLookupKey<TService>(key));
            return bindings.ContainsKey(bindingId);
        }

        public bool TryResolve<TService>(out TService service, string key = null) where TService : class
        {
            BindingId bindingId = BindingId.Create<TService>(ResolveLookupKey<TService>(key));

            if (bindings.TryGetValue(bindingId, out Func<IInjectionContainer, object> factory))
            {
                service = (TService)factory(this);
                return true;
            }

            service = null;
            return false;
        }

        public TService Resolve<TService>(string key = null) where TService : class
        {
            if (TryResolve<TService>(out TService service, key))
            {
                return service;
            }

            string lookupKey = ResolveLookupKey<TService>(key);
            string label = string.IsNullOrWhiteSpace(lookupKey) ? "default" : lookupKey;
            throw new InvalidOperationException($"No dependency registered for {typeof(TService).Name} with key '{label}'.");
        }

        public void Switch<TService>(string key) where TService : class
        {
            string normalizedKey = NormalizeKey(key);
            BindingId bindingId = BindingId.Create<TService>(normalizedKey);

            if (!bindings.ContainsKey(bindingId))
            {
                string label = string.IsNullOrWhiteSpace(normalizedKey) ? "default" : normalizedKey;
                throw new InvalidOperationException($"Cannot switch {typeof(TService).Name} to missing key '{label}'.");
            }

            activeKeys[typeof(TService)] = normalizedKey;
        }

        public string GetActiveKey<TService>() where TService : class
        {
            return activeKeys.TryGetValue(typeof(TService), out string key) ? key : DefaultKey;
        }

        private void EnsureActiveKey<TService>(string key) where TService : class
        {
            Type serviceType = typeof(TService);

            if (!activeKeys.ContainsKey(serviceType))
            {
                activeKeys[serviceType] = key;
            }
        }

        private string ResolveLookupKey<TService>(string key) where TService : class
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                return NormalizeKey(key);
            }

            return activeKeys.TryGetValue(typeof(TService), out string activeKey) ? activeKey : DefaultKey;
        }

        private static string NormalizeKey(string key)
        {
            return string.IsNullOrWhiteSpace(key) ? DefaultKey : key.Trim();
        }

        private readonly struct BindingId : IEquatable<BindingId>
        {
            public BindingId(Type serviceType, string key)
            {
                ServiceType = serviceType;
                Key = NormalizeKey(key);
            }

            public Type ServiceType { get; }

            public string Key { get; }

            public static BindingId Create<TService>(string key)
            {
                return new BindingId(typeof(TService), key);
            }

            public bool Equals(BindingId other)
            {
                return ServiceType == other.ServiceType && Key == other.Key;
            }

            public override bool Equals(object obj)
            {
                return obj is BindingId other && Equals(other);
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
