using System;
using BioSphereLab.Components;
using BioSphereLab.Components.Tag;
using BioSphereLab.DependencyInjection.Infrastructure;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace BioSphereLab.Systems
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [DisableAutoCreation]
    public partial class CreatureInitSystem : InjectedSystemBase, ICreatureInitSystem
    {
        private const double SpawnIntervalSeconds = 0.01d;
        private const float SpawnRadius = 10f;

        private EntityArchetype m_plantArchetype;
        private EntityQuery m_metaDataQuery;
        private double m_nextSpawnTime;
        private uint m_spawnSeed;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            m_spawnSeed = (uint)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            if (m_spawnSeed == 0)
            {
                m_spawnSeed = 1;
            }

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

            EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Unity.Collections.Allocator.TempJob);
            SpawnCreatureJob spawnCreatureJob = new SpawnCreatureJob
            {
                CommandBuffer = entityCommandBuffer.AsParallelWriter(),
                PlantArchetype = m_plantArchetype,
                CurrentTimeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                SpawnSeed = m_spawnSeed++,
                SpawnRadius = SpawnRadius
            };

            Dependency = spawnCreatureJob.ScheduleParallel(m_metaDataQuery, Dependency);
            Dependency.Complete();

            entityCommandBuffer.Playback(EntityManager);
            entityCommandBuffer.Dispose();
        }

        [BurstCompile]
        private partial struct SpawnCreatureJob : IJobEntity
        {
            public EntityCommandBuffer.ParallelWriter CommandBuffer;
            public EntityArchetype PlantArchetype;
            public long CurrentTimeStamp;
            public uint SpawnSeed;
            public float SpawnRadius;

            private void Execute(
                Entity p_metaDataEntity,
                [EntityIndexInQuery] int p_sortKey,
                in LifeSpan p_lifeSpan)
            {
                Entity creatureEntity = CommandBuffer.CreateEntity(p_sortKey, PlantArchetype);

                CommandBuffer.SetComponent(
                    p_sortKey,
                    creatureEntity,
                    new MetaDataRef { MetaDataEntity = p_metaDataEntity });

                CommandBuffer.SetComponent(
                    p_sortKey,
                    creatureEntity,
                    new TimeStamp { Value = CurrentTimeStamp });

                CommandBuffer.SetComponent(
                    p_sortKey,
                    creatureEntity,
                    new LocalToWorld
                    {
                        Value = float4x4.TRS(
                            GetRandomSpawnPosition(p_metaDataEntity, p_sortKey),
                            quaternion.identity,
                            new float3(10f, 10f, 10f))
                    });

                CommandBuffer.AppendToBuffer(
                    p_sortKey,
                    p_metaDataEntity,
                    new AliveCreatureEntityElement
                    {
                        Value = creatureEntity,
                        ExpireTimeStamp = CurrentTimeStamp + (long)p_lifeSpan.Value
                    });
            }

            private float3 GetRandomSpawnPosition(Entity p_metaDataEntity, int p_sortKey)
            {
                uint hash = math.hash(new uint4(
                    (uint)p_metaDataEntity.Index,
                    (uint)p_metaDataEntity.Version,
                    (uint)p_sortKey,
                    SpawnSeed));

                Unity.Mathematics.Random random = Unity.Mathematics.Random.CreateFromIndex(hash);
                float angle = random.NextFloat(0f, math.PI * 2f);
                float radius = math.sqrt(random.NextFloat()) * SpawnRadius;

                return new float3(
                    math.cos(angle) * radius,
                    0f,
                    math.sin(angle) * radius);
            }
        }
    }
    
    public interface ICreatureInitSystem
    {
    }
}
