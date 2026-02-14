---
title: Architecture & Design
weight: 1
toc: true
---

This page describes the internal architecture of MicroWin, including its code structure, data flow, and key components.

## Technology Stack

| Component | Technology |
|-----------|-----------|
| Language | C# |
| UI Framework | Windows Forms (WinForms) |
| Runtime | .NET Framework 4.8 |
| Image Servicing | Microsoft.Dism (NuGet, v4.0.7) |
| ISO Management | WMI (`MSFT_DiskImage` / `MSFT_Volume`) |
| ISO Creation | oscdimg.exe |
| Registry | reg.exe (offline hive manipulation) |
| Logging | DynaLog (custom, v1.0.3) |

## Project Structure

```
MicroWin/
├── Program.cs                    # Entry point (STAThread)
├── MainForm.cs                   # Main window, page navigation
├── AppState.cs                   # Global application state
├── WizardPages.cs                # All 8 wizard page UserControls
├── functions/
│   ├── dism/
│   │   ├── DismManager.cs        # DISM API wrapper
│   │   ├── ImageModificationTask.cs  # Abstract base for mod tasks
│   │   ├── OsFeatureDisabler.cs  # Disables Windows features
│   │   ├── OsPackageRemover.cs   # Removes OS packages
│   │   ├── StoreAppRemover.cs    # Removes Store apps
│   │   └── UnattendGenerator.cs  # Creates unattend.xml
│   ├── iso/
│   │   └── IsoManager.cs         # ISO mount/extract/dismount
│   ├── OSCDIMG/
│   │   └── OscdimgUtilities.cs   # oscdimg.exe wrapper
│   └── Helpers/
│       ├── DeleteFiles/
│       │   └── DeleteFile.cs     # Safe directory deletion
│       ├── DynaLog/
│       │   └── DynaLog.cs        # Logging system
│       └── RegistryHelpers/
│           ├── RegistryHelper.cs # Offline registry operations
│           ├── RegistryItem.cs   # Registry value data class
│           └── ValueKind.cs      # Registry value types enum
└── tools/
    ├── oscdimg.exe               # ISO creation tool
    ├── FirstStartup.ps1          # Runs on first login
    └── RemoveTempDir.bat         # Temp directory cleanup
```

## Application Flow

### Entry Point

`Program.cs` is the entry point. It runs on an STA thread and launches `MainForm`.

### MainForm and Page Navigation

`MainForm` is a 600x450 WinForms window that hosts a panel for content switching. The wizard pages are `UserControl` instances defined as inner classes within `WizardPages.cs`. Navigation replaces the panel's content with the next page in sequence.

### Global State

`AppState` is a static class that holds all shared data across wizard pages:

- **Paths:** ISO path, temp root (`%TEMP%\microwin`), mount path, scratch path
- **User input:** Selected image index, user accounts, setup type, output ISO path
- **Options:** Reporting tool shortcut, auto setup mode

### Wizard Pages

The wizard consists of 8 sequential pages:

| Page | Purpose |
|------|---------|
| `Page_SelectISO` | Select and extract a Windows ISO |
| `Page_WinVersion` | Choose a Windows edition from the WIM |
| `Page_SetupType` | Select setup type (currently Auto only) |
| `Page_Users` | Add local user accounts |
| `Page_Tools` | Toggle optional tools and shortcuts |
| `Page_Save` | Set output ISO filename and location |
| `Page_Progress` | Execute all modifications (main work) |
| `Page_Finish` | Completion screen |

## Key Components

### IsoManager

Handles ISO file operations using WMI:

- **Mount:** Uses `MSFT_DiskImage` to mount the ISO and `MSFT_Volume` to get the assigned drive letter
- **Extract:** Copies all files from the mounted drive to the temp directory
- **Dismount:** Safely ejects the ISO via WMI `Dismount` method

### DismManager

Wraps the Microsoft.Dism NuGet package for WIM image operations:

- **GetWimVersions:** Reads all image entries from a WIM/ESD file, returning a dictionary of index to edition name
- **MountImage:** Mounts a WIM image at a specified index to the scratch directory
- **UnmountAndSave:** Commits modifications and unmounts
- **UnmountAndDiscard:** Unmounts without saving

### Image Modification Tasks

All modification tasks inherit from the abstract `ImageModificationTask` base class and implement an async execution method. They run sequentially during the Progress page:

**OsFeatureDisabler** disables Windows optional features, keeping:
- Windows Defender, Printing, TelnetClient, PowerShell, .NET Framework (NetFx), Media features, NFS, Search Engine, Remote Desktop

**OsPackageRemover** removes OS packages, keeping:
- ApplicationModel, language packs/features, .NET, Notepad, WMIC, Ethernet, WiFi, Foundation, VBScript, licenses, Windows Hello Face, ISE, OpenSSH, PMCPPC

**StoreAppRemover** removes Microsoft Store apps, keeping:
- App Installer, Microsoft Store, Notepad, Printing, Phone Link, Xbox, Windows Terminal, Calculator, Photos, VCLibs, Paint, Gaming services, Edge extensions, Windows Security, Snipping Tool, Cross Device

### UnattendGenerator

Generates an `unattend.xml` file for automated Windows setup (OOBE bypass). This file configures:

- Network requirement bypass
- OOBE screen suppression (EULA, wireless, OEM, online account)
- Local user account creation with specified passwords
- Synchronous first-logon commands for additional customization

### RegistryHelper

Provides offline registry manipulation by wrapping `reg.exe` commands. It can:

- Load and unload registry hives from mounted images
- Add, modify, and delete registry values
- Support all standard registry value types (REG_SZ, REG_DWORD, REG_QWORD, etc.)

### OscdimgUtilities

Handles ISO creation using `oscdimg.exe`:

1. First searches for oscdimg in an existing Windows ADK installation (via registry lookup)
2. Falls back to the bundled copy in the `tools/` directory, or downloads it from GitHub
3. Creates bootable ISOs with both BIOS and UEFI boot support

### DynaLog

Custom logging system (v1.0.3) that writes to `logs/MW_DynaLog.log`:

- UTC timestamps with millisecond precision
- Caller method name tracking
- Log archival after 14 days, deletion after 28 days
- Maximum of 5 concurrent log files

## Data Flow

```
User selects ISO
       │
       ▼
ISO mounted via WMI ──► Files extracted to %TEMP%\microwin
       │
       ▼
User selects edition, accounts, options
       │
       ▼
DISM mounts WIM image (install.wim)
       │
       ├──► OsFeatureDisabler runs
       ├──► OsPackageRemover runs
       ├──► StoreAppRemover runs
       ├──► Registry hives loaded and modified
       ├──► Unattend.xml generated and injected
       │
       ▼
DISM unmounts and saves WIM
       │
       ▼
Boot WIM also mounted, modified, and saved
       │
       ▼
oscdimg.exe creates final ISO
       │
       ▼
Temp files cleaned up
```

