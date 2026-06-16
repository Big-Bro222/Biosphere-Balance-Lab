using Unity.Entities;

namespace BioSphereLab.Components
{
    public struct BirthTime : IComponentData
    {
        public int Year;
        public int Month;
        public int Day;
    }
}
