# MicroWin
This repository contains the source code for the MicroWin CANARY version. MicroWin CANARY allows us to work more quickly and efficiently on the next version of MicroWin that will be available on WinUtil (https://github.com/ChrisTitusTech/winutil).

The point of this repository is **NOT** to ditch MicroWin from WinUtil, but rather to allow us to work on the next version of MicroWin in a more efficient manner. Once the next version of MicroWin is ready, it will be merged back into WinUtil. I will merge the changes.

*Decoupling* it from WinUtil will only make it able to run independently.

**THIS REPOSITORY IS CURRENTLY MARKED AS PRIVATE because I'm not fully sure if it will be viable. Once this is decoupled from WinUtil and contributing to the CANARY version becomes more viable, I will make this repository public.**

<!-- by the way, chris, copilot wanted me to point to your social media sites while working on this paragraph -->
Those who are looking at this file have access to this repository. This essentially works like a repository owned by a team, in the sense that you can contribute directly, without forking. Still, I recommend that you create branches.

## Developing

To work on changes for MicroWin, you have to do the following:

1. Clone this repo
2. Run `envsetup.cmd` and answer the questions:

<img width="999" height="651" alt="vmware_fkEgHwjWMW" src="https://github.com/user-attachments/assets/58836862-be59-4223-b303-6e3be388ece3" />

3. Create your branch
4. Work on your changes **AND TEST THEM**
5. Create a PR

The plan is to make it run independently first before making any test suites. Then we can make a control panel and an entry point script.
