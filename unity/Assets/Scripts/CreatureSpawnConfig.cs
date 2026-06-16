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
        public int PlantCount => Const.PLANT_COUNT;

        public int HerbivoreCount => Const.HERBIVORE_COUNT;

        public int PredatorCount => Const.PREDATOR_COUNT;
    }
}
