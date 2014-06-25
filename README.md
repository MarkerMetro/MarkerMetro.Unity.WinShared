Getting Started
====================

This repo contains useful starting framework and code for all Unity projects.

UnityProject
============

This is a sample Unity Project which contains useful code for each new Unity Project. 

Ensure you add the subfolders and their contents to your Unity project on project commencement

WindowsSolution
====================
This is the base WindowsSolution folder to be used for all Unity Projects. Copy it across to any new project and add to the root of the client repo.

Run the Init script on the root to rename UnityProject to your product name (e.g LostLight). 

Then build out from Unity to /WindowsSolution/WindowsStore and /WindowsSolution/WindowsPhone.

AppResLib
====================

This solution is used to create localized titles and tile titles in Windows Phone 8.
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
