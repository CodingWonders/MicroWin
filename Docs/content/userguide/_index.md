---
title: User Guide
weight: 2
---

> [!WARNING]
> **Only use MicroWin if you have experience with Windows systems management.** This tool makes significant modifications to Windows images and requires administrator privileges. If you are unfamiliar with DISM, Windows imaging, or offline servicing, consider learning with [DISMTools](https://github.com/CodingWonders/DISMTools) first.

## Quick Start

1. Download the latest release from the [GitHub releases page](https://github.com/CodingWonders/MicroWin/releases)
2. Extract the archive to a folder of your choice
3. Run `MicroWin.exe` as an administrator
4. Follow the wizard to select an ISO, configure your options, and create a customized image

> [!NOTE]
> MicroWin is not signed with a code-signing certificate due to the cost of obtaining one. You may need to disable your antivirus or add an exclusion before running it. Please do not open issue reports about antivirus detections.

## System Requirements

| Requirement | Details |
|-------------|---------|
| Operating System | Windows 10 or Windows 11 |
| Runtime | .NET Framework 4.8 |
| Privileges | Administrator |
| Disk Space | 15-25 GB free in system temp directory |
| Input | A Windows 10 or 11 ISO file |

## Guides

- [Getting Started](getting-started/) — Full step-by-step walkthrough of the MicroWin wizard

## What MicroWin Does

MicroWin takes a standard Windows ISO and produces a customized version with:

- **Bloatware removed** — Pre-installed Store apps, non-essential OS packages, and optional features are stripped out
- **Privacy improvements** — Copilot, OneDrive, Content Delivery Manager, and News and Interests are removed or disabled
- **Hardware check bypasses** — TPM, Secure Boot, RAM, CPU, and storage requirement checks are bypassed for Windows 11
- **Automated setup** — An unattend.xml is injected to skip OOBE screens and auto-create local user accounts
- **BitLocker prevention** — Automatic BitLocker encryption is disabled

Essential apps like Calculator, Photos, Paint, Windows Terminal, Windows Security, and the Microsoft Store are kept intact.

## Output

The output is a standard bootable ISO file that can be:

- Written to a USB drive using tools like [Rufus](https://rufus.ie/)
- Burned to a DVD
- Used with a virtual machine (Hyper-V, VirtualBox, VMware, etc.)

