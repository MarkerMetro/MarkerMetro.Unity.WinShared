@echo off

SET vsCommonToolsDir=%1%
IF "%1" == "" (
    SET vsCommonToolsDir="C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\Tools\"
    )

call %vsCommonToolsDir%\vsvars32.bat
echo progress 0
msbuild Update_NuGet_Packages.proj
echo progress 100