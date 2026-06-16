# Dependency Injection

Runtime dependency registration for scene objects and ECS systems.

## Namespaces

- Runtime infrastructure: `BioSphereLab.DependencyInjection.Infrastructure`
- Examples: `BioSphereLab.DependencyInjection.Examples`

## Setup

1. Create a `StorageInjection` asset from `Create > BioSphereLab > Dependency Injection > Storage Injection`.
2. Add prefab entries such as `plant` or `animal` to the asset.
3. Add an empty GameObject named `Dependency Scope`.
4. Add `InjectionScope` to it and assign the `StorageInjection` asset.
5. Add `ExampleBiosphereDependencyInstaller` to the same GameObject.
6. Add `DependencyInjectionUsageExample` to any scene GameObject.

## Runtime Flow

```text
InjectionScope
  -> IInjectionInstaller
  -> InjectionContainer
  -> EcsInjectionRegistry
  -> ExampleSystem
```

## Switch Dependencies

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

## ECS Usage

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
