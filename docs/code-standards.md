# Code Standards

This document defines the coding conventions for Biosphere Balance Lab. Read it before making C# or Unity-side code changes.

## Scope

These standards apply to Unity project code under `unity/Assets/Scripts`.

Tool-enforced naming rules live in the repository `.editorconfig`. This document explains the intent and day-to-day usage.

## C# Naming

Use explicit, descriptive names. Prefer clarity over abbreviation.

| Symbol | Convention | Example |
| --- | --- | --- |
| Types | PascalCase | `CreatureInitSystem` |
| Interfaces | PascalCase with `I` prefix | `IInjectionContainer` |
| Public members | PascalCase | `TryGetPrefab` |
| Constants | PascalCase | `PlantCount` |
| Private fields | `m_` + camelCase | `m_container` |
| Private properties | `m_` + camelCase | `m_cachedValue` |
| Method parameters | `p_` + camelCase | `p_world` |
| `out` parameters | `o_` + camelCase | `o_container` |
| Local variables | camelCase | `normalizedKey` |

For parameters using `ref` or `in`, use the normal method parameter convention:

```csharp
public void OnUpdate(ref SystemState p_state)
{
}
```

For `out` parameters, both declarations and call-site inline variables should use `o_`:

```csharp
public bool TryResolve<TService>(out TService o_service, string p_key = null)
{
    o_service = null;
    return false;
}

if (container.TryResolve(out IBiosphereSpawnPolicy o_policy))
{
}
```

## Unity Serialization

Private serialized fields must still use the private field convention:

```csharp
[SerializeField] private StorageInjection m_storageInjection;
```

When renaming an existing serialized field, add `FormerlySerializedAs` so scene and asset data can migrate:

```csharp
[FormerlySerializedAs("storageInjection")]
[SerializeField] private StorageInjection m_storageInjection;
```

If a serialized asset, prefab, or scene is stored as text and currently uses the old field name, update the YAML field name when it is safe and obvious.

## C# Style

Use explicit type names in project code and examples instead of `var`.

Prefer early returns for guard clauses.

Keep methods focused and easy to scan. Introduce helper methods when they clarify Unity lifecycle code, ECS registration, or dependency setup.

Do not make broad formatting-only changes in unrelated files.

## Unity Architecture

Keep simulation logic separate from UI, browser integration, and editor tooling.

Use ECS where it demonstrates clear simulation or performance value. Do not force ECS into simple managed configuration or editor-only workflows.

Keep `MonoBehaviour` bridge code thin. It may coordinate scene references, installers, or Unity lifecycle events, but core simulation rules should live in testable systems or services.

Keep editor tools under an `Editor` folder and avoid runtime dependencies on `UnityEditor`.

## Dependency Injection

Use dependency injection for managed services, configuration, prefab lookup, factories, and bridge services.

Do not store ECS component data in managed services. Simulation state should remain in ECS components where practical.

Register dependencies through `IInjectionContainer` or project-level installers rather than scattering global lookups across systems.

## Documentation

When changing standards, update this file and `.editorconfig` together.

When changing architecture, simulation model, or React-Unity communication, also update the relevant document in `docs/`.
