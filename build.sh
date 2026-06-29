#!/bin/sh
set -eu

ROOT=$(CDPATH= cd -- "$(dirname -- "$0")" && pwd)
OUTPUT="$ROOT/bin/release"

echo "Building LaserCad into \"$OUTPUT\""

rm -rf "$OUTPUT"

dotnet restore "$ROOT/LaserCad.sln"
dotnet build "$ROOT/LaserCad.sln" --configuration Release --no-restore -p:OutDir="$OUTPUT/net9.0/"
dotnet build "$ROOT/src/LaserCad.Core/LaserCad.Core.csproj" --configuration Release --framework netstandard2.1 --no-restore -p:OutDir="$OUTPUT/unity-domain/"

echo
echo "Build finished."
echo ".NET assemblies: $OUTPUT/net9.0"
echo "Unity domain assemblies: $OUTPUT/unity-domain"
echo "Unity project: $ROOT/LaserCad.Unity"
