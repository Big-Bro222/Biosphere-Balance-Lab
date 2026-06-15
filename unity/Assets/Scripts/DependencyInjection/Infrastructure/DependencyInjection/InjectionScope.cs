using Unity.Entities;
using UnityEngine;

namespace BiosphereBalanceLab.Infrastructure.DependencyInjection
{
    public sealed class InjectionScope : MonoBehaviour
    {
        [SerializeField] private StorageInjection storageInjection;
        [SerializeField] private bool includeChildInstallers = true;
        [SerializeField] private bool registerWithDefaultEcsWorld = true;
        [SerializeField] private bool dontDestroyOnLoad;

        private InjectionContainer container;

        public IInjectionContainer Container
        {
            get
            {
                EnsureBuilt();
                return container;
            }
        }

        private void Awake()
        {
            EnsureBuilt();

            if (dontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (registerWithDefaultEcsWorld && World.DefaultGameObjectInjectionWorld != null)
            {
                EcsInjectionRegistry.Unregister(World.DefaultGameObjectInjectionWorld, container);
            }
        }

        private void EnsureBuilt()
        {
            if (container != null)
            {
                return;
            }

            container = new InjectionContainer();
            RegisterBuiltInServices();
            RunInstallers();
            RegisterEcsWorld();
        }

        private void RegisterBuiltInServices()
        {
            container.Register<IInjectionContainer>(container);

            if (storageInjection != null)
            {
                container.Register(storageInjection);
                container.Register<IPrefabStorage>(storageInjection);
            }
        }

        private void RunInstallers()
        {
            MonoBehaviour[] behaviours = includeChildInstallers
                ? GetComponentsInChildren<MonoBehaviour>(true)
                : GetComponents<MonoBehaviour>();

            foreach (MonoBehaviour behaviour in behaviours)
            {
                if (behaviour is IInjectionInstaller installer)
                {
                    installer.Install(container);
                }
            }
        }

        private void RegisterEcsWorld()
        {
            if (!registerWithDefaultEcsWorld || World.DefaultGameObjectInjectionWorld == null)
            {
                return;
            }

            EcsInjectionRegistry.Register(World.DefaultGameObjectInjectionWorld, container);
        }
    }
}
