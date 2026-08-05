# Biosphere Balance Lab Project Brief

## Purpose

Biosphere Balance Lab is a portfolio-oriented closed ecosystem simulation lab. The project should show how a complex Unity ECS simulation can be made understandable through a React dashboard, shared protocol design, and a future .NET backend boundary.

The first phase is not about building the final product. It is about creating a runnable MVP foundation that proves the technical direction is practical and gives later work a clear home.

Related technical boundaries are documented in `docs/tech-stack.md`.

## Portfolio Positioning

The project should stay balanced across Unity, React, and backend engineering. It should demonstrate:

* Unity simulation architecture with ECS-oriented data and systems
* React dashboard UX for configuration, observation, and explanation
* Shared contracts between simulation and frontend code
* .NET backend thinking for later scenario persistence, run history, and API design
* CI/CD and testing discipline as the project matures
* Clear documentation that helps technical reviewers understand tradeoffs

The target audience includes technical recruiters, Unity engineers, frontend engineers, backend/API reviewers, and product-minded engineering reviewers.

## Phase 1 Goal

Phase 1 establishes the runnable MVP foundation for the repository.

By the end of Phase 1, the project should have:

* A Unity project that opens and runs locally
* A Unity WebGPU viability check that shows whether WebGPU is a valid delivery direction for this project
* An inspectable initial ECS simulation foundation
* A React dashboard shell that runs locally
* A draft shared React/Unity protocol
* A repository structure that can support Unity, web, shared protocol, docs, CI, and future backend work
* A CI skeleton that points toward Unity and frontend validation
* Documentation that explains what exists, what is deferred, and why

## MVP Boundaries

Phase 1 should include only the foundation needed to make the project coherent and ready for end-to-end integration.

In scope:

* Define the Phase 1 MVP scope, non-goals, and success criteria
* Document the technical stack and architecture boundaries
* Stabilize the local Unity simulation foundation
* Validate Unity WebGPU feasibility with a minimal build or documented technical spike
* Expand the initial ECS biosphere resource model
* Implement the first simple ecosystem-cycle systems
* Scaffold a React dashboard shell
* Draft the shared React/Unity message protocol
* Add minimal CI workflow scaffolding

The MVP should favor readable, inspectable systems over polish. The simulation can be simple as long as it creates clear cause and effect between organisms, resources, and stability metrics.

## Phase 1 Non-Goals

Phase 1 should explicitly avoid expanding into work that belongs to later phases.

Out of scope:

* Polished Unity WebGPU/browser integration
* Production-ready WebGPU deployment
* Final React dashboard polish
* Real-time Unity-to-React integration
* .NET backend implementation
* Database persistence
* Scenario sharing or user accounts
* Scientific realism beyond a simple explainable ecosystem model
* Large-scale performance optimization
* Production deployment
* Public open-source packaging or licensing

WebGPU remains the preferred delivery direction and should be validated during Phase 1. The validation can be a minimal build, technical spike, or clearly documented feasibility check. Phase 1 does not need a polished embedded browser experience, but it should answer whether Unity WebGPU is a credible solution for this project before deeper integration work begins.

## Week 8 MVP Success Criteria

At a high level, the Week 8 MVP should be able to support a portfolio walkthrough where a reviewer can understand the product vision, inspect the technical architecture, and see the project moving toward an integrated simulation dashboard.

Success means:

* The repository structure clearly separates Unity, React, shared protocol, docs, CI, and future backend responsibilities
* Unity can run a local simulation foundation without depending on React or backend services
* Unity WebGPU has been validated enough to decide whether it remains the target delivery path
* The ECS model contains enough resource and entity data to support ecosystem-cycle behavior
* The first ecosystem systems produce observable or inspectable state changes
* The React app provides a dashboard shell for the future Unity viewport, controls, metrics, and event log
* The shared protocol draft defines commands, events, config, status, and metrics in a way React and Unity can both implement later
* CI scaffolding shows the intended validation path without pretending full automation is complete
* Documentation explains the current foundation, deferred work, and the reason for those boundaries

## Product Direction Guardrails

The project should feel like a lab tool, not a generic game prototype.

Good Phase 1 choices should make the system easier to explain later:

* Prefer named simulation concepts such as oxygen, CO2, water, nutrients, biomass, population, and stability
* Prefer simple resource loops that make cause and effect visible
* Prefer dashboard placeholders that map directly to future controls and metrics
* Prefer shared contracts over scattered string-based messages
* Prefer documentation that records decisions and boundaries while they are still fresh

Avoid adding features just because they are visually interesting. Every major addition should support the balanced portfolio story across Unity simulation, React product UI, backend/API thinking, and engineering workflow maturity.
