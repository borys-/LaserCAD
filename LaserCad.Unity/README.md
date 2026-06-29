# LaserCad.Unity

Projekt Unity dla interfejsu aplikacji Laser CAD.

## Cel

Ten projekt odpowiada za warstwe prezentacji:

- scene robocza 2D,
- kontrolery wejscia,
- renderowanie widoku,
- panele UI,
- komunikacje z bibliotekami domenowymi.

Logika CAD, geometria i przebudowa dokumentu pozostaja w bibliotekach domenowych `LaserCad.Core` oraz `LaserCad.Geometry`.

Granica odpowiedzialnosci Unity jest opisana w `Docs/DOMAIN_BOUNDARY.md`.

## Uruchamianie

1. Otworz katalog `LaserCad.Unity` w Unity.
2. Otworz scene `Assets/Scenes/Workspace.unity`.
3. Uruchom scene w edytorze Unity.
