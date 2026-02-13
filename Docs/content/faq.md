---
title: Frequently Asked Questions
toc: true
---

## What is MicroWin?

MicroWin is a Windows system customization tool that lets you modify Windows ISO images to remove bloatware, disable unnecessary features, and create a streamlined Windows installer. It is written in C# and provides a wizard-based GUI for the entire process.

## What Windows versions does MicroWin support?

MicroWin supports **Windows 10** and **Windows 11** ISO images. It works with both `install.wim` and `install.esd` image formats.

## Does MicroWin require administrator privileges?

Yes. MicroWin must be run as an administrator because it uses DISM (Deployment Image Servicing and Management) to mount and modify Windows images, which requires elevated privileges.

## My antivirus is blocking MicroWin. Is it safe?

MicroWin is not signed with a code-signing certificate due to the cost of obtaining one. This may cause antivirus software to flag it. The project is open source, so you can review the code yourself on [GitHub](https://github.com/CodingWonders/MicroWin). To use MicroWin, add an exclusion in your antivirus or temporarily disable it.

## How much disk space do I need?

You need approximately **15-25 GB of free space** in your system's temp directory. MicroWin extracts the ISO contents and mounts WIM images for modification, which requires significant temporary storage.

## What does MicroWin actually remove from Windows?

MicroWin removes or disables the following:

- **Store apps** — Most pre-installed Microsoft Store apps are removed, with exceptions for essential apps like Calculator, Photos, Paint, Windows Terminal, Windows Security, and the Microsoft Store itself.
- **OS packages** — Non-essential OS packages are removed, while keeping core components like .NET, Notepad, networking, language packs, and PowerShell.
- **Windows features** — Optional features are disabled, with exceptions for Defender, Printing, PowerShell, Remote Desktop, and others.

For the full lists, see [Architecture & Design](dev/architecture/).

## Can I choose which apps or features to keep?

Not currently. The removal and retention lists are hardcoded. Customizable selection is planned for a future release.

## What registry changes does MicroWin make?

MicroWin applies several registry modifications to the offline Windows image, including:

- Disabling Windows Platform Binary Table (WPBT) execution
- Skipping the first logon animation
- Setting PowerShell execution policy to RemoteSigned
- Bypassing hardware checks (TPM, Secure Boot, RAM, CPU, storage)
- Removing Copilot and OneDrive
- Disabling Content Delivery Manager (app suggestions)
- Disabling News and Interests
- Preventing automatic BitLocker encryption

## Does MicroWin bypass Windows 11 hardware requirements?

Yes. The modified image includes registry entries that bypass TPM, Secure Boot, RAM, CPU, and storage checks. This allows installation on hardware that does not meet Microsoft's official Windows 11 requirements.

## What is the output format?

MicroWin produces a standard **ISO file** that you can burn to a USB drive or DVD, or use with a virtual machine. The ISO is created using `oscdimg.exe`.

## Do I need Windows ADK installed?

It is not strictly required. MicroWin will look for `oscdimg.exe` from an existing Windows ADK installation. If the ADK is not found, it will download `oscdimg.exe` automatically from GitHub.

## Where are temporary files stored?

Temporary files are stored in `%TEMP%\microwin`. This includes the extracted ISO contents and mounted WIM images. These files are cleaned up after the process completes.

## I found a bug. Where do I report it?

Please open an issue on the [GitHub repository](https://github.com/CodingWonders/MicroWin/issues). Include as much detail as possible, including the log file from `logs/MW_DynaLog.log` in the application directory.

