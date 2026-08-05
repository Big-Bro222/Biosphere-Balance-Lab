# BioSphereLab Project Milestones

## Project Overview

**BioSphereLab** is an interactive WebGPU ecosystem simulation dashboard built with Unity ECS, React, and .NET.

The project demonstrates a closed biosphere simulation where users can configure environmental variables, run the simulation in the browser, inspect real-time ecosystem metrics, and analyze whether the system reaches balance or collapses over time.

## Core Technical Goals

This project is designed to demonstrate the following capabilities:

- Unity WebGPU deployment and browser integration
- Complex and highly customizable Unity ECS simulation architecture
- Unity automated testing with EditMode, PlayMode, and ECS-focused tests
- Large-scale data processing and performance optimization in Unity
- .NET backend API design for presets and simulation run history
- React dashboard UI for simulation control, metrics, and inspection
- CI/CD pipeline using GitHub Actions and GameCI

The stack and architecture boundaries for these areas are documented in `docs/tech-stack.md`.

---

# Milestone 1 — Unity ECS Simulation Core

## Goal

Build the core closed-biosphere simulation in Unity using ECS before connecting it to React or the backend.

The focus of this milestone is to prove that the ecosystem logic works independently.

## Key Features

- Generate initial biosphere entities:
  - Plants
  - Animals
  - Microbes
  - Soil cells
  - Atmosphere data
  - Water source
- Simulate basic ecosystem cycles:
  - Oxygen production
  - CO2 production
  - Water consumption
  - Plant growth
  - Animal respiration
  - Microbe decomposition
  - Nutrient recycling
- Detect ecosystem status:
  - Stable
  - Unstable
  - Collapsed

## ECS Systems

Initial ECS systems may include:

- `PhotosynthesisSystem`
- `RespirationSystem`
- `GrowthSystem`
- `WaterConsumptionSystem`
- `NutrientCycleSystem`
- `MicrobeDecompositionSystem`
- `DeathSystem`
- `PopulationMetricsSystem`
- `StabilityScoreSystem`

## Deliverables

- A Unity scene with a running ECS-based biosphere simulation
- Basic visual representation of plants, animals, and ecosystem resources
- Internal simulation metrics visible in Unity
- A simple stability score calculation
- Initial ECS architecture documentation

## Success Criteria

- The simulation can run without React or backend dependencies
- Plants, animals, microbes, water, oxygen, CO2, and nutrients affect each other
- The biosphere can reach stable, unstable, or collapsed states
- Core simulation data is structured in ECS components and systems

---

# Milestone 2 — Unity WebGPU and React Integration

## Goal

Embed the Unity WebGPU build inside a React application and establish basic communication between React and Unity.

The focus of this milestone is to turn the Unity simulation into a browser-based interactive experience.

## Key Features

- Build Unity project for WebGPU
- Embed Unity canvas inside the React app
- Add loading and error states
- Send simulation commands from React to Unity:
  - Start
  - Pause
  - Reset
  - Set simulation speed
  - Apply initial biosphere configuration
- Send simulation metrics from Unity to React:
  - Oxygen level
  - CO2 level
  - Water level
  - Plant biomass
  - Animal population
  - Nutrient level
  - Stability score

## Suggested Protocol

React to Unity commands:

```ts
type SimulationCommand =
  | { type: "START" }
  | { type: "PAUSE" }
  | { type: "RESET" }
  | { type: "SET_SPEED"; value: number }
  | { type: "APPLY_CONFIG"; payload: BiosphereConfig };
```

Unity to React events:

```ts
type SimulationEvent =
  | { type: "METRICS_UPDATE"; payload: BiosphereMetrics }
  | { type: "STATUS_CHANGED"; payload: BiosphereStatus }
  | { type: "WARNING"; payload: BiosphereWarning };
```

## Deliverables

- React app with embedded Unity WebGPU viewport
- Basic simulation toolbar
- Initial React-to-Unity command bridge
- Initial Unity-to-React metrics bridge
- Shared protocol documentation

## Success Criteria

- Unity WebGPU build runs inside the React app
- React can start, pause, reset, and configure the simulation
- React receives live metrics from Unity
- The integration is documented clearly enough for portfolio presentation

---

# Milestone 3 — React Dashboard and Product Experience

## Goal

Build a professional simulation dashboard around the Unity viewport.

The focus of this milestone is to make the project feel like a real product rather than a technical demo.

## Key Features

- Simulation toolbar:
  - Start
  - Pause
  - Reset
  - Speed control
- Biosphere configuration panel:
  - Light intensity
  - Water amount
  - Initial plant count
  - Initial animal count
  - Microbe activity
  - Temperature
  - Soil nutrient level
- Live metrics panel:
  - Oxygen
  - CO2
  - Water
  - Nutrients
  - Population
  - Stability score
- Timeline and charts:
  - Oxygen / CO2 over time
  - Population over time
  - Stability score over time
- Event log:
  - Resource warnings
  - Population collapse
  - Recovery events
  - Stability changes
- Entity inspector:
  - Select an entity in Unity
  - Display entity data in React
  - Show entity type, health, biomass, age, and resource state

## Suggested Layout

```txt
Left panel:     Simulation settings
Center:         Unity WebGPU viewport
Right panel:    Metrics and entity inspector
Bottom panel:   Timeline and event log
```

## Deliverables

- Polished React dashboard UI
- Reusable React components
- Real-time charts
- Event log
- Entity inspector
- UX notes explaining dashboard design decisions

## Success Criteria

- The dashboard clearly explains what is happening inside the simulation
- Users can configure, run, inspect, and analyze the biosphere
- The UI supports the portfolio story of making a complex system understandable

---

# Milestone 4 — .NET Backend for Presets and Run History

## Goal

Add a lightweight .NET backend to support scenario presets and simulation run history.

The focus of this milestone is to demonstrate backend thinking without overcomplicating the project.

## Key Features

- Save biosphere presets
- Load preset list
- Save simulation run summaries
- View previous simulation results
- Validate incoming configuration data
- Provide clean DTOs between frontend and backend

## Suggested API

```txt
GET    /api/presets
POST   /api/presets
GET    /api/presets/{id}

GET    /api/runs
POST   /api/runs
GET    /api/runs/{id}
```

## Example Data Models

```csharp
public record BiospherePresetDto(
    string Name,
    float LightIntensity,
    float InitialWater,
    int PlantCount,
    int AnimalCount,
    float MicrobeActivity,
    float Temperature,
    float SoilNutrients
);

public record SimulationRunSummaryDto(
    string PresetId,
    string FinalStatus,
    float FinalStabilityScore,
    int SimulatedDays,
    DateTime CreatedAt
);
```

## Deliverables

- ASP.NET Core Minimal API project
- Preset endpoints
- Run history endpoints
- Validation logic
- API documentation
- Frontend integration with backend

## Success Criteria

- Users can save and reload biosphere presets
- Users can view previous simulation run results
- The backend remains focused, lightweight, and easy to explain

---

# Milestone 5 — Testing, Optimization, and CI/CD

## Goal

Add automated testing, performance optimization, and CI/CD pipelines.

The focus of this milestone is to demonstrate engineering maturity and maintainability.

## Unity Testing

Test coverage may include:

- Plants increase oxygen when light and water are available
- Animals consume oxygen and produce CO2
- Dead biomass is converted into nutrients by microbes
- Plants stop growing when water is below the required threshold
- Biosphere collapses when oxygen remains too low
- Reset returns the simulation to the selected initial preset

## Performance Optimization

Optimization areas may include:

- Jobs and Burst for simulation systems
- Native containers for large-scale simulation data
- Reduced runtime allocations
- Entity query optimization
- Metric aggregation optimization
- Benchmarking with different entity counts

## Example Benchmark Targets

```txt
Entity Count | Target
1,000        | Smooth baseline simulation
5,000        | Optimized simulation test
10,000+      | Stress test / benchmark mode
```

## CI/CD Workflows

Suggested GitHub Actions workflows:

```txt
.github/workflows/
├── unity-tests.yml
├── unity-webgpu-build.yml
├── frontend-build.yml
├── backend-build.yml
└── release.yml
```

Workflow responsibilities:

- Run Unity tests with GameCI
- Build Unity WebGPU artifact
- Run React lint and build
- Run .NET build and tests
- Upload build artifacts
- Optionally create release builds

## Deliverables

- Unity EditMode / PlayMode tests
- React build validation
- .NET build and test validation
- GitHub Actions workflows
- GameCI-based Unity test/build automation
- Performance benchmark documentation

## Success Criteria

- Core simulation logic is covered by automated tests
- The project can be built through CI
- Unity WebGPU build can be produced as an artifact
- Performance results are documented and portfolio-ready

---

# Final Portfolio Deliverables

By the end of the project, the portfolio package should include:

- Demo video
- Public case study
- Architecture diagram
- ECS system diagram
- React dashboard screenshots
- Performance benchmark summary
- Testing and CI/CD summary
- Selected code snippets
- Private source repository
- Public-facing technical write-up

## Suggested Portfolio Title

**BioSphereLab - An Interactive WebGPU Ecosystem Simulation Dashboard**

## Suggested One-Sentence Summary

**BioSphereLab is a browser-based closed ecosystem simulation tool built with Unity ECS, WebGPU, React, and .NET, designed to demonstrate scalable simulation architecture, real-time data visualization, automated testing, and CI/CD engineering practices.**
