# Manualna checklista QA eksportu DXF z ulozonych arkuszy

Ta checklista dotyczy sekcji `16.5 Eksport DXF z ulozonych arkuszy`.

## Przygotowanie

- Uruchomic `C:\borys\CAD\bin\release\LaserCad.Desktop\LaserCad.Desktop.exe`.
- Narysowac kilka plyt przez `Rysuj plyte materialowa 3D`.
- W panelu `Properties` ustawic arkusz, np. `300` x `300`, margines `5`, odstep `5`.
- Kliknac `Przygotuj arkusz` i sprawdzic podglad nestingu.

## Eksport

- Wybrac `Export -> Export Nested DXF...`.
- Wskazac katalog eksportu.
- Sprawdzic, ze powstaje osobny plik dla arkusza, np. `Nazwa-projektu-sheet-001.dxf`.
- Sprawdzic, ze powstaje tez zbiorczy plik `Nazwa projektu-all-sheets.dxf`.

## Weryfikacja DXF

- Otworzyc DXF w zewnetrznym programie.
- Sprawdzic, ze kontury czesci sa na warstwie `Cut`.
- Sprawdzic, ze tabela warstw zawiera `Cut`, `Engrave` i `Score`.
- Dla projektu z otworem sprawdzic, ze otwor jest osobnym zamknietym konturem wewnetrznym na warstwie `Cut`.

## Ograniczenia aktualnego eksportu

- Eksport nestingu zapisuje kontury z `FlatPart`, ale nesting nadal uklada bounding boxy.
- DXF nie dodaje jeszcze etykiet grawerowanych nazw czesci.
- DXF nie zapisuje jeszcze kolejnosci ciecia ani ustawien konkretnej maszyny.
