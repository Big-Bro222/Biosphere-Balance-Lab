namespace BioSphereLab.DependencyInjection.Examples
{
    public sealed class StressTestBiosphereSpawnPolicy : IBiosphereSpawnPolicy
    {
        public string Name => "Stress Test";

        public int InitialPlants => 24;

        public int InitialAnimals => 60;
    }
}
