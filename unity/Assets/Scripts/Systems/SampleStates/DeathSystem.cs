using System;
using BioSphereLab.Components;
using Unity.Collections;
using Unity.Entities;

namespace BioSphereLab.Systems.SampleStates
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class DeathSystem : SystemBase
    {
        private EntityQuery m_creatureQuery;

        protected override void OnCreate()
        {
            base.OnCreate();
            m_creatureQuery = EntityManager.CreateEntityQuery(
                ComponentType.ReadOnly<TimeStamp>(),
                ComponentType.ReadOnly<MetaDataRef>());
        }

        protected override void OnUpdate()
        {
            long currentTimeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            if (m_creatureQuery.IsEmptyIgnoreFilter)
            {
                return;
            }

            using NativeArray<Entity> creatureEntities =
                m_creatureQuery.ToEntityArray(Allocator.Temp);

            for (int i = 0; i < creatureEntities.Length; i++)
            {
                Entity creatureEntity = creatureEntities[i];
                TimeStamp timeStamp = EntityManager.GetComponentData<TimeStamp>(creatureEntity);
                MetaDataRef metaDataRef = EntityManager.GetComponentData<MetaDataRef>(creatureEntity);

                if (!EntityManager.Exists(metaDataRef.MetaDataEntity) ||
                    !EntityManager.HasComponent<LifeSpan>(metaDataRef.MetaDataEntity))
                {
                    continue;
                }

                LifeSpan lifeSpan = EntityManager.GetComponentData<LifeSpan>(metaDataRef.MetaDataEntity);

                long expireTimeStamp = timeStamp.Value + (long)lifeSpan.Value;
                if (currentTimeStamp <= expireTimeStamp)
                {
                    continue;
                }

                RemoveCreatureEntityFromMetaDataBuffer(
                    metaDataRef.MetaDataEntity,
                    creatureEntity);

                EntityManager.DestroyEntity(creatureEntity);
            }
        }

        private void RemoveCreatureEntityFromMetaDataBuffer(Entity p_metaDataEntity, Entity p_creatureEntity)
        {
            if (!EntityManager.Exists(p_metaDataEntity) ||
                !EntityManager.HasBuffer<AliveCreatureEntityElement>(p_metaDataEntity))
            {
                return;
            }

            DynamicBuffer<AliveCreatureEntityElement> aliveCreatureEntities =
                EntityManager.GetBuffer<AliveCreatureEntityElement>(p_metaDataEntity);

            for (int i = aliveCreatureEntities.Length - 1; i >= 0; i--)
            {
                if (aliveCreatureEntities[i].Value == p_creatureEntity)
                {
                    aliveCreatureEntities.RemoveAt(i);
                    return;
                }
            }
        }
    }
}
