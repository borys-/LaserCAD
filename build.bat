@echo off
setlocal

set "ROOT=%~dp0"
set "OUTPUT=%ROOT%bin\release"
set "UNITY_PROJECT=%ROOT%LaserCad.Unity"
set "UNITY_OUTPUT=%OUTPUT%\LaserCad\LaserCad.exe"
set "DESKTOP_OUTPUT=%OUTPUT%\LaserCad.Desktop"

if not defined UNITY_EXE (
    set "UNITY_EXE=C:\Program Files\Unity\Hub\Editor\6000.0.0f1\Editor\Unity.exe"
)

echo Building LaserCad into "%OUTPUT%"

if exist "%OUTPUT%" (
    rmdir /s /q "%OUTPUT%"
)

dotnet restore "%ROOT%LaserCad.sln"
if errorlevel 1 goto :error

dotnet build "%ROOT%LaserCad.sln" --configuration Release --no-restore -p:OutDir="%OUTPUT%\net9.0\"
if errorlevel 1 goto :error

dotnet publish "%ROOT%src\LaserCad.Desktop\LaserCad.Desktop.csproj" --configuration Release --no-restore --output "%DESKTOP_OUTPUT%"
if errorlevel 1 goto :error

dotnet build "%ROOT%src\LaserCad.Core\LaserCad.Core.csproj" --configuration Release --framework netstandard2.1 --no-restore -p:OutDir="%OUTPUT%\unity-domain\"
if errorlevel 1 goto :error

dotnet build "%ROOT%src\LaserCad.Export.Svg\LaserCad.Export.Svg.csproj" --configuration Release --framework netstandard2.1 --no-restore -p:OutDir="%OUTPUT%\unity-svg\"
if errorlevel 1 goto :error

dotnet build "%ROOT%src\LaserCad.Export.Dxf\LaserCad.Export.Dxf.csproj" --configuration Release --framework netstandard2.1 --no-restore -p:OutDir="%OUTPUT%\unity-dxf\"
if errorlevel 1 goto :error

if exist "%UNITY_EXE%" (
    echo.
    echo Building Unity player into "%UNITY_OUTPUT%"
    "%UNITY_EXE%" -batchmode -quit -projectPath "%UNITY_PROJECT%" -executeMethod LaserCad.Unity.Editor.BuildPlayer.BuildWindows -buildOutput "%UNITY_OUTPUT%" -logFile "%OUTPUT%\unity-build.log"
    if errorlevel 1 goto :error
) else (
    echo.
    echo Unity executable was not found: "%UNITY_EXE%"
    echo Set UNITY_EXE to build the desktop player.
)

echo.
echo Build finished.
echo .NET assemblies: "%OUTPUT%\net9.0"
echo Desktop shell: "%DESKTOP_OUTPUT%\LaserCad.Desktop.exe"
echo Unity domain assemblies: "%OUTPUT%\unity-domain"
echo Unity SVG assemblies: "%OUTPUT%\unity-svg"
echo Unity DXF assemblies: "%OUTPUT%\unity-dxf"
echo Unity project: "%UNITY_PROJECT%"
if exist "%UNITY_OUTPUT%" (
    echo Desktop executable: "%UNITY_OUTPUT%"
)
exit /b 0

:error
echo.
echo Build failed.
exit /b 1
