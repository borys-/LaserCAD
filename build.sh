#!/bin/sh
set -eu

ROOT=$(CDPATH= cd -- "$(dirname -- "$0")" && pwd)
OUTPUT="$ROOT/bin/release"
UNITY_PROJECT="$ROOT/LaserCad.Unity"
UNITY_OUTPUT="$OUTPUT/LaserCad/LaserCad.exe"
: "${UNITY_EXE:=/c/Program Files/Unity/Hub/Editor/6000.0.0f1/Editor/Unity.exe}"

echo "Building LaserCad into \"$OUTPUT\""

rm -rf "$OUTPUT"

dotnet restore "$ROOT/LaserCad.sln"
dotnet build "$ROOT/LaserCad.sln" --configuration Release --no-restore -p:OutDir="$OUTPUT/net9.0/"
dotnet build "$ROOT/src/LaserCad.Core/LaserCad.Core.csproj" --configuration Release --framework netstandard2.1 --no-restore -p:OutDir="$OUTPUT/unity-domain/"

if [ -x "$UNITY_EXE" ]; then
    echo
    echo "Building Unity player into \"$UNITY_OUTPUT\""
    "$UNITY_EXE" -batchmode -quit -projectPath "$UNITY_PROJECT" -executeMethod LaserCad.Unity.Editor.BuildPlayer.BuildWindows -buildOutput "$UNITY_OUTPUT" -logFile "$OUTPUT/unity-build.log"
else
    echo
    echo "Unity executable was not found or is not executable: $UNITY_EXE"
    echo "Set UNITY_EXE to build the desktop player."
fi

echo
echo "Build finished."
echo ".NET assemblies: $OUTPUT/net9.0"
echo "Unity domain assemblies: $OUTPUT/unity-domain"
echo "Unity project: $UNITY_PROJECT"
if [ -f "$UNITY_OUTPUT" ]; then
    echo "Desktop executable: $UNITY_OUTPUT"
fi
