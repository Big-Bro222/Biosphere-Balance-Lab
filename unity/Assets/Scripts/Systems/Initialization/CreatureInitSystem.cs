using BioSphereLab.Components;
using BioSphereLab.Components.Tag;
using BioSphereLab.DependencyInjection.Infrastructure;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine.UIElements;

namespace BioSphereLab.Systems
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [DisableAutoCreation]
    public partial class CreatureInitSystem : InjectedSystemBase, ICreatureInitSystem
    {
        private EntityArchetype m_plantArchetype;
        private EntityArchetype m_herbivoreArchetype;
        private EntityArchetype m_predatorArchetype;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            m_plantArchetype = EntityManager.CreateArchetype(
                ComponentType.ReadWrite<PlantTag>(),
                ComponentType.ReadWrite<LocalToWorld>(),
                ComponentType.ReadWrite<Health>(),
                ComponentType.ReadWrite<BirthTime>(),
                ComponentType.ReadWrite<LifeSpan>());

            m_herbivoreArchetype = EntityManager.CreateArchetype(
                ComponentType.ReadWrite<HerbivoreTag>(),
                ComponentType.ReadWrite<LocalToWorld>(),
                ComponentType.ReadWrite<Health>(),
                ComponentType.ReadWrite<BirthTime>(),
                ComponentType.ReadWrite<LifeSpan>());

            m_predatorArchetype = EntityManager.CreateArchetype(
                ComponentType.ReadWrite<PredatorTag>(),
                ComponentType.ReadWrite<LocalToWorld>(),
                ComponentType.ReadWrite<Health>(),
                ComponentType.ReadWrite<BirthTime>(),
                ComponentType.ReadWrite<LifeSpan>());
        }

        protected override void OnUpdate()
        {
            if (!TryResolve<ICreatureSpawnConfig>(out ICreatureSpawnConfig o_spawnConfig))
            {
                return;
            }



            CreateEntities(m_plantArchetype, o_spawnConfig.PlantCount);
            CreateEntities(m_herbivoreArchetype, o_spawnConfig.HerbivoreCount);
            CreateEntities(m_predatorArchetype, o_spawnConfig.PredatorCount);

            Enabled = false;
        }

        private void CreateEntities(EntityArchetype p_archetype, int p_count)
        {
            for (int i = 0; i < p_count; i++)
            {
                EntityManager.CreateEntity(p_archetype);
            }
        }
    }
    
    public interface ICreatureInitSystem{}
}
