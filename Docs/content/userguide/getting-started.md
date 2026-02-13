---
title: Getting Started with Microwin
weight: 1
---

This guide walks you through using MicroWin to create a customized Windows ISO.

## Prerequisites

Before you begin, make sure you have:

- **Windows 10 or Windows 11** as your host operating system
- **Administrator access** on your machine
- **A Windows ISO file** (Windows 10 or 11) — you can download official ISOs from [Microsoft](https://www.microsoft.com/software-download/)
- **15-25 GB of free disk space** in your system temp directory for extraction and modification
- **Antivirus exclusion** — MicroWin is not code-signed, so you may need to add an exclusion or temporarily disable your antivirus

## Installation

1. Download the latest release from the [GitHub releases page](https://github.com/CodingWonders/MicroWin/releases)
2. Extract the archive to a folder of your choice
3. Right-click `MicroWin.exe` and select **Run as administrator**

> [!NOTE]
> MicroWin requires .NET Framework 4.8, which is included with Windows 10 version 1903 and later. If you are on an older version, you may need to install it manually.

## Step-by-Step Walkthrough

### Step 1: Select an ISO

Click **Browse** to select your Windows ISO file. MicroWin will mount the ISO and extract its contents to a temporary directory. This may take several minutes depending on your disk speed.

The tool supports ISOs containing either `install.wim` or `install.esd` image formats.

### Step 2: Choose a Windows Edition

After extraction, MicroWin reads the available Windows editions from the image (e.g., Home, Pro, Enterprise, Education). Select the edition you want to customize.

### Step 3: Setup Type

Choose your setup type. Currently, only **Auto Setup (Automated OOBE)** is available. This configures Windows to skip the Out-of-Box Experience screens during installation, allowing for a faster, unattended setup.

### Step 4: Configure User Accounts

Add the local user accounts you want to be created during Windows installation. For each account, specify:

- **Username**
- **Password**
- **Role** — Administrator or Standard User

You can add multiple accounts. These will be automatically created when Windows is installed from the modified ISO.

### Step 5: Tools and Shortcuts

Optionally enable additional tools to be included in the image:

- **Reporting Tool** — Adds a PowerShell-based reporting tool shortcut to the desktop

### Step 6: Save Location

Choose where to save the output ISO file. The default filename is `MicroWin.iso`. Pick a location with enough free space to store the final image.

### Step 7: Deployment

Click **Start** to begin the modification process. MicroWin will:

1. Mount the WIM image using DISM
2. Disable unnecessary Windows features
3. Remove bloatware OS packages
4. Remove pre-installed Microsoft Store apps
5. Apply registry modifications (hardware check bypasses, Copilot removal, OneDrive removal, etc.)
6. Generate and inject an unattend.xml for automated setup
7. Modify the boot WIM image
8. Create the final ISO using oscdimg

This process can take a significant amount of time depending on your hardware. Progress is displayed on screen.

### Step 8: Finish

Once complete, your customized ISO is ready. You can:

- **Burn it to a USB drive** using tools like [Rufus](https://rufus.ie/)
- **Burn it to a DVD**
- **Use it with a virtual machine** (Hyper-V, VirtualBox, VMware, etc.)

## What Gets Modified

For a detailed breakdown of what MicroWin removes, keeps, and changes in the Windows image, see the [Architecture & Design](../../dev/architecture/) page.

## Troubleshooting

- **Not enough disk space:** Ensure you have at least 15 GB free in your system temp directory (`%TEMP%`)
- **ISO creation fails:** MicroWin needs `oscdimg.exe`. It will try to find it from a Windows ADK installation or download it. If you are offline without ADK, ISO creation will fail
- **Antivirus interference:** Add MicroWin to your antivirus exclusions
- **Check the logs:** Diagnostic information is written to `logs/MW_DynaLog.log` in the application directory