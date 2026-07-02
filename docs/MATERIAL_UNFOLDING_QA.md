# Manualna checklista QA rozwiniecia 3D do 2D

Ta checklista dotyczy sekcji `16.3 Rozwiniecie modelu 3D do czesci 2D`.

## Przygotowanie

- Uruchomic `C:\borys\CAD\bin\release\LaserCad.Desktop\LaserCad.Desktop.exe`.
- Wybrac material w panelu `Material i warstwy`, np. `Sklejka 3 mm`.
- W razie potrzeby uzyc `View -> Clean Workspace`, zeby viewport byl dobrze widoczny.

## Plyty materialowe

- W toolbarze kliknac `Rysuj plyte materialowa 3D`.
- Narysowac kilka plyt o roznych rozmiarach.
- Sprawdzic, ze panel `Properties` pokazuje poprawny licznik `Plyty 3D`.
- Zapisac i ponownie wczytac projekt, zeby potwierdzic utrwalenie `materialSolids`.

## Rozwiniecie domenowe

- Aktualny UI nie ma jeszcze osobnego przycisku pokazujacego liste `FlatPart`.
- Rozwiniecie jest dostepne w domenie przez `MaterialUnfolder`.
- Dla zwyklej plyty `FlatPart.OuterContour` powinien odpowiadac obrysowi plyty.
- Dla plyty z wycieciem `FlatPart.InnerContours` powinien zawierac kontury otworow.
- Warstwy `Cut`, `Engrave`, `Score` i `Ignore` powinny byc przeniesione z dokumentu do czesci.
- Nazwy czesci powinny wynikac z nazw elementow 3D.

## Bryla z pochyla sciana

- `MaterialUnfolder.Unfold(SlopedMaterialSolid)` powinien zwrocic szesc czesci: front, tyl, dwa boki, dno i pochyla gore.
- Boki powinny pozostac trapezami, gdy wysokosc przodu i tylu jest rozna.
- Wyciecia przypisane do nazwanych scian powinny trafic do odpowiednich `InnerContours`.

## Ograniczenia aktualnego UI

- UI nie pokazuje jeszcze automatycznego panelu rozwiniecia 2D.
- Nie ma jeszcze nestingu czesci na arkuszu.
- Nie ma jeszcze eksportu DXF z wyniku rozwiniecia.
- Widoczna aplikacja pozostaje jak po sekcji `16.2`; zmiany `16.3` sa domenowe i testowe.
