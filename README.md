Getting Started
====================

This repo contain shared code that shouldn't be placed in a plugin.

Assets
======

Ensure you add the subfolders and their contents to your Unity project. 


WindowsSolution/Common
====================
Shared code between Windows 8 and Windows Phone 8 that has to be in the app project. 

Add the Common folder and its contents to your WindowsSolution folder (the folder holding your Windows and WP8 projects) and then add a linked file to the CommonMainPage (and any other files in the future) on the root of your projects so code is included correctly. 

Make sure the namespace for CommonMainPage.cs matches that of the MainPage.xaml.cs for the project.

Make sure to add the XAML for the TextBlock control to MainPage.xaml, this will show the memory stats.
For this XAML refer to a previous project such as Checkout Challenge or Lost Light.

## Live Tiles

*CommonMainPage.cs* offers an implementation of Live Tiles for medium and wide tiles, all you have to do is implement the *UpdateLiveTiles* method, according  to the code comments.

AppResLib
====================

This project is used to create localized titles and tile titles in Windows Phone 8.
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

**WARNING**: the article states that you should move the `AppResLib.dll.*.mui` files into the Resources folder (step 7). Don't do it!
