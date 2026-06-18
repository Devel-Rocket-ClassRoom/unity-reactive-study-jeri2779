# AGENTS.md

This file provides guidance to Codex (Codex.ai/code) when working with code in this repository.

## Project Overview

**Defendency_Injection** is a Unity project in early development stage, used to explore Dependency Injection in Unity with the **VContainer** framework. The current code is a minimal "Hello World" DI sample (entry point, service, and lifetime scope).

## Environment

- **Unity Version**: 6.0.3 (6000.3.15f1)
- **Render Pipeline**: Universal Render Pipeline (URP) 17.3.0
- **Input System**: New Input System 1.19.0
- **DI Framework**: VContainer 1.18.0 (via git package)
- **C# Version**: .NET Framework compatible with Unity 6

## Project Structure

```
Assets/
├── Scenes/
│   └── SampleScene.unity            # Main scene
├── Scripts/                           # DI sample scripts
│   ├── HelloWorldLifetimeScope.cs   # VContainer LifetimeScope (composition root)
│   ├── GreetingEntryPoint.cs        # IStartable entry point
│   └── GreetingService.cs           # Sample service resolved via DI
├── Settings/                          # URP render pipeline assets & volume profiles
└── TutorialInfo/                     # README and tutorial assets
```

## Key Dependencies

- **jp.hadashikick.vcontainer** (1.18.0) — Dependency Injection framework (installed via git URL)
- **com.unity.ai.navigation** (2.0.12) — AI navigation for pathfinding
- **com.unity.inputsystem** (1.19.0) — New Input System for input handling
- **com.unity.render-pipelines.universal** (17.3.0) — URP for rendering
- **com.unity.test-framework** (1.6.0) — Unit testing support
- **com.unity.timeline** (1.8.12) — Animation and cinematic support

> Note: VContainer is referenced as a git package in `Packages/manifest.json`. A clone needs git available so the Package Manager can resolve it.

## Building and Running

### Editor Play Mode
1. Open the project in Unity 6.0.3
2. Load `Assets/Scenes/SampleScene.unity`
3. Press **Play** in the Editor

### Build for Standalone
```bash
# Build is configured in Unity Project Settings > Build
# Use Editor: File > Build Settings > Build
```

## Code Style and Conventions

- **Language**: C# (.NET Framework compatible)
- **Formatter**: CSharpier (auto-applied via PostToolUse hook)
- **Naming**:
  - PascalCase for classes, methods, properties
  - camelCase for local variables and parameters
  - `_field` prefix for private serialized fields (if using Inspector)
- **No file restrictions** on scripts in `Assets/Scripts/` or custom folders
  - **Do not edit** auto-generated IDE project files (`.csproj`, `.sln`)
  - **Do not edit** scene files (`.unity`) directly — use Unity MCP or Editor UI instead
  - **Do not edit** metadata files (`.meta`)

## Unity-Specific Notes

### Dependency Injection (VContainer)
- `HelloWorldLifetimeScope` is the composition root — register services in its `Configure(IContainerBuilder)`
- Entry points implement VContainer interfaces (e.g. `IStartable`) and are registered via `RegisterEntryPoint`
- Keep services plain C# classes where possible; resolve dependencies through constructor injection

### Scene Management
- Main scene is `Assets/Scenes/SampleScene.unity`
- Use Unity MCP (when connected) to query/modify scenes programmatically from Codex

### Input System
- Project uses the **New Input System** (not the legacy Input Manager)
- Input actions are configured in `Assets/InputSystem_Actions.inputactions`

## Common Tasks

### Run Tests
```bash
# Tests are configured in com.unity.test-framework
# Run via Unity Editor: Window > General > Test Runner
```

### Code Formatting
After editing `.cs` files, the CSharpier hook automatically formats code on save (via `.codex/hooks/csharpier-format.sh`).

### Connect Unity MCP
To interact with the running Unity Editor from Codex:
1. Open **Edit > Project Settings > AI > Unity MCP Server**
2. Ensure **Unity Bridge** is **Running** (green)
3. Expand **Integrations** and click **Configure** next to your client
4. In a terminal, accept the pending connection (Unity shows it under **Pending Connections** → **Allow**)
5. Use tools like `Unity_GetConsoleLogs`, `Unity_RunCommand`, `Unity_SceneView_Capture2DScene`

> The relay binary is installed per-PC at `~/.unity/relay/` when the Editor loads. A local `.mcp.json` (Windows path, git-ignored) can be added for CLI auto-connect; it is intentionally not committed because the relay path differs per OS/user.

## Git Workflow

- `.gitignore` excludes Unity auto-generated folders (`Library/`, `Temp/`, `Logs/`, `obj/`, `Build/`)
- `.codex/hooks.json` runs the CSharpier formatter after edits
- `.claude/settings.local.json` and `.mcp.json` are git-ignored (machine/OS-specific)
- Commit changes to C# scripts, scenes (via changes only), and configuration files
- Use `git status` to verify only intended files are staged

## Next Steps

1. Register additional services and entry points in `HelloWorldLifetimeScope`
2. Expand `SampleScene.unity` with interactive elements wired through DI
3. Add tests under an `Assets/Tests/` assembly using the Test Framework
