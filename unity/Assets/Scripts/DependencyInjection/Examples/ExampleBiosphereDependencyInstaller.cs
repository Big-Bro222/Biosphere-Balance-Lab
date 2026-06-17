using BioSphereLab.DependencyInjection.Infrastructure;
using UnityEngine;
using UnityEngine.Serialization;

namespace BioSphereLab.DependencyInjection.Examples
{
    public sealed class ExampleBiosphereDependencyInstaller : MonoBehaviour, IInjectionInstaller
    {
        [FormerlySerializedAs("startupPolicyKey")]
        [SerializeField] private string m_startupPolicyKey = "balanced";

        public void Install(IInjectionContainer p_container)
        {
            p_container.Register<IBiosphereSpawnPolicy>(new BalancedBiosphereSpawnPolicy(), "balanced");
            p_container.Register<IBiosphereSpawnPolicy>(new StressTestBiosphereSpawnPolicy(), "stress");
            p_container.Switch<IBiosphereSpawnPolicy>(m_startupPolicyKey);
        }
    }
}
