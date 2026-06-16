namespace BioSphereLab.DependencyInjection.Examples
{
    public sealed class BalancedBiosphereSpawnPolicy : IBiosphereSpawnPolicy
    {
        public string Name => "Balanced";

        public int InitialPlants => 80;

        public int InitialAnimals => 18;
    }
}
