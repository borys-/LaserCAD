@echo off
setlocal

set "ROOT=%~dp0"
set "OUTPUT=%ROOT%bin\release"

echo Building LaserCad into "%OUTPUT%"

if exist "%OUTPUT%" (
    rmdir /s /q "%OUTPUT%"
)

dotnet restore "%ROOT%LaserCad.sln"
if errorlevel 1 goto :error

dotnet build "%ROOT%LaserCad.sln" --configuration Release --no-restore -p:OutDir="%OUTPUT%\net9.0\"
if errorlevel 1 goto :error

dotnet build "%ROOT%src\LaserCad.Core\LaserCad.Core.csproj" --configuration Release --framework netstandard2.1 --no-restore -p:OutDir="%OUTPUT%\unity-domain\"
if errorlevel 1 goto :error

echo.
echo Build finished.
echo .NET assemblies: "%OUTPUT%\net9.0"
echo Unity domain assemblies: "%OUTPUT%\unity-domain"
echo Unity project: "%ROOT%LaserCad.Unity"
exit /b 0

:error
echo.
echo Build failed.
exit /b 1
