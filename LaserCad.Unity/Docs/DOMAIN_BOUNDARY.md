# Granica domeny w Unity

Unity jest adapterem prezentacji dla Laser CAD. Kod w `LaserCad.Unity` moze:

- tworzyc i aktualizowac elementy UI,
- reagowac na wejscie uzytkownika,
- wyswietlac stan `CadDocument`,
- wywolywac publiczne API `LaserCad.Core` i `LaserCad.Geometry`.

Kod w `LaserCad.Unity` nie powinien:

- implementowac wlasnych typow punktow, wektorow, linii, okregow ani polygonow,
- liczyc przeciec, offsetow, bounding boxow ani transformacji geometrycznych poza prostym mapowaniem widoku,
- przebudowywac dokumentu poza wywolaniem uslug domenowych,
- kopiowac zasad walidacji parametrow z `LaserCad.Core`.

## Weryfikacja dla sekcji 3.0

- `LaserCadApplicationController` tworzy pusty `CadDocument` przez `LaserCad.Core`.
- `DocumentInfoView` tylko wyswietla dane dokumentu.
- W projekcie Unity nie ma osobnych typow geometrycznych ani algorytmow CAD.

