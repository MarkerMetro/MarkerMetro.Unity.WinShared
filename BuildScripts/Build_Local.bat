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
    SET vsCommonToolsDir="C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\Tools"
    )
SET winLegacy=MarkerMetro.Unity.WinLegacy
SET winIntegration=MarkerMetro.Unity.WinIntegration
echo progress 0

nuget restore "%winIntegrationDir%\%winIntegration%.sln%"
echo progress 15

call %vsCommonToolsDir%\vsvars32.bat

msbuild "%winLegacyDir%\%winLegacy%Metro\%winLegacy%Metro.csproj" /p:Configuration=%buildConfig% /t:Clean;Build
echo progress 30
msbuild "%winLegacyDir%\%winLegacy%Unity\%winLegacy%Unity.csproj" /p:Configuration=%buildConfig% /t:Clean;Build
echo progress 45

msbuild "%winIntegrationDir%\%winIntegration%Metro\%winIntegration%Metro.csproj" /p:Configuration=%buildConfig% /t:Clean;Build
echo progress 60
msbuild "%winIntegrationDir%\%winIntegration%Unity\%winIntegration%Unity.csproj" /p:Configuration=%buildConfig% /t:Clean;Build
echo progress 75
msbuild "%winIntegrationDir%\%winIntegration%WP81\%winIntegration%WP81.csproj" /p:Configuration=%buildConfig% /t:Clean;Build
echo progress 100

copy /y "%winLegacyDir%\%winLegacy%Metro\bin\%buildConfig%\%winLegacy%.dll" "%unityPluginDir%\Metro\%winLegacy%.dll"
copy /y "%winLegacyDir%\%winLegacy%Metro\bin\%buildConfig%\%winLegacy%.dll" "%unityPluginDir%\Metro\WindowsPhone81\%winLegacy%.dll"
copy /y "%winLegacyDir%\%winLegacy%Unity\bin\%buildConfig%\%winLegacy%.dll" "%unityPluginDir%\%winLegacy%.dll"

copy /y "%winLegacyDir%\%winLegacy%Metro\bin\%buildConfig%\%winLegacy%.pdb" "%unityPluginDir%\Metro\%winLegacy%.pdb"
copy /y "%winLegacyDir%\%winLegacy%Metro\bin\%buildConfig%\%winLegacy%.pdb" "%unityPluginDir%\Metro\WindowsPhone81\%winLegacy%.pdb"
copy /y "%winLegacyDir%\%winLegacy%Unity\bin\%buildConfig%\%winLegacy%.pdb" "%unityPluginDir%\%winLegacy%.pdb"

copy /y "%winIntegrationDir%\%winIntegration%Metro\bin\%buildConfig%\%winIntegration%.dll" "%unityPluginDir%\Metro\%winIntegration%.dll"
copy /y "%winIntegrationDir%\%winIntegration%Unity\bin\%buildConfig%\%winIntegration%.dll" "%unityPluginDir%\%winIntegration%.dll"
copy /y "%winIntegrationDir%\%winIntegration%WP81\bin\%buildConfig%\%winIntegration%.dll" "%unityPluginDir%\Metro\WindowsPhone81\%winIntegration%.dll"

copy /y "%winIntegrationDir%\%winIntegration%Metro\bin\%buildConfig%\%winIntegration%.pdb" "%unityPluginDir%\Metro\%winIntegration%.pdb"
copy /y "%winIntegrationDir%\%winIntegration%Unity\bin\%buildConfig%\%winIntegration%.pdb" "%unityPluginDir%\%winIntegration%.pdb"
copy /y "%winIntegrationDir%\%winIntegration%WP81\bin\%buildConfig%\%winIntegration%.pdb" "%unityPluginDir%\Metro\WindowsPhone81\%winIntegration%.pdb"
