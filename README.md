Getting Started
====================

Starting framework code for Unity ports to Windows.

You can view an overview of the architecture here:
https://www.dropbox.com/s/w8wt0au5602vl57/MarkerMetro.Unity.WinShared.jpg

Configured and tested with: Unity 4.5f6, and Unity 4.5.1f3

The approach when starting a new porting project is to simply copy and paste the necessary folders across to the client repo as detailed below:


Build Server Setup
==================
It's important to copy the build steps and tweak for your project as required, see the steps here:

http://mmbuild2.markermetro.com:9091/admin/editBuildRunners.html?id=buildType:MarkerMetroUnityWinShared_CI
http://mmbuild2.markermetro.com:9091/admin/editBuildRunners.html?id=buildType:MarkerMetroUnityWinShared_Release


Unity Project
=======

The root of the repo is a small Unity project for testing purposes.

Copy and paste the following /Assets subfolders and files and the .gitignore to the existing Assets folder in client repo's Unity project (normally on root but could be in a sub folder)
Assets/MetroTestCertificate.pfx
Assets/MetroTestCertificate.pfx.meta
Assets/Editor/*
Assets/Plugins/*

The code here helps with automated builds and includes various helper files.

You can build out this project to the WindowsSolution folders as below, note that you should ensure the ProductName in PlayerSettings is "UnityProject" so that everything just works.

WindowsSolution
====================
This is the base WindowsSolution folder to be used for all Unity Projects. Copy it across to any new project and add to the root of the client repo. This contains all the goodness from working on previous projects at the app level.

Run the Init.ps1 script on the root to rename UnityProject to your product name (e.g LostLight). 

Then subsequently build out from Unity to /WindowsSolution/WindowsStore and /WindowsSolution/WindowsPhone.

Note that visual assets are from the Disney BOLA game so you know exactly which ones to replace.

NuGet Plugins
=====================================================================
This is the Nuget folder allowing for easy plugin integration to your Unity project. Copy it across to any new project and add to the root of the client repo's Unity project (normally on root but could be in a sub folder)

By default core Marker Metro plugins are included (WinLegacy, WinIntegration and LitJson), but the .csproj file can be edited to maintain plugin list.

To add/update the plugins you can run the following: \NuGet\Update_NuGet_Packages.bat (ensuring you have set up NuGet Access).

Once you have done this, be sure and push the updates to the dependencies.

If you need to work on any of the dependencies, you will need to open the project from Marker Metro Github and push any changes.

Once you have made the changes, you can manually run a build on the build server (See Automated Builds below)

Once the build has been run, you can then run the bat file above to include the latest binaries.

First Time Marker Metro NuGet Access
=========================

Use  Marker Metro's private [NuGet](http://docs.nuget.org/docs/start-here/installing-nuget) feed: 
[http://mmbuild1.cloudapp.net/httpAuth/app/nuget/v1/FeedService.svc/](http://mmbuild1.cloudapp.net/httpAuth/app/nuget/v1/FeedService.svc/)
If you don't have personal account you can always use Disney's guest account:

*Username*: Disney

*Password*: Disney40cks

This project repository incudes a NuGet folder in the root with *nuget.exe* and it can be used to setup sources and store passwords. To add Marker Metro's Private Feed and remember authentication you can use following command-line:

**./NuGet.exe sources add -Name "Marker Metro Private" -Source "http://mmbuild1.cloudapp.net/httpAuth/app/nuget/v1/FeedService.svc/" -UserName disney -Password Disney40cks**

You can also modify previously added feed using update command:

**./NuGet.exe sources update -Name "Marker Metro Private" -UserName disney -Password Disney40cks**

To list existing sources you can use:

**./NuGet.exe sources**


AppResLib
====================

This solution is used to create localized titles and tile titles in Windows Phone 8.

Copy it across to any new project and add to the root of the client repo.

These strings are going to be read from a dll , and there's a dll for each language.


In order to create these dlls, follow these steps:

 - Open the AppResLib solution and project;
 - Expand the 'Resource Files' folder under the project
 - Double click on 'AppResLib.rc'
 - The Resource View pane will open, expand the 'String Table' folder
 - Double-click on the 'String Table' item in the folder;
 - Change the 'ID' and 'Caption' cells to the values for the language you wish to the build the DLL for. 
 - Make sure the 'Value' column has a unique number, as this is how the key will be referenced.
 - You only need to specify localisation keys for text displayed on live tiles, such as the app name and high score etc. 
 - Build the project (use 'Release' configuration!), the generated dll 'AppResLib.dll' can be found under '/WinShared/AppResLib/Release/'.  (If it is not there make sure you are looking at the correct Release folder, as there are two that are generated)
 - For the English values dll, rename it to 'AppResLibLangNeutral.dll', for other languages rename the dll to the name defined in the table found here:
 
http://msdn.microsoft.com/en-us/library/windowsphone/develop/ff967550%28v=vs.105%29.aspx

So you'll end up with one dll for each language other than English renamed according to the table, and one 'AppResLibLangNeutral.dll'.

To use the dlls, follow the instructions in the article, section *"To use the localized resource strings in your Windows Phone app"*.

Don't forget to add the AppResLib solution folder to your project's repository. It should sit in the Windows Phone project folder, an example can be found here:
https://github.wdig.com/MarkerMetro/StarWarsCCGWindows/tree/master/StarWars.WinPhone

**WARNING**: the article states that you should move the `AppResLib.dll.*.mui` files into the Resources folder (step 7). Don't do it!

**WARNING**: In order for localisation changes to be displayed correctly in the store, the manifest must have the appropriate culture codes defined for each *.mui file.  These codes must have a language and a region set, for example "ru-RU" (russian for russia).  Failing this step will mean that the title in the store will not be substituted as expected.

**WARNING**: Temporarily renaming the *.mui files to have a dll extension allows them to be modified within the windows solution, however this can cause them to lose their content flag.  Make sure that in the file properties they are set as 'content', not 'none'.

Memory Optimization
====================

There is a script that tries to optimize assets settings to lower memory usage, which is useful specially for Windows Phone 8.
You can find it at `\Assets\Editor\MarkerMetro\MemoryOptimizer.cs`.
Please refer to the code documentation for instructions on how to use it.
