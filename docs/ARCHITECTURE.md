# Architektura projektu

Ten dokument opisuje podstawowy podzial odpowiedzialnosci w projekcie Laser CAD. Szczegolowe standardy kodowania sa w `docs/CODING_STANDARDS.md`.

## Cel architektury

Architektura ma wspierac parametryczny CAD dla wycinarek laserowych. Najwazniejsze cele:

- testowalna domena bez Unity,
- oddzielenie geometrii od dokumentu CAD,
- oddzielenie modelu domenowego od eksportu,
- mozliwosc przebudowy modelu po zmianie parametrow,
- latwe dodawanie generatorow i eksporterow.

## Projekty

### LaserCad.Geometry

Odpowiada za matematyke i geometrie 2D:

- punkty,
- wektory,
- bounding boxy,
- macierze transformacji,
- linie, odcinki, luki, okregi, polilinie i polygony,
- tolerancje geometryczne,
- podstawowe operacje geometryczne.

Ten projekt nie zna dokumentu CAD, warstw, generatorow, eksportu ani Unity.

### LaserCad.Core

Odpowiada za logike domenowa CAD:

- dokument projektu,
- szkice,
- encje szkicu,
- warstwy,
- profile materialow,
- parametry,
- wyrazenia parametryczne,
- generatory,
- historia operacji.

Moze korzystac z `LaserCad.Geometry`.

### LaserCad.Export.Svg

Odpowiada za eksport dokumentu albo geometrii produkcyjnej do SVG.

Moze korzystac z `LaserCad.Core`, ale nie powinien modyfikowac dokumentu. Eksporter czyta model i produkuje wynik.

### LaserCad.Export.Dxf

Odpowiada za eksport dokumentu albo geometrii produkcyjnej do DXF.

Moze korzystac z `LaserCad.Core`, ale nie powinien modyfikowac dokumentu. Eksporter czyta model i produkuje wynik.

### LaserCad.Unity

Docelowo odpowiada za interfejs uzytkownika, kamere, siatke, snap, zaznaczanie i interakcje.

Unity moze korzystac z bibliotek domenowych, ale biblioteki domenowe nie moga zalezec od Unity.

### LaserCad.Tests

Zawiera testy jednostkowe dla bibliotek domenowych i eksporterow.

Testy uzywaja NUnit i Moq.

## Kierunek zaleznosci

Dozwolony kierunek zaleznosci:

```text
LaserCad.Unity
    -> LaserCad.Core
    -> LaserCad.Geometry

LaserCad.Export.Svg
    -> LaserCad.Core
    -> LaserCad.Geometry

LaserCad.Export.Dxf
    -> LaserCad.Core
    -> LaserCad.Geometry

LaserCad.Tests
    -> wszystkie biblioteki testowane
```

Niedozwolone:

- `LaserCad.Geometry` nie zalezy od `LaserCad.Core`,
- `LaserCad.Core` nie zalezy od exporterow,
- biblioteki domenowe nie zaleza od Unity,
- eksportery nie zaleza od Unity.

## Zasada warstw

Warstwy nizej w architekturze nie znaja warstw wyzej:

- geometria nie wie, czym jest dokument CAD,
- core nie wie, jak wyglada UI,
- eksport nie wykonuje edycji,
- Unity nie implementuje logiki geometrycznej drugi raz.

## Parametrycznosc

Parametrycznosc jest czescia fundamentu:

- parametry dokumentu maja stabilne identyfikatory,
- encje i generatory moga zalezec od parametrow,
- zmiana parametru powinna docelowo przebudowac zalezne elementy,
- generatory pozostaja w dokumencie jako edytowalne instancje.

## Jednostki

Wewnetrzna jednostka dlugosci to milimetry.

Inne jednostki moga pojawic sie tylko jako jawna konwersja na granicy systemu, np. w UI albo imporcie.

## Eksport

Eksportery powinny:

- czytac model domenowy,
- respektowac warstwy,
- respektowac jednostki w milimetrach,
- nie modyfikowac dokumentu,
- miec testy dla podstawowych encji i warstw.

## Testowanie

Kazda istotna logika domenowa powinna miec testy jednostkowe.

Minimalna weryfikacja przed commitem dla zmian w kodzie:

```bash
dotnet test LaserCad.sln
```
