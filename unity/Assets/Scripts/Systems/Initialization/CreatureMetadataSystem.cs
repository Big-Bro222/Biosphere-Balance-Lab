using System;
using BioSphereLab.Components;
using Unity.Entities;

namespace BioSphereLab.Systems
{
    public interface ICreatureMetadataSystem
    {
    }

    /// <summary>
    /// This system is responsible for creating MetaData for each individual creatures based on the number
    /// </summary>
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateBefore(typeof(CreatureInitSystem))]
    [DisableAutoCreation]
    public partial class CreatureMetadataSystem : SystemBase, ICreatureMetadataSystem
    {
        private int m_plantToCreatCount;
        private int m_herbivoreToCreatCount;
        private int m_predatorToCreatCount;
        //Might want to change to a universal size id later on
        private int m_nextCreatureId = 0;
        
        private EntityArchetype m_metadataArchetype;
        

        protected override void OnCreate()
        {
            base.OnCreate();
            
            m_plantToCreatCount = Const.PlantCount;
            m_nextCreatureId = 1;
            m_metadataArchetype = EntityManager.CreateArchetype(
                ComponentType.ReadWrite<TimeStamp>(),
                ComponentType.ReadWrite<CreatureId>(),
                ComponentType.ReadWrite<LifeSpan>());
        }
        
        protected override void OnUpdate()
        {
            if (m_plantToCreatCount == 0)
            {
                return;
            }
            
            //based on the Pending Count, generate a entity with a new PlantId
            for (int i = 0; i < m_plantToCreatCount; i++)
            {
                Entity metaDataEntity = EntityManager.CreateEntity(m_metadataArchetype);
                EntityManager.SetComponentData(metaDataEntity, new TimeStamp
                {
                    Value = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                });
                EntityManager.SetComponentData(metaDataEntity, new LifeSpan(){Value = 100});
                EntityManager.SetComponentData(metaDataEntity, new CreatureId
                {
                    Value = m_nextCreatureId++
                });
            }

            m_plantToCreatCount = 0;
        }
    }
}
