# TrainGame — Dev Notes for Claude

Train board game built with Godot 4.6 (C#, .NET 8).

## Workflow rules

- **Plan first.** Always present a plan and wait for approval before implementing.
- **Test interactively before committing.** Run the game (`godot --path .`) and/or run `dotnet test` and confirm things actually work. Type checks and builds are not enough.
- **Ask before `git push`.** Commits are fine to make when asked; pushes always need explicit confirmation.
- **Prefer TDD.** Write a failing test first, then the implementation to make it pass. Applies to logic/model code; UI-only work is exempt when tests would be contrived.

## Project layout

```
TrainGame.sln
TrainGame.csproj          # game project (Godot.NET.Sdk)
project.godot
Scenes/                   # .tscn files
Scripts/
  Logic/                  # game rules, pure C#
  Model/                  # data types, pure C#
  UI/                     # Godot node scripts
Resources/
Data/
Tests/
  TrainGame.Tests.csproj  # xUnit, references game csproj
```

`Tests/**` is excluded from `TrainGame.csproj` via `<Compile Remove="Tests/**" />` so the game project doesn't try to compile test code.

## Commands

- Run the game headed: `/Applications/Godot_mono.app/Contents/MacOS/Godot --path .`
- Open editor: `open -a Godot_mono project.godot`
- Build: `dotnet build TrainGame.sln`
- Test: `dotnet test TrainGame.sln`
