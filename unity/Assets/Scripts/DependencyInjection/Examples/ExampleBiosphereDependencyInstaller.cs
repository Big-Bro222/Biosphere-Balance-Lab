using BioSphereLab.DependencyInjection.Infrastructure;
using UnityEngine;

namespace BioSphereLab.DependencyInjection.Examples
{
    public sealed class ExampleBiosphereDependencyInstaller : MonoBehaviour, IInjectionInstaller
    {
        [SerializeField] private string startupPolicyKey = "balanced";

        public void Install(IInjectionContainer container)
        {
            container.Register<IBiosphereSpawnPolicy>(new BalancedBiosphereSpawnPolicy(), "balanced");
            container.Register<IBiosphereSpawnPolicy>(new StressTestBiosphereSpawnPolicy(), "stress");
            container.Switch<IBiosphereSpawnPolicy>(startupPolicyKey);
        }
    }
}
