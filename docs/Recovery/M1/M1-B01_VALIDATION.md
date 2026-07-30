# M1-B01 Validation Notes

## Scope

This package contains only the Solution & Repository Foundation v1.0 deliverables. Existing Milestone 0 repository files are intentionally not included and must remain unchanged when this package is overlaid onto the repository root.

## Dependency Graph

- `CubeVault.Domain`: no project references.
- `CubeVault.Shared`: no project references.
- `CubeVault.Application` -> `CubeVault.Domain`.
- `CubeVault.Infrastructure` -> `CubeVault.Application`, `CubeVault.Domain`, `CubeVault.Shared`.
- `CubeVault.Cli` -> all source projects and serves as the composition root.
- Each test project references only its corresponding source project, in addition to centrally versioned test packages.

## Build Commands

```shell
dotnet restore CubeVault.sln
dotnet build CubeVault.sln --configuration Release --no-restore
dotnet test CubeVault.sln --configuration Release --no-build
```

No domain entities, value objects, services, repositories, persistence, OneStream integration, dependency injection, logging, business rules, or UI features are included.
