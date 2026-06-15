namespace BiosphereBalanceLab.Examples.DependencyInjection
{
    public interface IBiosphereSpawnPolicy
    {
        string Name { get; }

        int InitialPlants { get; }

        int InitialAnimals { get; }
    }
}
