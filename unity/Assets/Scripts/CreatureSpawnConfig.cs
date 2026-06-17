namespace BioSphereLab
{
    public interface ICreatureSpawnConfig
    {
        int PlantCount { get; }

        int HerbivoreCount { get; }

        int PredatorCount { get; }
    }

    public sealed class ConstCreatureSpawnConfig : ICreatureSpawnConfig
    {
        public int PlantCount => Const.PlantCount;

        public int HerbivoreCount => Const.HerbivoreCount;

        public int PredatorCount => Const.PredatorCount;
    }
}
