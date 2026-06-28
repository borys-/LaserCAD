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

## Granice projektow

### Granica geometrii

`LaserCad.Geometry` jest najnizsza warstwa domenowa.

Moze zawierac:

- typy matematyczne,
- typy geometryczne,
- tolerancje,
- transformacje,
- algorytmy geometryczne.

Nie moze zawierac:

- dokumentu CAD,
- warstw produkcyjnych,
- profili materialowych,
- generatorow,
- eksportu SVG/DXF,
- zaleznosci od Unity.

### Granica core

`LaserCad.Core` modeluje intencje projektu CAD.

Moze zawierac:

- dokument,
- szkice,
- encje,
- parametry,
- wyrazenia,
- warstwy,
- profile materialowe,
- generatory,
- operacje edycyjne.

Nie moze zawierac:

- kodu UI,
- typow Unity,
- logiki rysowania widoku,
- szczegolow formatu SVG/DXF poza neutralnymi danymi domenowymi.

### Granica exporterow

`LaserCad.Export.Svg` i `LaserCad.Export.Dxf` sa adapterami wyjsciowymi.

Moga zawierac:

- mapowanie domeny na format pliku,
- opcje eksportu,
- serializacje wyniku eksportu.

Nie moga zawierac:

- logiki edycji dokumentu,
- generatorow geometrii domenowej,
- interakcji UI,
- zaleznosci od Unity.

### Granica Unity

`LaserCad.Unity` jest adapterem UI.

Moze zawierac:

- renderowanie,
- kontrolery wejscia,
- kamere,
- grid,
- snap wizualny,
- panele narzedzi i parametrow.

Nie moze duplikowac logiki domenowej, ktora nalezy do `LaserCad.Core` albo `LaserCad.Geometry`.

### Granica testow

`LaserCad.Tests` moze zalezec od wszystkich bibliotek testowanych.

Testy nie powinny wymuszac zmian w API tylko dla wygody testu. Jesli API jest trudne do testowania, nalezy poprawic projekt domenowy, a nie obchodzic go przez szczegoly implementacji.

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

## UI i domena

UI jest adapterem prezentacji. Wyswietla stan dokumentu, zbiera intencje uzytkownika i przekazuje je do bibliotek domenowych jako komendy, zmiany parametrow albo wywolania serwisow.

Obliczenia naleza do domeny. Przeliczanie parametrow, przebudowa geometrii, walidacja wymiarow, logika generatorow, kompensacja kerfu i przygotowanie danych do eksportu powinny byc implementowane w `LaserCad.Core` albo `LaserCad.Geometry`, a nie w Unity ani w kodzie widoku.

Dzieki temu ten sam model moze byc testowany jednostkowo, uzywany przez rozne interfejsy i eksportowany bez zaleznosci od konkretnego UI.

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
