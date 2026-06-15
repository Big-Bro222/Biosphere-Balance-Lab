# Dependency Injection Example

This folder shows how to use the small runtime DI system in `Assets/Scripts/DependencyInjection/Infrastructure/DependencyInjection`.

## Scene setup

1. Create a `StorageInjection` asset from `Create > Biosphere Balance Lab > Dependency Injection > Storage Injection`.
2. Add prefab entries such as `plant` or `animal` to the asset.
3. Add an empty GameObject named `Dependency Scope`.
4. Add `InjectionScope` to it and assign the `StorageInjection` asset.
5. Add `ExampleBiosphereDependencyInstaller` to the same GameObject.
6. Add `DependencyInjectionUsageExample` to any scene GameObject.

## Switch dependency

`ExampleBiosphereDependencyInstaller` registers two implementations for the same interface:

```csharp
container.Register<IBiosphereSpawnPolicy>(new BalancedBiosphereSpawnPolicy(), "balanced");
container.Register<IBiosphereSpawnPolicy>(new StressTestBiosphereSpawnPolicy(), "stress");
container.Switch<IBiosphereSpawnPolicy>("balanced");
```

At runtime, switch the active dependency:

```csharp
container.Switch<IBiosphereSpawnPolicy>("stress");
IBiosphereSpawnPolicy policy = container.Resolve<IBiosphereSpawnPolicy>();
```

## ECS usage

`InjectionScope` automatically registers the container against `World.DefaultGameObjectInjectionWorld`.
`ExampleSystem` inherits directly from `SystemBase`, is marked with `DisableAutoCreation`, and is manually registered by `GameBootstrapper`.
It resolves the active dependency from the current ECS world:

```csharp
public partial class ExampleSystem : SystemBase
{
    protected override void OnUpdate()
    {
        bool resolved = EcsInjectionRegistry.TryResolve(
            World,
            out IBiosphereSpawnPolicy policy);

        if (resolved)
        {
            // Use managed configuration or services here.
        }
    }
}
```

Keep simulation data in ECS components. Use DI for managed configuration, factories, prefab lookups, adapters, and bridge services.
