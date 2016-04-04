@echo off

SET winLegacyDir=%1
SET winIntegrationDir=%2
SET unityPluginDir=%3
SET buildConfig=%4
SET vsCommonToolsDir=%5%
IF "%1" == "" (
    SET buildConfig=Release
    )
IF "%5" == "" (
    SET vsCommonToolsDir="C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\Tools"
    )
SET winLegacy=MarkerMetro.Unity.WinLegacy
SET winIntegration=MarkerMetro.Unity.WinIntegration
echo progress 0

nuget restore "%winIntegrationDir%\%winIntegration%.sln%"
echo progress 12

call %vsCommonToolsDir%\vsvars32.bat

msbuild "%winLegacyDir%\%winLegacy%Metro\%winLegacy%Metro.csproj" /p:Configuration=%buildConfig% /t:Clean;Build
echo progress 24
msbuild "%winLegacyDir%\%winLegacy%Unity\%winLegacy%Unity.csproj" /p:Configuration=%buildConfig% /t:Clean;Build
echo progress 36

msbuild "%winIntegrationDir%\%winIntegration%Metro\%winIntegration%Metro.csproj" /p:Configuration=%buildConfig% /t:Clean;Build
echo progress 48
msbuild "%winIntegrationDir%\%winIntegration%Unity\%winIntegration%Unity.csproj" /p:Configuration=%buildConfig% /t:Clean;Build
echo progress 60
msbuild "%winIntegrationDir%\%winIntegration%WP81\%winIntegration%WP81.csproj" /p:Configuration=%buildConfig% /t:Clean;Build
echo progress 72
msbuild "%winIntegrationDir%\%winIntegration%UWP\%winIntegration%UWP.csproj" /p:Configuration=%buildConfig% /t:Clean;Build
echo progress 84

copy /y "%winLegacyDir%\%winLegacy%Metro\bin\%buildConfig%\%winLegacy%.dll" "%unityPluginDir%\Metro\WindowsPhone81\%winLegacy%.dll"
copy /y "%winLegacyDir%\%winLegacy%Metro\bin\%buildConfig%\%winLegacy%.dll" "%unityPluginDir%\Metro\WSA8.1\%winLegacy%.dll"
copy /y "%winLegacyDir%\%winLegacy%Metro\bin\%buildConfig%\%winLegacy%.dll" "%unityPluginDir%\Metro\WSA10\%winLegacy%.dll"
copy /y "%winLegacyDir%\%winLegacy%Unity\bin\%buildConfig%\%winLegacy%.dll" "%unityPluginDir%\%winLegacy%.dll"
echo progress 88

copy /y "%winLegacyDir%\%winLegacy%Metro\bin\%buildConfig%\%winLegacy%.pdb" "%unityPluginDir%\Metro\WindowsPhone81\%winLegacy%.pdb"
copy /y "%winLegacyDir%\%winLegacy%Metro\bin\%buildConfig%\%winLegacy%.pdb" "%unityPluginDir%\Metro\WSA8.1\%winLegacy%.pdb"
copy /y "%winLegacyDir%\%winLegacy%Metro\bin\%buildConfig%\%winLegacy%.pdb" "%unityPluginDir%\Metro\WSA10\%winLegacy%.pdb"
copy /y "%winLegacyDir%\%winLegacy%Unity\bin\%buildConfig%\%winLegacy%.pdb" "%unityPluginDir%\%winLegacy%.pdb"
echo progress 92

copy /y "%winIntegrationDir%\%winIntegration%WP81\bin\%buildConfig%\%winIntegration%.dll" "%unityPluginDir%\Metro\WindowsPhone81\%winIntegration%.dll"
copy /y "%winIntegrationDir%\%winIntegration%Metro\bin\%buildConfig%\%winIntegration%.dll" "%unityPluginDir%\Metro\WSA8.1\%winIntegration%.dll"
copy /y "%winIntegrationDir%\%winIntegration%UWP\bin\%buildConfig%\%winIntegration%.dll" "%unityPluginDir%\Metro\WSA10\%winIntegration%.dll"
copy /y "%winIntegrationDir%\%winIntegration%Unity\bin\%buildConfig%\%winIntegration%.dll" "%unityPluginDir%\%winIntegration%.dll"
echo progress 96

copy /y "%winIntegrationDir%\%winIntegration%WP81\bin\%buildConfig%\%winIntegration%.pdb" "%unityPluginDir%\Metro\WindowsPhone81\%winIntegration%.pdb"
copy /y "%winIntegrationDir%\%winIntegration%Metro\bin\%buildConfig%\%winIntegration%.pdb" "%unityPluginDir%\Metro\WSA8.1\%winIntegration%.pdb"
copy /y "%winIntegrationDir%\%winIntegration%UWP\bin\%buildConfig%\%winIntegration%.pdb" "%unityPluginDir%\Metro\WSA10\%winIntegration%.pdb"
copy /y "%winIntegrationDir%\%winIntegration%Unity\bin\%buildConfig%\%winIntegration%.pdb" "%unityPluginDir%\%winIntegration%.pdb"
echo progress 100