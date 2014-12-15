@echo off

SET winLegacyDir=%1
SET winIntegrationDir=%2
SET unityPluginDir=%3
SET buildConfig=%4
IF "%1" == "" (
    SET buildConfig=Release
    )
SET winLegacy=MarkerMetro.Unity.WinLegacy
SET winIntegration=MarkerMetro.Unity.WinIntegration

call "C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\Tools\vsvars32.bat"
echo "%winLegacyDir%\%winLegacy%Metro\%winLegacy%Metro.csproj"
msbuild "%winLegacyDir%\%winLegacy%Metro\%winLegacy%Metro.csproj" /p:Configuration=%buildConfig% /t:Clean;Build
msbuild "%winLegacyDir%\%winLegacy%Unity\%winLegacy%Unity.csproj" /p:Configuration=%buildConfig% /t:Clean;Build
msbuild "%winLegacyDir%\%winLegacy%WP8\%winLegacy%WP8.csproj" /p:Configuration=%buildConfig% /t:Clean;Build

msbuild "%winIntegrationDir%\%winIntegration%Metro\%winIntegration%Metro.csproj" /p:Configuration=%buildConfig% /t:Clean;Build
msbuild "%winIntegrationDir%\%winIntegration%Unity\%winIntegration%Unity.csproj" /p:Configuration=%buildConfig% /t:Clean;Build
msbuild "%winIntegrationDir%\%winIntegration%WP8\%winIntegration%WP8.csproj" /p:Configuration=%buildConfig% /t:Clean;Build

xcopy /y "%winLegacyDir%\%winLegacy%Metro\bin\%buildConfig%\%winLegacy%.dll" "%unityPluginDir%\Metro\%winLegacy%.dll"
xcopy /y "%winLegacyDir%\%winLegacy%Unity\bin\%buildConfig%\%winLegacy%.dll" "%unityPluginDir%\%winLegacy%.dll"
xcopy /y "%winLegacyDir%\%winLegacy%WP8\bin\%buildConfig%\%winLegacy%.dll" "%unityPluginDir%\WP8\%winLegacy%.dll"

xcopy /y "%winLegacyDir%\%winLegacy%Metro\bin\%buildConfig%\%winLegacy%.pdb" "%unityPluginDir%\Metro\%winLegacy%.pdb"
xcopy /y "%winLegacyDir%\%winLegacy%Unity\bin\%buildConfig%\%winLegacy%.pdb" "%unityPluginDir%\%winLegacy%.pdb"
xcopy /y "%winLegacyDir%\%winLegacy%WP8\bin\%buildConfig%\%winLegacy%.pdb" "%unityPluginDir%\WP8\%winLegacy%.pdb"

xcopy /y "%winIntegrationDir%\%winIntegration%Metro\bin\%buildConfig%\%winIntegration%.dll" "%unityPluginDir%\Metro\%winIntegration%.dll"
xcopy /y "%winIntegrationDir%\%winIntegration%Unity\bin\%buildConfig%\%winIntegration%.dll" "%unityPluginDir%\%winIntegration%.dll"
xcopy /y "%winIntegrationDir%\%winIntegration%WP8\bin\%buildConfig%\%winIntegration%.dll" "%unityPluginDir%\WP8\%winIntegration%.dll"

xcopy /y "%winIntegrationDir%\%winIntegration%Metro\bin\%buildConfig%\%winIntegration%.pdb" "%unityPluginDir%\Metro\%winIntegration%.pdb"
xcopy /y "%winIntegrationDir%\%winIntegration%Unity\bin\%buildConfig%\%winIntegration%.pdb" "%unityPluginDir%\%winIntegration%.pdb"
xcopy /y "%winIntegrationDir%\%winIntegration%WP8\bin\%buildConfig%\%winIntegration%.pdb" "%unityPluginDir%\WP8\%winIntegration%.pdb"