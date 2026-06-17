using System;
using BioSphereLab.Components;
using BioSphereLab.Components.Tag;
using BioSphereLab.DependencyInjection.Infrastructure;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace BioSphereLab.Systems
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [DisableAutoCreation]
    public partial class CreatureInitSystem : InjectedSystemBase, ICreatureInitSystem
    {
        private const double SpawnIntervalSeconds = 1.0d;

        private EntityArchetype m_plantArchetype;
        private EntityQuery m_metaDataQuery;
        private double m_nextSpawnTime;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            m_plantArchetype = EntityManager.CreateArchetype(
                ComponentType.ReadWrite<MetaDataRef>(),
                ComponentType.ReadWrite<PlantTag>(),
                ComponentType.ReadWrite<LocalToWorld>(),
                ComponentType.ReadWrite<TimeStamp>(),
                ComponentType.ReadWrite<Health>());

            m_metaDataQuery = EntityManager.CreateEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    ComponentType.ReadOnly<TimeStamp>(),
                    ComponentType.ReadOnly<CreatureId>(),
                    ComponentType.ReadOnly<LifeSpan>()
                },
                None = new ComponentType[]
                {
                    ComponentType.ReadOnly<MetaDataRef>()
                }
            });
        }

        protected override void OnUpdate()
        {
            double elapsedTime = SystemAPI.Time.ElapsedTime;
            if (elapsedTime < m_nextSpawnTime)
            {
                return;
            }


            m_nextSpawnTime = elapsedTime + SpawnIntervalSeconds;

            if (m_metaDataQuery.IsEmptyIgnoreFilter)
            {
                return;
            }

            using (NativeArray<Entity> metaDataEntities = m_metaDataQuery.ToEntityArray(Allocator.Temp))
            {
                for (int i = 0; i < metaDataEntities.Length; i++)
                {
                    Entity metaDataEntity = metaDataEntities[i];
                    Entity creatureEntity = EntityManager.CreateEntity(m_plantArchetype);

                    EntityManager.SetComponentData(creatureEntity, new MetaDataRef { MetaDataEntity = metaDataEntity });
                    EntityManager.SetComponentData(creatureEntity, new TimeStamp { Value = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), });
                }
            };
        }
    }
    
    public interface ICreatureInitSystem
    {
    }
}
