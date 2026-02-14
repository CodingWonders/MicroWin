---
title: Known Issues
toc: true
---

This page lists known issues and limitations in the current **ALPHA** release of MicroWin.

## Antivirus False Positives

MicroWin is **not signed with a code-signing certificate** due to the high cost of obtaining one. As a result, some antivirus software may flag the executable as suspicious.

**Workaround:** Disable your antivirus or add an exclusion for MicroWin before running it. Please do not open issue reports about antivirus detections.

## Incomplete Logging

Several internal components do not yet log errors or diagnostic information consistently. This can make it harder to troubleshoot failures during image modification. Improved logging coverage is planned for a future release.

## Silent Exception Handling

In some cases, errors during DISM operations or registry modifications may be caught and silently ignored. If the output ISO behaves unexpectedly, this may be the cause. Check the log file at `logs/MW_DynaLog.log` in the application directory for any recorded errors.

## Disk Space Requirements

MicroWin extracts the full contents of a Windows ISO to a temporary directory and then mounts WIM images for modification. This can require **15-25 GB of free disk space** in your system's temp folder (`%TEMP%\microwin`), depending on the Windows edition. The tool does not currently warn you if there is insufficient space.

## Single Setup Type

The wizard currently only supports **Auto Setup (Automated OOBE)**. A manual setup option is not yet available.

## Hardcoded Package and App Lists

The lists of Windows features, OS packages, and Store apps that are kept or removed are hardcoded. There is currently no UI to customize which items are removed or retained. See [Architecture & Design](dev/architecture/) for details on what is kept by default.

## Windows 10+ Only

MicroWin only supports Windows 10 and Windows 11 ISO images. Older Windows versions are not supported.

## oscdimg Dependency

ISO creation requires `oscdimg.exe`. MicroWin will attempt to locate it via a Windows ADK installation, and will fall back to downloading it from GitHub if not found. If you are offline and do not have the ADK installed, ISO creation will fail.

