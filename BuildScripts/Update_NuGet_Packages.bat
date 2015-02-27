@echo off

SET unityPluginDir=%1
if "%1" == "" (
    SET unityPluginDir="..\Assets\Plugins\"
    )
SET vsCommonToolsDir=%2
IF "%2" == "" (
    SET vsCommonToolsDir="C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\Tools\"
    )

SET winLegacy=MarkerMetro.Unity.WinLegacy
SET winIntegration=MarkerMetro.Unity.WinIntegration

echo "%unityPluginDir%\Metro\%winLegacy%.pdb"

if exist "%unityPluginDir%\Metro\%winLegacy%.pdb" (
    del "%unityPluginDir%\Metro\%winLegacy%.pdb"
    del "%unityPluginDir%\Metro\%winLegacy%.pdb.meta"
)
if exist "%unityPluginDir%\%winLegacy%.pdb" (
    del "%unityPluginDir%\%winLegacy%.pdb"
    del "%unityPluginDir%\%winLegacy%.pdb.meta"
)
if exist "%unityPluginDir%\WP8\%winLegacy%.pdb" (
    del "%unityPluginDir%\WP8\%winLegacy%.pdb"
    del "%unityPluginDir%\WP8\%winLegacy%.pdb.meta"
)

if exist "%unityPluginDir%\Metro\%winIntegration%.pdb" (
    del "%unityPluginDir%\Metro\%winIntegration%.pdb"
    del "%unityPluginDir%\Metro\%winIntegration%.pdb.meta"
)
if exist "%unityPluginDir%\%winIntegration%.pdb" (
    del "%unityPluginDir%\%winIntegration%.pdb"
    del "%unityPluginDir%\%winIntegration%.pdb.meta"
)
if exist "%unityPluginDir%\WP8\%winIntegration%.pdb" (
    del "%unityPluginDir%\WP8\%winIntegration%.pdb"
    del "%unityPluginDir%\WP8\%winIntegration%.pdb.meta"
)


call %vsCommonToolsDir%\vsvars32.bat
echo progress 0
msbuild Update_NuGet_Packages.proj
echo progress 100