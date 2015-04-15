Getting Started
====================

WinShared is both a starting framework and sample code for Unity ports to Windows.

The repository itself is a Unity project with useful plugins, scripts, tools and a sample game. The repository contains Windows Solutions which enhance and extend the base solutions outputted from Unity specifically for Windows.

You can view an overview of the architecture here:
https://www.dropbox.com/s/w8wt0au5602vl57/MarkerMetro.Unity.WinShared.jpg

Prerequisites
==================

- Visual Studio 2013 with latest updates
- Nuget Package Manager
- Unity 5.0.0p2

Using WinShared on an existing porting project
============================================

Skip this step if you just want to take a look at WinShared.

Follow the steps in this section To initialize an exiting Unity project you may be working on using WinShared. 

This will include al the functionality of WinShared and related plugins as detailed in this read me, making your porting efforts hopefully a lot easier.

To do this, you will execeute the Init.ps1 PowerShell script, located in root of this repo.

Note: Please ignore this section if you have already installed via Unity Asset Store.

You will need to provide following parameters:

- TargetRepoPath - full path to root folder of new repository (for example: `C:\Code\SomeProject\`)
- UnityProjectTargetDir - subdirectory under _TargetRepoPath_ that contains Unity project (can be empty, if it is in the root of repo, for example: `Unity` for Unity project to be in `C:\Code\SomeProject\Unity`)
- ProjectName - name of new project (ensure same as ProductName in PlayerSettings). Script will rename Windows projects, namespaces and solutions to match this name.
- IncludeExamples : optional. Boolean to indicate whether to include the example scene and game from Marker Metro to demonstrate WinIntegration features. Defaults to false.

This script will always copy the following files and folders to your target project:

* .gitignore - Up to date git ignore for Unity projects (you should manually merge this if there is an existing one)
* /BuildScripts/* - helper build scripts
* /Assets/Plugins/* - plugin binaries and scripts
* /Assets/MarkerMetro/Editor/* - helper editor scripts, including Tools > MarkerMetro menu options
* /WindowsSolutionUniversal - boilerplate windows 8.1 Universal solution folder, enhanced version of what Unity outputs

If 'IncludeExamples' is true the following will be copied across:

* /Assets/MarkerMetro/Example/* - see FaceFlip.unity, a small optional game scene with [WinIntegration](https://github.com/MarkerMetro/MarkerMetro.Unity.WinIntegration) test points (recommended to see WinIntegration features in action)
* /Assets/StreamingAssets/MarkerMetro/* - supporting example video for WinIntegration tests

Guidance
==================

Provided here is guidance for working with WinShared (and projects based on WinShared) which you should read and understand but are not directly related to setting up a new project.

## Windows Solution Build Output

The WindowsSolutionUniversal folder contains the Windows Store and Windows Phone apps (Windows 8.1 and Windows Phone 8.1 Universal).

### Building to Windows

You must build out from Unity for the Windows Solutions to work. You can do this using the Tools > MarkerMetro > Build menu or specify the path manually via a standard File > Build Settings build.

Ensure that you build out to the correct output folders as follows:

- Windows 8.1 Universal > /WindowsSolutionUniversal

### Game Settings

Some game settings need to be provided via the application side such as music, sound and notifications enabled status via the Windows 8 charms bar. 

For example in the [Windows Store Settings Charm Control] (https://github.com/MarkerMetro/MarkerMetro.Unity.WinShared/blob/master/WindowsSolutionUniversal/UnityProject/UnityProject.Windows/GameSettingsFlyout.xaml.cs) you will see code similar to the following:

```csharp
musicSwitch.IsOn = GameController.Instance.GameSettings.MusicEnabled;
```

The GameController class is an intermediary class between the Application and Unity. Make sure that you initialize the Game Settings on the game side with a class which implements [IGameSettings] (https://github.com/MarkerMetro/MarkerMetro.Unity.WinShared/blob/master/Assets/Plugins/MarkerMetro/IGameSettings.cs)

```csharp
GameController.Instance.Init(IGameSettings gameSettings);
```
Within the example game in WinShared this is done with the main GameMaster.cs class in the Awake method, [see here] (https://github.com/MarkerMetro/MarkerMetro.Unity.WinShared/blob/master/Assets/MarkerMetro/Example/Scripts/GameMaster.cs).

### Application Configuration

Application configuration is provided via the /Config/AppConfig.cs class. You are able to turn various features on and off as well as supplying facebook and exception logging api keys for example.

Note that this class implemented IGameConfig and is supplied to Unity game side as part of app initialization within [MainPage.xaml.cs] (https://github.com/MarkerMetro/MarkerMetro.Unity.WinShared/blob/master/WindowsSolutionUniversal/UnityProject/UnityProject.Shared/MainPage.xaml.cs). This way you can have configuration which works across both the app and game. 

```csharp
MarkerMetro.Unity.WinShared.GameController.Instance.Init(AppConfig.Instance);
```

### Managing Exception Logging

See [WinIntegration Exception Logging](https://github.com/MarkerMetro/MarkerMetro.Unity.WinIntegration/blob/master/README.md#exception-logging) for more on exception logging.

You can replace [RaygunExceptionLogger](https://github.com/MarkerMetro/MarkerMetro.Unity.WinShared/blob/master/WindowsSolutionUniversal/UnityProject/UnityProject.Shared/Logging/RaygunExceptionLogger.cs) with an alternative implementation of IExceptionLogger for your crash reporting needs. Ensure that your projects have a reference to the crash reporting solution you are using and wire up your IExceptionLogger implementation to that solution. Lastly, [modify AppConfig.cs](https://github.com/MarkerMetro/MarkerMetro.Unity.WinShared/blob/master/WindowsSolutionUniversal/UnityProject/UnityProject.Shared/Config/AppConfig.cs) to assign your api key as required.

If you are not using exception logging at all you can remove Raygun as follows:

- Remove Raygun Nuget packages from the solution. Right click the solution and select "Manage Nuget Packages for Solution" and uncheck Raygun. 
- Remove the RaygunExceptionLogger.cs class
- Change the ExceptionLogger.Initialize line in App.xaml.cs to ExceptionLogger.Initialize(null);
- [modify AppConfig.cs](https://github.com/MarkerMetro/MarkerMetro.Unity.WinShared/blob/master/WindowsSolutionUniversal/UnityProject/UnityProject.Shared/Config/AppConfig.cs)  to set the ExceptionLoggingEnabled property to return false to completely disable exception logging. 

### QA and Master configurations

The Windows Solutions have QA and Master solution configurations. QA has been copied from Master to provide a production ready configuration that can support testing. 

At this point the differences are:

- Store Integration - QA uses simulated IAP, Master will attempt to use real production store APIs.
- Exception Logging - QA enables ability to crash test via Windows settings charm, but will be off by default. Master will hide this capability but ensure Exception Logging is on by default (if a key has been supplied)
- GameController.Instance.GameConfig.CurrentBuildConfig in Unity will return Debug/QA/Master configuraiton at runtime (based on app solution configuration) which can be useful to apply environment specific login within the game side.

## Marker Metro Tools Menu

Tools > MarkerMetro provides some useful features for Unity developers porting to Windows.

### Build Menu

Alows you to quickly build to  Windows Universal.

### Plugins Menu

This repository includes a stable version of all dependent Marker Metro plugins. You can easily update to later versions, as well as use and debug local versions of WinLegacy and WinIntegration.

Plugins > Update allows you to update the Marker Metro plugins (WinIntegration and WinLegacy) to the latest version. By default this option gets latest stable versions via the main public NuGet field, but you can also configure to use local solution folders if you are using your own fork for example.

Building the plugins locally allows you also to easily debug a particular Windows Store or Windows Phone plugin project as follows:

- Add the platform specific plugin project to your Windows solution (e.g. MarkerMetro.Unity.WinIntegrationMetro)
- Tools > MarkerMetro > Plugins > Configure ( ensure Plugin Source is Local, and Build Local is Debug)
- Tools > MarkerMetro > Plugins > Update
- Tools > MarkerMetro > Build >
- Set breakpoints in your platform specific plugin project and then F5 on your app

## Sample Game and [WinIntegration](https://github.com/MarkerMetro/MarkerMetro.Unity.WinIntegration) Samples

The FaceFlip.unity scene demonstrates some key [WinIntegration](https://github.com/MarkerMetro/MarkerMetro.Unity.WinIntegration) features around a simple game such as:

- [Facebook Integration](https://github.com/MarkerMetro/MarkerMetro.Unity.WinIntegration/blob/master/README.md#facebook-integration)
- [Store Integration](https://github.com/MarkerMetro/MarkerMetro.Unity.WinIntegration/blob/master/README.md#store-integration)
- [Exception Logging](https://github.com/MarkerMetro/MarkerMetro.Unity.WinIntegration/blob/master/README.md#exception-logging) (Crash and Exception Testing)
- [Local Notifications](https://github.com/MarkerMetro/MarkerMetro.Unity.WinIntegration/blob/master/README.md#local-notifications)
- [Video Player](https://github.com/MarkerMetro/MarkerMetro.Unity.WinIntegration/blob/master/README.md#video-player)
- [Platform specific information](https://github.com/MarkerMetro/MarkerMetro.Unity.WinIntegration/blob/master/README.md#helper)


## Windows Phone Low Memory Optimization

There is a [memory optimization script](https://github.com/MarkerMetro/MarkerMetro.Unity.WinShared/blob/master/Assets/MarkerMetro/Editor/MemoryOptimizer.cs) that optimize texture settings to lower memory usage, which is useful specially for Windows Phone.

The script lowers the maximum size that a texture can have, the default is to use half of the resolution that is currently being used but it can be configured to be more aggressive. Note that if you run the script multiple times it will lower the resolution even more, but if that's your intention you should play around with the `TextureResizingFactor` variable in the script. The script also forces the textures to use DXT compression, which is the best option for Windows.

You can use the menu item _Tools > Marker Metro > Memory Optimization > Optimize Texture Settings_ to run it. It will locate all texture assets in the project and optimize their settings. You can also create an ignore list of files that you don't want to change; for this, create a CSV file called `WindowsAssetIgnoreList.csv` in the root of your Unity project with the paths of the files you want the script to ignore (e.g. `Assets/Resources/ignoreme.jpg`). 

Texture settings are stored in their corresponding meta files, therefore this script only changes a texture's meta files. Since Windows 8.1 and Windows Phone 8.1 use the same textures for a Universal build, we cannot change these settings for Windows Phone only. Therefore, you will need to essentially run a secnd low memory build after running this script specifically for Windows Phone.

## App Name Localization

Windows 8.1 and Windows Phone 8.1 app manifest uses AppName and AppDescription resources to localize the store name, application name and description.
