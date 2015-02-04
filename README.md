Getting Started
====================

Starting framework and sample code for Unity ports to Windows.

You can view an overview of the architecture here:
https://www.dropbox.com/s/w8wt0au5602vl57/MarkerMetro.Unity.WinShared.jpg

Prerequisites
==================

- Visual Studio 2013 with latest updates
- Nuget Package Manager
- Unity 4.6.1f1 is required

Setup
==================

## Starting a new porting project

To initialize an exiting Unity project using WinShared code you can use Init.ps1 script, located in root of this repo.

You will need to provide following:

- TargetRepoPath - full path to root folder of new repository (for example: `C:\Code\SomeProject\`)
- UnityProjectTargetDir - subdirectory under _TargetRepoPath_ that contains Unity project (can be empty, if it is in the root of repo, for example: `Unity` for Unity project to be in `C:\Code\SomeProject\Unity`)
- ProjectName - name of new project (ensure same as ProductName in PlayerSettings). Script will rename Windows projects, namespaces and solutions to match this name.

This script will copy the following files and folders to your target project:

* .gitignore - Up to date git ignore for Unity projects (you should manually merge this if there is an existing one)
* /BuildScripts/* - helper build scripts
* /Assets/MarkerMetro/Editor/* - helper editor scripts, including Tools > MarkerMetro menu options
* /Assets/MarkerMetro/Example/* - see FaceFlip.unity, a small optional game scene with [WinIntegration](https://github.com/MarkerMetro/MarkerMetro.Unity.WinIntegration) test points (recommended to see WinIntegration features in action)
* /Assets/Plugins/* - plugin binaries and scripts
- WindowsSolution - boilerplate windows 8.1/Windows Phone 8.0 solution folder, enhanced version of what Unity outputs
- WindowsSolutionUniversal - boilerplate windows 8.1 Universal solution folder, enhanced version of what Unity outputs

Guidance
==================

Provided here is guidance for working with WinShared based projects which you should read and understand but are not directly related to setting up a new project.

## Marker Metro Tools Menu

Tools > MarkerMetro provides some useful features for Unity developers porting to Windows.

### Build Menu

Alows you to quickly build to Windows 8.l, Windows Phone 8.0, Windows Universal or All (cycling through Windows 8.1, Windows Phone 8.0 and Windows Universal). 

### Features Menu

This allows to enable/disable some commonly used Windows features. Things like having the sound options in the charm settings or displaying an IAP disclaimer when opening the game for the first time.

These features can be set in the _Features_ window under the _MarkerMetro_ menu on Unity.

These settings are stored in a file called `WinSharedSettings.xml`, so you must add this file to your version control system once it becomes updated.

### Plugins Menu

This repository includes a stable version of all dependent Marker Metro plugins. You can easily update to later versions, as well as use and debug local versions of WinLegacy and WinIntegration.

Plugins > Update allows you to update the Marker Metro plugins (WinIntegration and WinLegacy) to the latest version. By default this option gets latest stable versions via the main public NuGet field, but you can also configure to use local solution folders if you are using your own fork for example.

Building the plugins locally allows you also to easily debug a particular Windows Store or Windows Phone plugin project as follows:

- Add the platform specific plugin project to your Windows solution (e.g. MarkerMetro.Unity.WinIntegrationMetro)
- Tools > MarkerMetro > Plugins > Configure ( ensure Plugin Source is Local, and Build Local is Debug)
- Tools > MarkerMetro > Plugins > Update
- Tools > MarkerMetro > Build > (e.g. Windows 8.1)
- Set breakpoints in your platform specific plugin project and then F5 on your app

## Windows Solution Build Output

There are 2 base folders to be used for all Unity Projects, WindowsSolution (Windows 8.1 and Windows Phone 8.0) and WindowsSolutionUniversal (Windows 8.1 and Windows Phone 8.1 Universal). This contains the Windows Store and Windows Phone apps.

You can then subsequently build out from Unity using the Tools > MarkerMetro > Build menu or specify the path manually via a standard File > Build Settings build. 

Note that currently these projects are for Windows 8.1 and Windows Phone 8.0

## Sample Game and [WinIntegration](https://github.com/MarkerMetro/MarkerMetro.Unity.WinIntegration) Samples

The FaceFlip.unity scene demonstrates some key ]WinIntegration](https://github.com/MarkerMetro/MarkerMetro.Unity.WinIntegration) features around a simple game such as:

- Facebook Integration
- Store Integration
- Notifications
- Platform Specific Debug Info
- Crash and Exception Testing (if configured)

## QA and Master configurations

The Windows Solutions have QA and Master solution configurations. QA has been copied from Master to provide a production ready configuration that can support testing. 

At this point the differences are:

- Store Integration - QA uses simulated IAP, Master will attempt to use real production store APIs.
- Exception Logging - QA enables ability to crash test via Windows settings charm, but will be off by default. Master will hide this capability but ensure Exception Logging is on by default (if a key has been supplied)
- All for Assets.Plugins.MarkerMetro.DeviceInformation.GetEnvironment() call in Unity to return QA or Production based QA/Master configuraiton at runtime which can be useful to apply environment specific login within the game side.

## Windows Phone Low Memory Optimization

There is a script that tries to optimize assets settings to lower memory usage, which is useful specially for Windows Phone 8.
You can find it at `\Assets\Editor\MarkerMetro\MemoryOptimizer.cs`.
Please refer to the code documentation for instructions on how to use it.

## App Name Localization

Windows 8.1 and Windows Phone 8.1 app manifest uses AppName and AppDescription resources to localize the store name, application name and description.

Windows Phone 8.0 manifest uses a build-process generated AppResLib.dll[*.mui] files that pull strings from resources automatically:
- ApplicationTitle is used to generate resource string 100, wich is used to Application Display Name
- ApplicationTileTitle is used to generate resource string 101, wich is used to set Tile Title in manifest
- ApplicationDescription is used to generate resource string 102, which is used to set Application Description

If any of these strings are missing from resources, AppResLibGenerator will report a warning.

AppResLibGenerator is referenced as [Nuget Package](https://www.nuget.org/packages/MarkerMetro.WindowsPhone.AppResLibGenerator/) and is also on [Github](https://github.com/MarkerMetro/AppResLibGenerator)
