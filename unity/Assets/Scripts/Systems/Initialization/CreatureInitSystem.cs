using BioSphereLab.Components.Tag;
using BioSphereLab.DependencyInjection.Infrastructure;
using Unity.Entities;

namespace BioSphereLab.Systems
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [DisableAutoCreation]
    public partial class CreatureInitSystem : InjectedSystemBase, ICreatureInitSystem
    {
        protected override void OnUpdate()
        {
            if (!TryResolve<ICreatureSpawnConfig>(out ICreatureSpawnConfig spawnConfig))
            {
                return;
            }
            CreateEntitiesWithTag<PlantTag>(spawnConfig.PlantCount);
            CreateEntitiesWithTag<HerbivoreTag>(spawnConfig.HerbivoreCount);
            CreateEntitiesWithTag<PredatorTag>(spawnConfig.PredatorCount);
        }

        private void CreateEntitiesWithTag<TTag>(int count) where TTag : unmanaged, IComponentData
        {
            for (int i = 0; i < count; i++)
            {
                EntityManager.CreateEntity(ComponentType.ReadWrite<TTag>());
            }
        }
    }
    
    public interface ICreatureInitSystem{}
}
