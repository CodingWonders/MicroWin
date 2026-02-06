# MicroWin

![Release](https://img.shields.io/github/v/release/CodingWonders/MicroWin?color=blue&label=Latest%20Release)
![License: MIT](https://img.shields.io/badge/License-MIT-green)
![Platform: Windows](https://img.shields.io/badge/Platform-Windows%2010%20%2F%2011-0078d4)

**MicroWin** is an open-source, standalone utility designed to surgically debloat and optimize Windows 10 and 11 images. It provides a specialized environment for creating streamlined, high-performance Windows installations by removing telemetry, unnecessary apps, and background bloat.

### 📜 Evolution & History
Originally developed as a flagship component of the **ChrisTitusTech WinUtil** suite, **MicroWin was officially converted into its own independent project on Thursday, February 5th, 2026.** This strategic decoupling allows for a dedicated C# architecture, a robust native GUI, and rapid development cycles independent of the main WinUtil script.

---

## 📂 Project Structure
To maintain professional standards and ensure code maintainability, the project follows a clean "Separation of Concerns" architecture:

| Directory | Description |
| :--- | :--- |
| **`/functions/dism`** | DISM logic. |
| **`/functions/iso`** | ISO logic. |

---

## 🛠 Getting Started

### For Users
1.  Navigate to the **[Releases](https://github.com/CodingWonders/MicroWin/releases)** page.
2.  Download the latest `MicroWin.exe`.
3.  **Run as Administrator** to ensure full access to Windows administration tools like DISM.

### For Developers
1.  **Clone the Repository:**
    `git clone https://github.com/CodingWonders/MicroWin.git`
3.  **Build:** Open the solution in **Visual Studio 2022**. Requires the **.NET Desktop Development** workload.

---

## 🤝 Contributing
MicroWin is a community-driven project. We welcome direct contributions to the main build.
* **Feature Branches:** Please create a branch for new features rather than committing directly to `main`.
* **Testing:** All changes **MUST** be verified in a Virtual Machine (VM) before submitting a Pull Request is submitted.
* **Architecture:** Keep the logic in the correct folders.

---

## 📜 License
This project is licensed under the **MIT License**. See the [LICENSE](LICENSE) file for more information.

---

## 📝 Credits
MicroWin is developed and maintained by the community, with special thanks to the original contributors of the WinUtil project for the foundational logic.
