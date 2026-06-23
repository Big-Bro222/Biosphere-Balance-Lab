using System;
using BioSphereLab.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace BioSphereLab.Systems.SampleStates
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class DeathSystem : SystemBase
    {
        private EntityQuery m_metaDataQuery;

        protected override void OnCreate()
        {
            base.OnCreate();
            m_metaDataQuery = EntityManager.CreateEntityQuery(
                ComponentType.ReadWrite<AliveCreatureEntityElement>());
        }

        protected override void OnUpdate()
        {
            long currentTimeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            if (m_metaDataQuery.IsEmptyIgnoreFilter)
            {
                return;
            }

            EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Allocator.TempJob);
            DestroyExpiredCreatureJob destroyExpiredCreatureJob = new DestroyExpiredCreatureJob
            {
                CurrentTimeStamp = currentTimeStamp,
                CreatureRefs = GetComponentLookup<MetaDataRef>(true),
                CommandBuffer = entityCommandBuffer.AsParallelWriter()
            };

            Dependency = destroyExpiredCreatureJob.ScheduleParallel(m_metaDataQuery, Dependency);
            Dependency.Complete();

            entityCommandBuffer.Playback(EntityManager);
            entityCommandBuffer.Dispose();
        }

        [BurstCompile]
        private partial struct DestroyExpiredCreatureJob : IJobEntity
        {
            public long CurrentTimeStamp;
            [ReadOnly] public ComponentLookup<MetaDataRef> CreatureRefs;
            public EntityCommandBuffer.ParallelWriter CommandBuffer;

            private void Execute(
                [EntityIndexInQuery] int p_sortKey,
                DynamicBuffer<AliveCreatureEntityElement> p_aliveCreatureEntities)
            {
                int expiredCount = 0;

                for (int i = 0; i < p_aliveCreatureEntities.Length; i++)
                {
                    AliveCreatureEntityElement creatureElement = p_aliveCreatureEntities[i];

                    if (CurrentTimeStamp <= creatureElement.ExpireTimeStamp)
                    {
                        break;
                    }

                    if (CreatureRefs.HasComponent(creatureElement.Value))
                    {
                        CommandBuffer.DestroyEntity(p_sortKey, creatureElement.Value);
                    }

                    expiredCount++;
                }

                if (expiredCount > 0)
                {
                    p_aliveCreatureEntities.RemoveRange(0, expiredCount);
                }
            }
        }
    }
}
