@echo off

SET winLegacyDir=%1
SET winIntegrationDir=%2
SET buildConfig=%3
IF "%1" == "" (
    SET buildConfig=Release
    )



call "C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\Tools\vsvars32.bat"
msbuild %winLegacyDir%\MarkerMetro.Unity.WinLegacyMetro\MarkerMetro.Unity.WinLegacyMetro.csproj /p:Configuration=%buildConfig% /t:Clean;Build
msbuild %winLegacyDir%\MarkerMetro.Unity.WinLegacyUnity\MarkerMetro.Unity.WinLegacyUnity.csproj /p:Configuration=%buildConfig% /t:Clean;Build
msbuild %winLegacyDir%\MarkerMetro.Unity.WinLegacyWP8\MarkerMetro.Unity.WinLegacyWP8.csproj /p:Configuration=%buildConfig% /t:Clean;Build

msbuild %winIntegrationDir%\MarkerMetro.Unity.WinIntegrationMetro\MarkerMetro.Unity.WinIntegrationMetro.csproj /p:Configuration=%buildConfig% /t:Clean;Build
msbuild %winIntegrationDir%\MarkerMetro.Unity.WinIntegrationUnity\MarkerMetro.Unity.WinIntegrationUnity.csproj /p:Configuration=%buildConfig% /t:Clean;Build
msbuild %winIntegrationDir%\MarkerMetro.Unity.WinIntegrationWP8\MarkerMetro.Unity.WinIntegrationWP8.csproj /p:Configuration=%buildConfig% /t:Clean;Build
