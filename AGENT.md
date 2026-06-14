# AGENTS.md

## Project overview

This repository is for **Biosphere Balance Lab**, a portfolio-oriented Unity WebGPU + React project.

The project is an interactive closed biosphere / ecosystem balance simulator. It should demonstrate:

* Unity WebGPU integration
* A complex and highly customizable Unity ECS simulation system
* Unity unit tests
* Large data processing and optimization
* React frontend skills
* .NET backend thinking
* CI/CD with GitHub Actions and GameCI
* UX/product design thinking
* Clear technical communication

This is a job-seeking portfolio project. Prefer practical, explainable, portfolio-friendly architecture over over-engineering.

## Core product direction

The demo should feel like a lab tool where users can configure and observe a closed ecosystem.

Possible simulation concepts:

* Organisms
* Resources
* Energy flow
* Population balance
* Environmental parameters
* Scenario presets
* System stability / collapse indicators
* Data visualization and comparison

The project should not become a generic game prototype. Prioritize systems design, simulation clarity, visual explanation, and technical communication.

## Architecture principles

* React is the preferred frontend framework.
* Unity WebGPU is the preferred Unity delivery target.
* Unity should own real-time simulation and visualization.
* React should own dashboard UI, configuration panels, charts, scenario controls, and portfolio-friendly user flows.
* .NET backend should be introduced when it adds clear value, such as saving scenarios, processing simulation results, or exposing API design.
* Avoid unnecessary microservices or overly complex cloud infrastructure.
* Keep the architecture understandable for recruiters and hiring managers.

## Important project documents

Before making architectural, dependency, or framework-level decisions, read:

* `docs/project-brief.md`
* `docs/tech-stack.md`
* `docs/architecture.md`
* `docs/decision-log.md`
* `docs/roadmap.md`

If these files do not exist yet, propose the minimal file structure before making large changes.

## Technology boundaries

Do not replace the core stack unless explicitly asked.

Preferred stack:

* Unity
* Unity WebGPU
* Unity ECS / Entities where useful
* Unity Jobs / Burst where useful
* C#
* React
* TypeScript
* .NET Web API
* GitHub Actions
* GameCI

Do not repeatedly explain that Unity WebGPU is experimental unless the risk directly affects a concrete technical decision.

Do not add a public open-source license unless the user explicitly asks. This project is intended to remain private for now.

## Repository structure expectations

Prefer this structure unless the repository already uses a different clear structure:

```text
/
  AGENTS.md
  README.md
  docs/
    project-brief.md
    tech-stack.md
    architecture.md
    decision-log.md
    roadmap.md
  app/
    web/
    unity/
  backend/
  shared/
    protocol/
  .github/
    workflows/
```

Expected responsibilities:

* `app/web/`: React frontend
* `app/unity/`: Unity project
* `backend/`: .NET backend
* `shared/protocol/`: shared message contracts, schemas, or generated types
* `docs/`: project planning and technical communication
* `.github/workflows/`: CI/CD workflows

## Coding guidelines

### General

* Keep changes small and easy to review.
* Prefer clear naming over clever abstractions.
* Do not introduce large dependencies without explaining why.
* Do not rewrite unrelated files.
* Preserve existing project structure unless there is a clear reason to change it.
* When making architectural changes, update the relevant docs.

### React / TypeScript

* Use TypeScript.
* Prefer functional React components.
* Keep UI state and simulation state clearly separated.
* Keep Unity communication code isolated from general UI components.
* Prefer readable component structure over premature abstraction.
* Add lightweight tests when practical.

### Unity / C#

* Keep simulation systems modular and testable.
* Prefer ECS where it demonstrates clear value.
* Avoid mixing UI-specific concerns into simulation logic.
* Keep WebGPU/browser integration concerns separate from core simulation logic.
* Add Unity tests for simulation rules and data transformations where practical.
* Use clear names for components, systems, authoring scripts, and simulation concepts.

### .NET backend

* Keep the backend simple and explainable.
* Prefer clean API boundaries.
* Avoid introducing database complexity before it is needed.
* Use DTOs/contracts that can be understood by both frontend and Unity integration code.
* Add tests for core business logic when practical.

## Unity + React communication

When working on communication between React and Unity:

* Define message contracts clearly.
* Prefer typed shared protocol definitions where possible.
* Keep command names explicit, for example `SetEnvironmentParameter`, `LoadScenario`, `PauseSimulation`, `ResumeSimulation`.
* Avoid scattering string-based messages across the codebase.
* Document protocol changes in `docs/architecture.md` or `shared/protocol/README.md`.

## Portfolio quality requirements

Whenever adding a major feature, consider how it will be explained in the portfolio.

Good features should help demonstrate at least one of:

* Unity ECS technical depth
* WebGPU integration
* React dashboard/product UI
* .NET API/backend thinking
* Performance optimization
* Data processing
* UX clarity
* Testing discipline
* CI/CD maturity

Avoid features that are fun but hard to explain as portfolio value.

## Testing and validation

Before finishing a coding task, run relevant checks only if a user asks.

Use the commands defined by the repository. If they do not exist yet, suggest adding them.

Expected future checks:

```bash
# React
cd app/web
npm install
npm run lint
npm run test
npm run build

# .NET
cd backend
dotnet restore
dotnet test
dotnet build

# Unity
# Run Unity EditMode / PlayMode tests through Unity Test Runner or GameCI when configured.
```

Do not claim tests were run unless they were actually run.

If a command cannot be run in the current environment, explain what was skipped and why.

## CI/CD expectations

Prefer GitHub Actions for CI/CD.

Expected future workflows:

* React lint/test/build
* .NET restore/build/test
* Unity tests with GameCI
* Unity WebGPU build when feasible
* Optional deployment workflow for portfolio demo

Do not add heavy CI workflows before the corresponding local commands are stable.

## MCP usage

Use MCP servers only when they add clear value.

Recommended usage:

* Use Microsoft Learn MCP when checking .NET, ASP.NET, C#, or official Microsoft platform behavior.
* Use Playwright MCP when inspecting or testing the React frontend in a browser.
* Use GitHub MCP when working with issues, pull requests, or repository metadata.
* Use Figma MCP only when implementing UI from design files.

Do not use MCP tools for simple local edits that can be solved by reading the repository.

## Documentation rules

Update documentation when changing:

* Architecture
* Tech stack
* Major folder structure
* Simulation model
* React-Unity communication protocol
* CI/CD workflow
* Portfolio positioning

Use English for project documentation.

Keep documentation practical and readable. The target audience includes technical recruiters, Unity developers, frontend developers, and product/design reviewers.

## Decision log

When making a meaningful technical decision, update `docs/decision-log.md`.

Use this format:

```md
## YYYY-MM-DD — Decision title

Decision:
- What was chosen.

Reason:
- Why this choice fits the portfolio and architecture.

Alternatives considered:
- Option A
- Option B

Impact:
- What this affects.
```

## What not to do

* Do not turn this into a generic game project.
* Do not over-engineer the backend.
* Do not add unnecessary cloud infrastructure.
* Do not add unrelated AI features just because they sound impressive.
* Do not hide important logic inside hard-to-review abstractions.
* Do not make large formatting-only changes.
* Do not introduce new package managers or build tools without a clear reason.
* Do not add a public open-source license unless explicitly requested.

## Communication style

When reporting work:

* Summarize what changed.
* Mention which files were changed.
* Mention what tests or checks were run.
* Mention any limitations or follow-up work.
* Be direct and practical.
