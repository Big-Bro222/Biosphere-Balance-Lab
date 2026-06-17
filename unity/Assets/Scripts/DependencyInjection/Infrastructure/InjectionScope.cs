using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

namespace BioSphereLab.DependencyInjection.Infrastructure
{
    public sealed class InjectionScope : MonoBehaviour
    {
        [FormerlySerializedAs("storageInjection")]
        [SerializeField] private StorageInjection m_storageInjection;
        [FormerlySerializedAs("includeChildInstallers")]
        [SerializeField] private bool m_includeChildInstallers = true;
        [FormerlySerializedAs("registerWithDefaultEcsWorld")]
        [SerializeField] private bool m_registerWithDefaultEcsWorld = true;
        [FormerlySerializedAs("dontDestroyOnLoad")]
        [SerializeField] private bool m_dontDestroyOnLoad;

        private InjectionContainer m_container;

        public IInjectionContainer Container
        {
            get
            {
                EnsureBuilt();
                return m_container;
            }
        }

        private void Awake()
        {
            EnsureBuilt();

            if (m_dontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (m_registerWithDefaultEcsWorld && World.DefaultGameObjectInjectionWorld != null)
            {
                EcsInjectionRegistry.Unregister(World.DefaultGameObjectInjectionWorld, m_container);
            }
        }

        private void EnsureBuilt()
        {
            if (m_container != null)
            {
                return;
            }

            m_container = new InjectionContainer();
            RegisterBuiltInServices();
            RunInstallers();
            RegisterEcsWorld();
        }

        private void RegisterBuiltInServices()
        {
            m_container.Register<IInjectionContainer>(m_container);

            if (m_storageInjection != null)
            {
                m_container.Register(m_storageInjection);
                m_container.Register<IAssetStorage>(m_storageInjection);
            }
        }

        private void RunInstallers()
        {
            MonoBehaviour[] behaviours = m_includeChildInstallers
                ? GetComponentsInChildren<MonoBehaviour>(true)
                : GetComponents<MonoBehaviour>();

            foreach (MonoBehaviour behaviour in behaviours)
            {
                if (behaviour is IInjectionInstaller installer)
                {
                    installer.Install(m_container);
                }
            }
        }

        private void RegisterEcsWorld()
        {
            if (!m_registerWithDefaultEcsWorld || World.DefaultGameObjectInjectionWorld == null)
            {
                return;
            }

            EcsInjectionRegistry.Register(World.DefaultGameObjectInjectionWorld, m_container);
        }
    }
}
