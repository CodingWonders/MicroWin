# MicroWin .NET 10 Upgrade Tasks

## Overview

Upgrade the `MicroWin` solution (2 projects) from .NET Framework to .NET 10.0 by performing an atomic, all-project upgrade (project file conversion, package updates, build fixes) followed by test execution and final commit.

**Progress**: 3/4 tasks complete (75%) ![0%](https://progress-bar.xyz/75)

---

## Tasks

### [✓] TASK-001: Verify prerequisites *(Completed: 2026-03-17 10:06)*
**References**: Plan §Implementation Timeline Phase 0, Plan §Detailed Execution Steps Step 1

- [✓] (1) Verify .NET 10 SDK is installed on the execution machine(s) (`dotnet --list-sdks`) per Plan §Implementation Timeline Phase 0
- [✓] (2) Runtime/sdk presence meets requirement for `net10.0` (**Verify**)
- [✓] (3) Inspect `global.json` (if present) for pinned SDK; update or remove to allow .NET 10 SDK as described in Plan §Detailed Execution Steps Step 1
- [✓] (4) `global.json` is compatible with .NET 10 or removed/updated (**Verify**)

### [✓] TASK-002: Atomic framework and package upgrade with compilation fixes *(Completed: 2026-03-17 10:12)*
**References**: Plan §Migration Strategy, Plan §Implementation Timeline Phase 1, Plan §Detailed Execution Steps Steps 2-5, Plan §Project-by-Project Migration Plans

- [✓] (1) Convert all project files to SDK-style and update `TargetFramework` values per Plan §Detailed Execution Steps Step 2 and Project-by-Project Migration Plans (e.g., `MicroWin\MicroWin.csproj` → `net10.0-windows`, `TestProjects\UnattendedWriter\UnattendedWriter.csproj` → `net10.0`)
- [✓] (2) All project files updated to SDK-style and target frameworks set (**Verify**)
- [✓] (3) Update package references per Plan §Package Update Reference (retain `Microsoft.Dism` unless build indicates otherwise; investigate and replace/remove `Microsoft.UI.Xaml` as required)
- [✓] (4) Package updates applied or incompatibilities flagged for remediation (**Verify**)
- [✓] (5) Restore dependencies at solution root (`dotnet restore`) per Plan §Detailed Execution Steps Step 4
- [✓] (6) All dependencies restore successfully (**Verify**)
- [✓] (7) Build the solution and fix all compilation errors caused by the framework/package upgrades (reference Plan §Breaking Changes Catalog for common Windows Forms and `System.Drawing` issues)
- [✓] (8) Solution builds with 0 errors (**Verify**)

### [✓] TASK-003: Run full test suite and validate upgrade *(Completed: 2026-03-17 10:12)*
**References**: Plan §Implementation Timeline Phase 2, Plan §Testing & Validation Strategy

- [✓] (1) Run tests in all test projects (`dotnet test`) per Plan §Testing & Validation Strategy
- [✓] (2) Fix any test failures caused by runtime or API behavioral changes (reference Plan §Breaking Changes Catalog)
- [✓] (3) Re-run tests after fixes
- [✓] (4) All tests pass with 0 failures (**Verify**)

### [ ] TASK-004: Final commit
**References**: Plan §Source Control Strategy

- [ ] (1) Commit all remaining changes with message: "TASK-004: Complete upgrade to .NET 10.0 (MicroWin)"









