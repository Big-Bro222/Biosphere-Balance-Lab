using System.Collections.Generic;
using BioSphereLab.Components;
using BioSphereLab.DependencyInjection.Infrastructure;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace BioSphereLab.Systems.Presentation
{
    public interface ICreaturePresentationSystem
    {
    }

    [UpdateInGroup(typeof(PresentationSystemGroup))]
    [DisableAutoCreation]
    public partial class CreaturePresentationSystem : InjectedSystemBase, ICreaturePresentationSystem
    {
        private const int MaxInstancesPerBatch = 1023;
        private const string CreatureMeshKey = "Sphere";
        private const string CreatureMaterialKey = "Default";

        private readonly List<Matrix4x4> m_latestCreatureMatrices = new();
        private readonly InstanceData[] m_instanceBatch = new InstanceData[MaxInstancesPerBatch];

        private EntityQuery m_metaDataQuery;
        private Mesh m_creatureMesh;
        private Material m_creatureMaterial;
        private bool m_triedResolveRenderAssets;
        private bool m_loggedMissingRenderAssets;

        protected override void OnCreate()
        {
            base.OnCreate();
            m_metaDataQuery = EntityManager.CreateEntityQuery(
                ComponentType.ReadOnly<AliveCreatureEntityElement>());
        }

        protected override void OnUpdate()
        {
            EnsureRenderAssets();

            m_latestCreatureMatrices.Clear();

            if (!m_metaDataQuery.IsEmptyIgnoreFilter)
            {
                CollectLatestCreatureMatrices();
            }

            DrawInstances();
        }

        protected override void OnDestroy()
        {
            if (m_creatureMaterial != null)
            {
                Object.Destroy(m_creatureMaterial);
            }

            m_latestCreatureMatrices.Clear();
        }

        private void EnsureRenderAssets()
        {
            if (m_triedResolveRenderAssets)
            {
                return;
            }

            m_triedResolveRenderAssets = true;

            if (!TryResolve(out IAssetStorage assetStorage))
            {
                LogMissingRenderAssets("IAssetStorage is not registered in the injection container.");
                return;
            }

            if (!assetStorage.TryGetMesh(CreatureMeshKey, out m_creatureMesh))
            {
                LogMissingRenderAssets(
                    $"StorageInjection does not contain a mesh with key '{CreatureMeshKey}'.");
                return;
            }

            if (!assetStorage.TryGetMaterial(CreatureMaterialKey, out Material creatureMaterial))
            {
                LogMissingRenderAssets(
                    $"StorageInjection does not contain a material with key '{CreatureMaterialKey}'.");
                return;
            }

            m_creatureMaterial = new Material(creatureMaterial)
            {
                enableInstancing = true
            };
        }

        private void LogMissingRenderAssets(string p_message)
        {
            if (m_loggedMissingRenderAssets)
            {
                return;
            }

            m_loggedMissingRenderAssets = true;
            Debug.LogWarning($"CreaturePresentationSystem skipped rendering: {p_message}");
        }

        private void CollectLatestCreatureMatrices()
        {
            int metaDataCount = m_metaDataQuery.CalculateEntityCount();
            if (metaDataCount == 0)
            {
                return;
            }

            NativeArray<float4x4> latestCreatureMatrices =
                new NativeArray<float4x4>(metaDataCount, Allocator.TempJob);
            NativeArray<byte> hasLatestCreatureMatrix =
                new NativeArray<byte>(metaDataCount, Allocator.TempJob);

            CollectLatestCreatureMatricesJob collectLatestCreatureMatricesJob =
                new CollectLatestCreatureMatricesJob
                {
                    LocalToWorlds = GetComponentLookup<LocalToWorld>(true),
                    LatestCreatureMatrices = latestCreatureMatrices,
                    HasLatestCreatureMatrix = hasLatestCreatureMatrix
                };

            Dependency = collectLatestCreatureMatricesJob.ScheduleParallel(m_metaDataQuery, Dependency);
            Dependency.Complete();

            for (int i = 0; i < latestCreatureMatrices.Length; i++)
            {
                if (hasLatestCreatureMatrix[i] == 0)
                {
                    continue;
                }

                m_latestCreatureMatrices.Add(ToMatrix4x4(latestCreatureMatrices[i]));
            }

            latestCreatureMatrices.Dispose();
            hasLatestCreatureMatrix.Dispose();
        }

        private void DrawInstances()
        {
            if (m_creatureMesh == null ||
                m_creatureMaterial == null ||
                m_latestCreatureMatrices.Count == 0)
            {
                return;
            }

            RenderParams renderParams = new RenderParams(m_creatureMaterial)
            {
                worldBounds = new Bounds(Vector3.zero, Vector3.one * 10000f)
            };

            for (int startIndex = 0;
                 startIndex < m_latestCreatureMatrices.Count;
                 startIndex += MaxInstancesPerBatch)
            {
                int instanceCount = Mathf.Min(
                    MaxInstancesPerBatch,
                    m_latestCreatureMatrices.Count - startIndex);

                for (int i = 0; i < instanceCount; i++)
                {
                    m_instanceBatch[i] = new InstanceData
                    {
                        objectToWorld = m_latestCreatureMatrices[startIndex + i]
                    };
                }

                Graphics.RenderMeshInstanced(
                    renderParams,
                    m_creatureMesh,
                    0,
                    m_instanceBatch,
                    instanceCount);
            }
        }

        private static Matrix4x4 ToMatrix4x4(float4x4 p_value)
        {
            return new Matrix4x4(
                new Vector4(p_value.c0.x, p_value.c0.y, p_value.c0.z, p_value.c0.w),
                new Vector4(p_value.c1.x, p_value.c1.y, p_value.c1.z, p_value.c1.w),
                new Vector4(p_value.c2.x, p_value.c2.y, p_value.c2.z, p_value.c2.w),
                new Vector4(p_value.c3.x, p_value.c3.y, p_value.c3.z, p_value.c3.w));
        }

        private struct InstanceData
        {
            public Matrix4x4 objectToWorld;
        }

        [BurstCompile]
        private partial struct CollectLatestCreatureMatricesJob : IJobEntity
        {
            [ReadOnly] public ComponentLookup<LocalToWorld> LocalToWorlds;
            public NativeArray<float4x4> LatestCreatureMatrices;
            public NativeArray<byte> HasLatestCreatureMatrix;

            private void Execute(
                [EntityIndexInQuery] int p_entityIndexInQuery,
                DynamicBuffer<AliveCreatureEntityElement> p_aliveCreatureEntities)
            {
                RemoveMissingCreatureEntities(p_aliveCreatureEntities);

                if (p_aliveCreatureEntities.Length == 0)
                {
                    return;
                }

                Entity latestCreatureEntity =
                    p_aliveCreatureEntities[p_aliveCreatureEntities.Length - 1].Value;

                if (!LocalToWorlds.HasComponent(latestCreatureEntity))
                {
                    return;
                }

                LatestCreatureMatrices[p_entityIndexInQuery] =
                    LocalToWorlds[latestCreatureEntity].Value;
                HasLatestCreatureMatrix[p_entityIndexInQuery] = 1;
            }

            private void RemoveMissingCreatureEntities(
                DynamicBuffer<AliveCreatureEntityElement> p_aliveCreatureEntities)
            {
                for (int i = p_aliveCreatureEntities.Length - 1; i >= 0; i--)
                {
                    if (!LocalToWorlds.HasComponent(p_aliveCreatureEntities[i].Value))
                    {
                        p_aliveCreatureEntities.RemoveAt(i);
                    }
                }
            }
        }
    }
}
