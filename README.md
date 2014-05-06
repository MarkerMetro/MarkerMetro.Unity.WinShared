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



AppResLib
====================

This project is used to create localized titles and tile titles in Windows Phone 8.
These strings are going to be read from a dll , and there's a dll for each language.
In order to create these dlls, follow these steps:

 - Open the AppResLib solution and project;
 - Open the `String Table`;
 - Change the `Caption` cells to the values that you want for some specific language;
 - Build the project (use `Release` configuration!), the generated dll can be found under `/WinShared/AppResLib/Release/`.
 - For the English values dll, rename it to `AppResLibLangNeutral.dll`, for other languages rename the dll to the name defined in the table found here:
 
http://msdn.microsoft.com/en-us/library/windowsphone/develop/ff967550%28v=vs.105%29.aspx

So you'll end up with one dll for each language other than English renamed according to the table, and one `AppResLibLangNeutral.dll`.

To use the dlls, follow the instructions in the article, section *"To use the localized resource strings in your Windows Phone app"*.

**Attention**: the article states that you should move the `AppResLib.dll.*.mui` files into the Resources folder (step 7). Don't do it!
