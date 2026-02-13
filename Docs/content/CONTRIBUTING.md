---
title: Contributing
toc: true
---

Thank you for your interest in contributing to MicroWin! This guide covers everything you need to get started.

## Requirements

- **Visual Studio 2026** (or later)
- **.NET Desktop development** workload installed
- **Windows 10 or Windows 11** as your development OS
- A Windows ISO file for testing your changes

## Setting Up Your Development Environment

1. **Fork** the [MicroWin repository](https://github.com/CodingWonders/MicroWin) on GitHub
2. **Clone** your fork locally:
   ```bash
   git clone https://github.com/<your-username>/MicroWin.git
   ```
3. Open `MicroWin.csproj` in Visual Studio
4. Restore NuGet packages (Visual Studio should do this automatically)
5. Build the project to verify everything compiles

The project targets **.NET Framework 4.8** and uses the **Microsoft.Dism** NuGet package for image servicing.

## Making Changes

1. Create a new branch for your work:
   ```bash
   git checkout -b my-feature
   ```
2. Make your changes
3. **Test your changes thoroughly** — MicroWin modifies Windows images, so untested changes can produce broken ISOs
4. Commit your work with a clear, descriptive commit message

## Testing

Before submitting a pull request, please verify:

- The project builds without errors or warnings
- Your changes work end-to-end with an actual Windows ISO
- Existing functionality is not broken by your changes
- The application still runs correctly as administrator

> [!IMPORTANT]
> MicroWin makes significant modifications to Windows images. **Always test your changes** against a real ISO before submitting a pull request.

## Submitting a Pull Request

1. Push your branch to your fork
2. Open a pull request against the `main` branch of the [upstream repository](https://github.com/CodingWonders/MicroWin)
3. Describe what your changes do and why
4. Reference any related issues if applicable

## Code Guidelines

- Follow the existing code style and naming conventions in the project
- Use async/await for long-running operations to keep the UI responsive
- Use the `DynaLog` logging system for diagnostic output instead of `Console.WriteLine`
- Place new functionality in the appropriate namespace under `functions/` (e.g., DISM operations in `functions/dism/`, helpers in `functions/Helpers/`)

## Reporting Bugs

If you find a bug but don't have a fix, please [open an issue](https://github.com/CodingWonders/MicroWin/issues) on GitHub. Include:

- Steps to reproduce the problem
- The Windows edition and ISO you were using
- The log file from `logs/MW_DynaLog.log` in the application directory