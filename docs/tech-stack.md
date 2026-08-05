# Tech Stack and Architecture Boundaries

## Purpose

This document defines the intended technical stack for Biosphere Balance Lab and the boundary between each major area of the project. The goal is to keep the architecture practical, explainable, and portfolio-friendly while Phase 1 establishes the runnable MVP foundation.

## Current Stack Snapshot

| Area | Current or intended choice | Role |
| --- | --- | --- |
| Unity | Unity `6000.4.11f1` | Owns the real-time biosphere simulation and visual simulation viewport |
| Unity ECS | `com.unity.feature.ecs` `1.0.0`, Entities `6.4.0` from the lock file | Structures simulation data and systems |
| Rendering | Universal Render Pipeline `17.4.0` | Provides the current render pipeline for the Unity scene |
| Input | Input System `1.19.0` | Handles future simulation viewport input where needed |
| Testing | Unity Test Framework `1.6.0` | Supports future EditMode, PlayMode, and simulation rule tests |
| Frontend | React, Vite, TypeScript | Owns the dashboard shell, controls, metrics, charts, and product UX |
| Shared protocol | JSON-message contracts under `shared/protocol` | Defines commands, events, config, status, and metrics shared by React and Unity |
| Backend | Future ASP.NET Core Web API | Owns scenario presets, run history, validation, and API design when needed |
| CI/CD | GitHub Actions, future GameCI | Validates frontend, Unity, and backend work as local commands become stable |

## Unity Boundary

Unity owns simulation execution and the primary visual representation of the ecosystem.

Unity should contain:

* ECS components for organisms, resources, lifecycle state, and simulation metrics
* ECS systems for initialization, resource cycles, growth, death, decomposition, and aggregation
* Local simulation scenes used to validate the MVP without React or backend dependencies
* Thin bridge code for future communication with the React host
* Unity tests for simulation rules and data transformations when practical

Unity should not contain:

* Dashboard layout logic
* Long-lived scenario persistence
* Backend API concerns
* Browser-specific UI state beyond the minimum needed for integration
* Hard-coded string protocol messages scattered through systems

Phase 1 should verify that the Unity project opens and runs locally. It should also validate whether Unity WebGPU is a credible delivery path through a minimal build, technical spike, or documented feasibility check.

## React Boundary

React owns the user-facing dashboard experience around the simulation.

React should contain:

* Dashboard layout and navigation
* Simulation controls
* Configuration panels
* Metrics panels and charts
* Event log and inspection UI
* Unity viewport host shell
* Protocol adapter code that translates UI actions into shared commands

React should not contain:

* Authoritative simulation rules
* ECS-like simulation state that competes with Unity
* Direct knowledge of Unity implementation internals beyond the shared protocol
* Backend persistence logic outside API client boundaries

Phase 1 should scaffold the React app under the web application area and create clear placeholder regions for the future Unity viewport, toolbar, metrics panel, and event log. It does not need polished charts or real Unity integration yet.

## Shared Protocol Boundary

The shared protocol is the contract between React and Unity.

It should define:

* Commands from React to Unity, such as start, pause, reset, speed changes, and configuration updates
* Events from Unity to React, such as metrics updates, status changes, warnings, and selection events
* Simulation configuration shape
* Metrics snapshot shape
* Simulation status values
* Versioning or compatibility notes once integration begins

The protocol should start as simple TypeScript-friendly JSON contracts. C# DTO mirrors can be added later when Unity integration work begins. The important Phase 1 outcome is a clear contract draft that prevents command and event names from spreading as unrelated strings.

## Backend Boundary

The backend is intentionally future work. It should be introduced when it adds clear portfolio value instead of as default infrastructure.

The future .NET backend should own:

* Saved scenario presets
* Simulation run summaries
* Validation for persisted configuration data
* Clean REST API boundaries
* DTOs that can be understood by both React and Unity-facing code

The backend should not own:

* Real-time simulation execution
* Unity rendering or scene state
* React dashboard state
* Complex infrastructure before persistence is needed

Phase 1 should document this boundary but should not implement the backend.

## CI/CD Boundary

CI should grow with the project instead of pretending the final automation story exists on day one.

Phase 1 CI should prepare:

* A frontend validation path for install, lint, test, or build once the React app exists
* A Unity validation path for future GameCI or Unity Test Runner usage
* Clear comments or documentation for required secrets, licenses, or setup steps

Later CI can add:

* Unity EditMode and PlayMode tests
* Unity WebGPU build artifacts
* Frontend lint/test/build checks
* Backend restore/build/test checks
* Optional portfolio deployment once the app is ready

## Phase 1 Boundaries

Phase 1 includes:

* Local Unity simulation foundation
* Unity WebGPU viability validation
* Initial ECS resource model
* First simple ecosystem-cycle systems
* React dashboard shell
* Shared protocol draft
* Minimal CI scaffolding
* Practical documentation

Phase 1 excludes:

* Polished embedded Unity WebGPU experience in React
* Production-ready deployment
* Final dashboard design
* Real-time React/Unity integration
* .NET backend implementation
* Database persistence
* User accounts or scenario sharing
* Large-scale performance optimization

## Decision Principles

When choosing technology or adding structure:

* Prefer the existing stack unless there is a clear reason to change it
* Keep boundaries visible in folder structure and documentation
* Add dependencies only when they reduce real implementation risk
* Keep the project understandable to Unity, frontend, backend, and product-minded reviewers
* Record meaningful architecture decisions in `docs/decision-log.md`
