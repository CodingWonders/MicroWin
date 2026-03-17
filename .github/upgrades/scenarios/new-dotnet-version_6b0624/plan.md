# .NET 10 Upgrade Plan for MicroWin

Table of Contents
- Executive Summary
- Migration Strategy
- Implementation Timeline
- Detailed Execution Steps
- Package Update Reference
- Project-by-Project Migration Plans
  - MicroWin\MicroWin.csproj
  - TestProjects\UnattendedWriter\UnattendedWriter.csproj
- Detailed Dependency Analysis
- Breaking Changes Catalog
- Testing & Validation Strategy
- Risk Management and Mitigation
- Source Control Strategy
- Success Criteria
- Appendices & Action Items


---

## Executive Summary

Selected Strategy
**All-At-Once Strategy** — Update all projects simultaneously in a single atomic operation.

Rationale
- Solution size: 2 projects (small), which fits All-At-Once guidance.
- No inter-project dependencies requiring staged migration.
- Test projects are limited and can be executed after the atomic upgrade.
- Assessment shows heavy Windows Forms API incompatibilities; upgrading all projects together reduces repeated compatibility churn and simplifies dependency resolution.

Key Metrics (from assessment)
- Total projects: 2
- Projects requiring upgrade: 2
- Primary project `MicroWin\MicroWin.csproj` lines of code: 3,943 (heavy WinForms surface)
- Test project `TestProjects\UnattendedWriter` lines of code: 182
- NuGet packages observed: 2 (`Microsoft.Dism` v4.0.7 — compatible; `Microsoft.UI.Xaml` v2.8.7 — incompatible)
- Top risk: Numerous Windows Forms binary incompatibilities (see Breaking Changes Catalog)

Critical issues
- `MicroWin\MicroWin.csproj` is a classic (non-SDK) WinForms project targeting `net48`. It must be converted to an SDK-style project and target `net10.0-windows`.
- `TestProjects\UnattendedWriter\UnattendedWriter.csproj` is a classic project targeting `net48` and should be converted to SDK-style and target `net10.0`.
- `Microsoft.UI.Xaml` is flagged as not compatible with `net10.0` in this codebase and requires investigation (remove/replace/update).

Decision
- Proceed with an All-At-Once atomic upgrade on branch `upgrade-to-NET10` (single coordinated change). This keeps the upgrade steps consolidated, simplifies cross-project package resolution, and aligns with the solution characteristics (small project count).

---

## Migration Strategy

Approach: All-At-Once (atomic)
- Convert all project files to SDK-style.
- Change target frameworks to `net10.0-windows` for WinForms app and `net10.0` for console/test apps.
- Update package references where required; apply all package updates flagged in assessment.
- Restore, build and fix compilation errors in a single coordinated pass.
- Run tests and validate full-solution behavior.

Dependency ordering rationale
- There are no project-to-project dependencies that require phased migration. All projects will be updated simultaneously.

Parallelization
- Because this is an atomic upgrade, developers may work in parallel on the single upgrade branch, but the upgrade itself is to be applied as a single commit set.

Phases (for human understanding)
- Phase 0: Preparation and prerequisites
- Phase 1: Atomic Upgrade (project file edits, package updates, restore, build and fix)
- Phase 2: Test Validation and finalization

---

## Implementation Timeline (Phases)

Phase 0: Preparation (prerequisites)
- Ensure .NET 10 SDK is installed on the build machine(s) and CI.
- Validate `global.json` (if present) references compatible SDK; update or remove to allow .NET 10 SDK.
- Verify that branch `dotnet10` (source) exists and create `upgrade-to-NET10` branch from it.
- Ensure working directory is clean or follow the chosen pending change action.

Phase 1: Atomic Upgrade (single coordinated batch)
- Convert all projects to SDK-style and update `TargetFramework` values.
- Update or remove incompatible package references (see Package Update Reference).
- Restore dependencies across solution.
- Build the solution and fix compilation errors caused by framework & package updates.
- Verify the solution builds successfully.

Phase 2: Test Validation
- Execute all test projects and unit tests.
- Address test failures.
- Execute any available integration or automated UI tests.
- Finalize commit and open pull request for review.

Deliverable for Phase 1: Solution builds with 0 compiler errors.
Deliverable for Phase 2: All automated tests pass (where available) and manual verification checklist completed.

---

## Detailed Execution Steps

### Step 1: Preparation
- Install .NET 10 SDK on the machine(s) performing the upgrade.
- Run `dotnet --list-sdks` to confirm `10.x` SDK presence.
- Update or remove `global.json` to reference an SDK compatible with `net10.0` (if `global.json` pins older SDK, update to a `10.x` or remove for the upgrade run then restore afterward).
- Create and switch to branch `upgrade-to-NET10` from source branch `dotnet10`:
  - `git checkout dotnet10`
  - `git checkout -b upgrade-to-NET10`

### Step 2: Convert project files to SDK-style and update TargetFramework
- For `MicroWin\MicroWin.csproj` (WinForms app):
  - Replace the legacy classic project file with an SDK-style project header similar to:

    ```xml
    <Project Sdk="Microsoft.NET.Sdk.WindowsDesktop">
      <PropertyGroup>
        <TargetFramework>net10.0-windows</TargetFramework>
        <UseWindowsForms>true</UseWindowsForms>
        <Nullable>enable</Nullable>
        <ImplicitUsings>enable</ImplicitUsings>
      </PropertyGroup>
      ...
    </Project>
    ```

  - Preserve important project settings (resources, embedded files, COM references) by translating them into SDK-style equivalents.
  - Inspect and migrate custom `AssemblyInfo`/`Resources`/`Settings` to the SDK-style pattern.

- For `TestProjects\UnattendedWriter\UnattendedWriter.csproj`:
  - Convert to an SDK-style project, e.g. `<Project Sdk="Microsoft.NET.Sdk">` and set `<TargetFramework>net10.0</TargetFramework>`.

Notes
- Some properties and `<Reference>` elements used in classic projects require specific SDK-style attributes (`<PackageReference>`, `<Reference Include=...>` with `HintPath`, or `<COMReference>` translation). Keep a side-by-side copy of the original project file until validation complete.
- If the code uses `app.config` heavily, retain `System.Configuration.ConfigurationManager` NuGet as a bridge if necessary; plan to migrate to `Microsoft.Extensions.Configuration` over time.

### Step 3: Update package references
- Apply all package updates flagged by assessment (must not be skipped). Specific actions:
  - `Microsoft.Dism` (current: 4.0.7): Assessment marks as compatible. Keep existing version unless CI/build signals otherwise.
  - `Microsoft.UI.Xaml` (current: 2.8.7): Marked incompatible. Options:
    - Investigate if there is a `winui` or `Microsoft.UI.Xaml` version compatible with `net10.0-windows` and replace if available.
    - If the project does not use WinUI features at runtime, remove the package and related code paths.
    - If WinUI is required, consider isolating WinUI usage into a separate project targeting the appropriate platform, or migrate to supported WinUI packages for .NET 10.

- For any other transitive packages discovered during restore/build that the compiler flags as incompatible, update them to versions which support `net10.0` (use `dotnet list package --outdated` and `upgrade_get_supported_package_version_for_project` tool if available).

### Step 4: Restore & Build
- Perform a full restore: `dotnet restore` at the solution root.
- Build the solution: `dotnet build -c Release`.
- Expect compilation errors due to API changes; capture them and prioritize by frequency and project impact.

### Step 5: Fix compilation & API issues (single coordinated pass)
- Fix source-incompatible and binary-incompatible API usages discovered by the build.
- Focus areas (based on analysis): Windows Forms control APIs, System.Drawing usage, and any obsolete patterns.
- Keep a running catalog of code changes required and group them by type (control property changes, constructor changes, event signature changes, etc.).
- Apply code changes and re-run a single full rebuild until the solution builds with 0 errors.

### Step 6: Run tests and validate
- Discover test projects and run them (unit tests and any automated integration tests).
- Address any failing tests caused by runtime behavioral changes.

### Step 7: Finalize and cleanup
- Remove temporary compatibility NuGet packages if no longer needed.
- Update documentation/comments mentioning target framework.
- Commit all upgrade changes in a single logical commit (or grouped commits but pushed as the upgrade PR) and open a pull request for review.

---

## Package Update Reference

### Packages observed in assessment
- `Microsoft.Dism` — current: `4.0.7` — Assessment: Compatible. Action: retain unless build indicates update needed.
- `Microsoft.UI.Xaml` — current: `2.8.7` — Assessment: Incompatible. Action: Investigate and replace or remove. No suggested version provided; investigator must locate `winui` packages compatible with `net10.0-windows` or remove usage.

⚠️ Action item: Use `upgrade_get_supported_package_version_for_project` (or `dotnet list package --outdated`) to find supported package versions for `net10.0` before committing.

---

## Project-by-Project Migration Plans

### Project: `MicroWin\\MicroWin.csproj`
**Current State**: Classic .NET Framework 4.8 WinForms app (non-SDK). Uses `Microsoft.Dism` (4.0.7) and `Microsoft.UI.Xaml` (2.8.7).

**Target State**: SDK-style targeting `net10.0-windows` with `<UseWindowsForms>true` and `Microsoft.NET.Sdk.WindowsDesktop`.

**Migration Steps**:
1. Create a backup of the original `.csproj` file.
2. Convert to SDK-style using `Microsoft.NET.Sdk.WindowsDesktop` and set `<TargetFramework>net10.0-windows</TargetFramework>` and `<UseWindowsForms>true`.
3. Migrate NuGet references to `<PackageReference>` tags; retain `Microsoft.Dism` and investigate `Microsoft.UI.Xaml`.
4. Migrate settings/resources/AssemblyInfo as required to SDK-style defaults.
5. Run `dotnet restore` and `dotnet build` to collect compilation issues.
6. Address build errors (binary incompatibilities and source incompatibilities). Focus on UI code and `System.Drawing` usage.
7. Rebuild until zero errors.
8. Run application-level smoke tests (manual or automated) on Windows to verify UI and behavior.

**Expected Breaking Areas**:
- Windows Forms controls, layout, and property access patterns (see Breaking Changes Catalog).
- `System.Drawing` constructors and font handling.
- Any P/Invoke or COM interop usage may require attention to platform targets.

**Validation Checklist**:
- [ ] `MicroWin` project file is converted to SDK-style and targets `net10.0-windows`.
- [ ] `dotnet restore` completes without unresolved package errors.
- [ ] Solution builds with 0 errors.
- [ ] App starts (manual UI smoke) without immediate crashes.
- [ ] Relevant unit/integration tests pass (where applicable).

---

### Project: `TestProjects\\UnattendedWriter\\UnattendedWriter.csproj`
**Current State**: Classic .NET Framework console/test helper targeting `net48` (classic project style).

**Target State**: SDK-style `<Project Sdk="Microsoft.NET.Sdk">` targeting `net10.0`.

**Migration Steps**:
1. Convert `.csproj` to SDK-style and set `<TargetFramework>net10.0</TargetFramework>`.
2. Convert any `<Reference>` entries to `<PackageReference>` if applicable.
3. `dotnet restore` and `dotnet build`.
4. Fix any API or configuration differences.
5. Run tests for this project.

**Validation Checklist**:
- [ ] Project converted to SDK-style and targets `net10.0`.
- [ ] Restores and builds successfully.
- [ ] Relevant tests pass.

---

## Detailed Dependency Analysis

Summary
- No project-to-project references were detected. Projects are independent and can be migrated together safely.
- No circular dependencies.

Critical path
- `MicroWin` must be validated carefully because it contains almost all production code and the majority of API incompatibilities. However, no other project depends on it, so the atomic upgrade remains manageable.

---

## Breaking Changes Catalog (Highlights)

Based on analysis, the most common and impactful areas to validate and fix:

1. Windows Forms binary incompatibilities
   - Control constructors and initialization sequences may differ.
   - Properties such as `Control.Name`, `Control.Size`, `Control.Location`, `Control.Anchor`, `Control.Dock` and `Control.Controls` manipulations can surface source or binary incompatibilities.
   - Methods like `SuspendLayout`/`ResumeLayout` usage should be revalidated.

2. Controls and layout types
   - `TableLayoutPanel`, `ColumnHeader`, `ListView` and `PictureBox` behavior should be validated.

3. System.Drawing changes
   - `Font` constructors and `GraphicsUnit`/`FontStyle` semantics may require constructor argument changes.
   - Consider replacing heavy `System.Drawing` usage with supported packages if planning cross-platform targets; for Windows-only desktop apps `System.Drawing.Common` remains usable but may need package reference adjustments.

4. Behavior changes
   - Some runtime behaviors and default values can differ; add targeted tests to verify expected behavior in the UI.

5. Native interop, COM, and Registry APIs
   - Ensure P/Invoke signatures remain valid; check `DllImport` declarations and platform targets.
   - Registry usage (code uses `Microsoft.Win32.Registry`) is supported, but verify behavior in .NET 10 runtime.

Notes: The full set of 2,801 binary incompatibilities should be triaged based on build errors; many will be resolved during code compilation fixes.

---

## Testing & Validation Strategy

Levels of validation
1. Unit tests: Run all unit tests. Fix or mark flakey tests.
2. Build validation: Full solution build required.
3. Functional smoke tests: Start the WinForms app and verify major flows (ISO extraction, image mounting, driver export/import, unattended generation).
4. Integration tests: If automated integration tests exist, run them.
5. Security/vulnerability scan: Re-run NuGet vulnerability scans after package updates.

Automated steps
- Use CI pipeline with .NET 10 SDK installed to run `dotnet restore`, `dotnet build`, `dotnet test`.
- Add a temporary job to run the WinForms binary on a Windows runner to perform smoke checks if automation is available.

Validation checklist (solution-level)
- [ ] All projects target `net10.0` or `net10.0-windows` as appropriate
- [ ] `dotnet restore` passes
- [ ] `dotnet build` passes with zero errors
- [ ] All unit and integration tests pass
- [ ] No outstanding security vulnerabilities in NuGet packages

---

## Risk Management and Mitigation

Primary risks
- High number of Windows Forms binary incompatibilities (high risk).
- `Microsoft.UI.Xaml` package incompatibility: may require rework or removal.
- Possible runtime behavioral changes in UI and System.Drawing.

Mitigations
- Use All-At-Once approach to avoid repeated churn across projects.
- Keep the original project files and a backup branch to allow a rollback if the upgrade proves blocking.
- Create a short list of high-priority UI scenarios for manual testing after the upgrade.
- Keep `global.json` and SDK pinned only if necessary; otherwise prefer a flexible SDK selection for upgrade.

Contingency & rollback
- If the atomic upgrade introduces hard blockers that cannot be fixed in the single pass, revert branch and consider a targeted incremental strategy for the problematic project(s).
- Keep a branch snapshot of the pre-upgrade state to allow revert.

---

## Source Control Strategy

- Create branch: `upgrade-to-NET10` from `dotnet10` (source branch identified during initialization).
- Perform the entire atomic upgrade as a single logical change set on `upgrade-to-NET10`.
- Commit strategy: Group related changes (project file conversions, package updates, code fixes) in a small number of coherent commits but present them as one PR for reviewer convenience.
- PR requirements: Must include build verification (CI) and a reviewer with Windows Desktop/WinForms experience.

Note: This conforms to All-At-Once strategy guidance: prefer a single commit/PR that updates all projects simultaneously.

---

## Success Criteria

The migration is complete when:
- All projects target their proposed frameworks (`net10.0-windows` for WinForms, `net10.0` for helpers/tests).
- All package updates from assessment applied or explicitly documented with justification.
- Solution builds cleanly with 0 compiler errors.
- All automated tests pass.
- No unresolved NuGet security vulnerabilities remain.

---

## Appendices & Action Items

Immediate action items (before starting the atomic pass)
1. Ensure .NET 10 SDK installed on all build machines and CI.
2. Create branch `upgrade-to-NET10` from `dotnet10`.
3. Backup existing project files and commit the backup to the branch (or tag pre-upgrade state).
4. Investigate `Microsoft.UI.Xaml` compatibility for `net10.0-windows`. Decision required: replace, remove, or isolate.
5. Gather any designer-generated `.resx` or `.Designer.cs` files that might require conversion tweaks.
6. Prepare CI job to run `dotnet restore`, `dotnet build`, and `dotnet test` using .NET 10 runner.

Items flagged for investigation
- `Microsoft.UI.Xaml` incompatibility — locate supported package or remove usage.
- Any COM or native references that fail after moving to SDK-style project.

---

If you want, I can now:
- Generate `TASK-001` (atomic upgrade task markdown) based on this plan, or
- Produce a condensed checklist for an executor to apply the changes.

End of plan.md
