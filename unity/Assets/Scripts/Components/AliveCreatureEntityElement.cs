using Unity.Entities;

namespace BioSphereLab.Components
{
    public struct AliveCreatureEntityElement : IBufferElementData
    {
        public Entity Value;
        public long ExpireTimeStamp;
    }
}
