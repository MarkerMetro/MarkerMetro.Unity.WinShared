Getting Started
====================

Starting framework code for Unity ports to Windows.

You can view an overview of the architecture here:
https://www.dropbox.com/s/w8wt0au5602vl57/MarkerMetro.Unity.WinShared.jpg

Prerequisites
==================

- Visual Studio 2013 with latest updates
- Nuget Package Manager
- Unity 4.6 (although 4.5 should work as well)

Setup
==================

## Scripting a new porting project

The approach when starting a new porting project is to simply copy and paste the Unity Project and Windows Solution and across to the client repo as detailed below:

To initialize a project using WinShared code you can use Init.ps1 script, located in root of the WinShared project.
This will copy the .gitignore, NuGet, Assets and Windows Solution folders, while renaming UnityProject.

You will need to provide following:

- TargetRepoPath - full path to root folder of new repository (for example: `C:\Code\SomeProject\`)
- UnityProjectTargetDir - subdirectory under _TargetRepoPath_ that contains Unity project (can be empty, if it is in the root of repo, for example: `Unity` for Unity project to be in `C:\Code\SomeProject\Unity`)
- ProjectName - name of new project. Script will rename Windows projects, namespaces and solutions to match this name.

## The Unity Project

The root of the repo contains a Unity project created for testing purposes, however it houses a number of useful scripts that we want to make use of in all projects.

If you don't use _Init.ps1_ to initialize the project you can copy and paste just the following folders and files into the root of client repo's Unity project. The client repo will normally have it on root but could be in a sub folder, so make sure you copy to the correct location.

* .gitignore - Up to date git ignore for Unity projects (you should manually merge this if there is an existing one)
* /Nuget/* - helper scripts to bring in Nuget based plugins to Unity
* /Assets/MarkerMetro/Editor/* - helper editors scripts
* /Assets/Plugins/* - plugin binaries and scripts
* /Assets/Example/* - see FaceFlip.unity, a small game with win integration test points (optional, but you can test out facebook, exception logging, and some helper class method outputs for example)

Ensure that the ProductName in PlayerSettings is correct and matches the game name. It this value that will be used to script the Windows Solution and set up the build server.

## Windows Solution Output

There are 2 base folders to be used for all Unity Projects, WindowsSolution (Windows 8.1 and Windows Phone 8.0) and WindowsSolutionUniversal (Windows 8.1 and Windows Phone 8.1 Universal). Copy the appropriate one across to any new project and always add to the root of the client repo. This contains the Windows Store and Windows Phone apps.

You can then subsequently build out from Unity using the Tools > MarkerMetro > Build menu or specific the path manually via a standard File > Build Settings build. 

Note that currently these projects are for Windows 8.1 and Windows Phone 8.0

# Guidance

Provided here is guidance for working with WinShared based projects which you should read and understand but are not directly related to setting up a new project.

## Marker Metro Plugin Integration

This repository includes a stable version of all dependent Marker Metro plugins. You can easily update to later versions, as well as use and debug local versions of WinLegacy and WinIntegration.

### Updating plugins

Use the MarkerMetro > Plugins > Update menu operation to ensure you have the very latest stable plugins within Unity. Plugins are pulled automatically from NuGet. By default, WinLegacy, WinIntegration and LitJson are included.

Note: The /BuildScripts/Update_NuGet_Packages.proj file can be updated directly should you will to include additional plugins from Nuget.

Once you have done this, be sure and push the updates to the dependencies.

### Using local versions of WinLegacy and WinIntegration

Perhaps you are using a fork or want to debug into the plugins?

Using the MarkerMetro > Plugins > Configure menu operation you can switch to using Local as the Plugin Source. This will allow you to specify directories for WinLegacy and WinIntegration. You can use Debug or Release configuration to specify the build configation.

### Debugging local versions of WinLegacy and WinIntegration

You will need to build out again from Unity and include the platform specific plugin project into your solution before setting breakpoints. (TO DO - Use an image and explain this better)

## Enabling Windows Features in your project

WinShared offers a window in Unity to enable/disable some of the commonly used Windows features. Things like having the sound options in the charm settings or displaying the IAP disclaimer when opening the game for the first time.

These features can be set in the _Features_ window under the _MarkerMetro_ menu on Unity.

These settings are stored in a file called `WinSharedSettings.xml`, so you must add this file to your version control system in order to have everybody in your project using the same settings.

It's also important to notice that `WinSharedSettings.xml` is embedded to the Unity build, to be read by the Windows projects. Therefore, upon changing a setting you must build on Unity in order for it to take effect.

By default, _all_ features are enabled.

## Win Shared FaceFlip Scene Features (TBC)

### Exception Logging

#### Testing exceptions/crashes

- **Unity** project has extra button in `UIStart.cs` in /Assets/WinIntegrationExample/FaceFlip.unity test scene in WinShared.
- **Windows Store** project has extra Settings charm menu item **Crash** (only for Debug)
- **Windows Phone** project has AppBar to allow crash testing (only for Debug)

## Windows Phone Low Memory Optimization

There is a script that tries to optimize assets settings to lower memory usage, which is useful specially for Windows Phone 8.
You can find it at `\Assets\Editor\MarkerMetro\MemoryOptimizer.cs`.
Please refer to the code documentation for instructions on how to use it.

## App Name Localization

Windows Store app manifest uses AppName and AppDescription resources to localize the store name, application name and description.

Windows Phone manifest uses a build-process generated AppResLib.dll[*.mui] files that pull strings from resources automatically:
- ApplicationTitle is used to generate resource string 100, wich is used to Application Display Name
- ApplicationTileTitle is used to generate resource string 101, wich is used to set Tile Title in manifest
- ApplicationDescription is used to generate resource string 102, which is used to set Application Description

If any of these strings are missing from resources, AppResLibGenerator will report a warning.

AppResLibGenerator is referenced as [Nuget Package](https://www.nuget.org/packages/MarkerMetro.WindowsPhone.AppResLibGenerator/) and is also on [Github](https://github.com/MarkerMetro/AppResLibGenerator)
