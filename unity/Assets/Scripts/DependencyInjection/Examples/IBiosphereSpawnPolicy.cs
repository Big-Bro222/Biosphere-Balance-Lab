namespace BioSphereLab.DependencyInjection.Examples
{
    public interface IBiosphereSpawnPolicy
    {
        string Name { get; }

        int InitialPlants { get; }

        int InitialAnimals { get; }
    }
}
