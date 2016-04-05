@echo off

SET unityPluginDir=%1
if "%1" == "" (
    SET unityPluginDir="..\Assets\Plugins\"
    )
SET vsCommonToolsDir=%2
IF "%2" == "" (
    SET vsCommonToolsDir="C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\Tools\"
    )

SET winLegacy=MarkerMetro.Unity.WinLegacy
SET winIntegration=MarkerMetro.Unity.WinIntegration

if exist "%unityPluginDir%\Metro\WSA8.1\%winLegacy%.pdb" (
    del "%unityPluginDir%\Metro\WSA8.1\%winLegacy%.pdb"
    del "%unityPluginDir%\Metro\WSA8.1\%winLegacy%.pdb.meta"
)
if exist "%unityPluginDir%\Metro\WSA10\%winLegacy%.pdb" (
    del "%unityPluginDir%\Metro\WSA10\%winLegacy%.pdb"
    del "%unityPluginDir%\Metro\WSA10\%winLegacy%.pdb.meta"
)
if exist "%unityPluginDir%\Metro\WindowsPhone81\%winLegacy%.pdb" (
    del "%unityPluginDir%\Metro\WindowsPhone81\%winLegacy%.pdb"
    del "%unityPluginDir%\Metro\WindowsPhone81\%winLegacy%.pdb.meta"
)
if exist "%unityPluginDir%\%winLegacy%.pdb" (
    del "%unityPluginDir%\%winLegacy%.pdb"
    del "%unityPluginDir%\%winLegacy%.pdb.meta"
)
if exist "%unityPluginDir%\%winLegacy%.mdb" (
    del "%unityPluginDir%\%winLegacy%.mdb"
    del "%unityPluginDir%\%winLegacy%.mdb.meta"
)

if exist "%unityPluginDir%\Metro\WSA8.1\%winIntegration%.pdb" (
    del "%unityPluginDir%\Metro\WSA8.1\%winIntegration%.pdb"
    del "%unityPluginDir%\Metro\WSA8.1\%winIntegration%.pdb.meta"
)
if exist "%unityPluginDir%\Metro\WSA10\%winIntegration%.pdb" (
    del "%unityPluginDir%\Metro\WSA10\%winIntegration%.pdb"
    del "%unityPluginDir%\Metro\WSA10\%winIntegration%.pdb.meta"
)
if exist "%unityPluginDir%\Metro\WindowsPhone81\%winIntegration%.pdb" (
    del "%unityPluginDir%\Metro\WindowsPhone81\%winIntegration%.pdb"
    del "%unityPluginDir%\Metro\WindowsPhone81\%winIntegration%.pdb.meta"
)
if exist "%unityPluginDir%\%winIntegration%.pdb" (
    del "%unityPluginDir%\%winIntegration%.pdb"
    del "%unityPluginDir%\%winIntegration%.pdb.meta"
)
if exist "%unityPluginDir%\%winIntegration%.mdb" (
    del "%unityPluginDir%\%winIntegration%.mdb"
    del "%unityPluginDir%\%winIntegration%.mdb.meta"
)

call %vsCommonToolsDir%\vsvars32.bat
echo progress 0
msbuild Update_NuGet_Packages.proj
echo progress 100