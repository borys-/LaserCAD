# Roadmapa projektu Laser CAD

## Cel projektu

Celem projektu jest stworzenie parametrycznego programu CAD wspierajacego projektowanie elementow pod wycinarke laserowa, szczegolnie dla materialow takich jak sklejka, MDF i akryl.

Program ma laczyc proste projektowanie 2D z funkcjami produkcyjnymi typowymi dla lasera: warstwy ciecia i grawerowania, kompensacje kerfu, generatory pudelek i organizerow, finger jointy, eksport SVG/DXF oraz docelowo podglad 3D i nesting.

Kluczowym wyroznikiem ma byc pelna parametrycznosc. Uzytkownik powinien moc zdefiniowac wartosci takie jak:

- szerokosc = 300 mm,
- glebokosc = 200 mm,
- wysokosc = 150 mm,
- grubosc materialu = 3 mm,
- kerf = 0.12 mm,
- szerokosc palca = 10 mm.

Po zmianie dowolnego parametru caly model powinien automatycznie sie przebudowac.

## Zalozenia architektoniczne

- Program powinien byc budowany modulowo, z wyraznym rozdzieleniem geometrii, dokumentu CAD, eksportu, UI i generatorow.
- Logika domenowa powinna dzialac niezaleznie od Unity, aby mozna bylo ja testowac jednostkowo.
- Wszystkie wartosci geometryczne powinny byc przechowywane wewnetrznie w milimetrach.
- Parametrycznosc powinna byc elementem fundamentu, a nie dodatkiem w pozniejszej fazie.
- Generatory powinny tworzyc modele zalezne od parametrow, a nie tylko jednorazowa statyczna geometrie.

## Faza 0 - Fundament projektu

### 0.1 Architektura

- Zalozenie solution.
- Projekty:
  - `LaserCad.Core`,
  - `LaserCad.Geometry`,
  - `LaserCad.Export.Svg`,
  - `LaserCad.Export.Dxf`,
  - `LaserCad.Unity`,
  - projekt testow jednostkowych.
- Dependency Injection.
- Testy jednostkowe od pierwszych klas domenowych.
- Rozdzielenie logiki CAD od interfejsu Unity.

### 0.2 Matematyka

- `Point2D`.
- `Vector2D`.
- `BoundingBox`.
- `Matrix3x3` dla transformacji 2D.
- Tolerancja porownywania punktow i wartosci geometrycznych.
- Operacje pomocnicze:
  - odleglosc,
  - iloczyn skalarny,
  - iloczyn wektorowy 2D,
  - normalizacja,
  - katy,
  - transformacje punktow i wektorow.

### 0.3 Parametric Core

- `Parameter`.
- Typy parametrow:
  - dlugosc,
  - liczba,
  - wartosc logiczna,
  - tekst,
  - wybor z listy.
- Wyrazenia parametryczne, np. `Width - 2 * MaterialThickness`.
- Graf zaleznosci miedzy parametrami.
- Automatyczne przeliczanie wartosci po zmianie parametru.
- Walidacja parametrow, np. `FingerWidth > Kerf * 2`.
- Wykrywanie cyklicznych zaleznosci.

### 0.4 Document Model

- `CadDocument`.
- `Sketch`.
- `Layer`.
- `Entity`.
- `GeneratorInstance`.
- `MaterialProfile`.
- Zapis i odczyt projektu, np. JSON.
- Wersjonowanie formatu pliku.
- Oddzielenie danych projektu od widoku i narzedzi edycyjnych.

### 0.5 Units

- Silny typ dla wymiarow, np. `Length`.
- Wewnetrzna jednostka: milimetry.
- Konwersje z centymetrow i cali jako funkcje pomocnicze.
- Unikanie zwyklego `double` w API tam, gdzie wartosc oznacza fizyczny wymiar.

### 0.6 Material Profiles

- Profile materialow:
  - sklejka 3 mm,
  - sklejka 4 mm,
  - MDF,
  - akryl.
- Domyslna grubosc materialu.
- Domyslny kerf.
- Domyslny luz montazowy.
- Minimalna bezpieczna szerokosc palca.
- Domyslne kolory i warstwy produkcyjne.

## Faza 1 - Kernel geometrii

- Linie:
  - tworzenie,
  - dlugosc,
  - punkt posredni,
  - przeciecia.
- Odcinki.
- Luki.
- Okregi.
- Polilinie.
- Polygony.
- Transformacje:
  - obrot,
  - skalowanie,
  - przesuniecie,
  - odbicie.
- Operacje pomocnicze:
  - obliczanie bounding box,
  - sprawdzanie domkniecia konturu,
  - kierunek konturu,
  - podstawowe offsety.

### Ograniczenia offsetu MVP

Offset w pierwszej wersji kernela geometrii obsluguje tylko zamkniete, proste i wypukle polygony z krawedziami liniowymi. Algorytm nie obsluguje jeszcze luk, polilinii otwartych, polygonow wkleslych, samoprzeciec ani zaawansowanego laczenia krawedzi po offsecie.

Offset zewnetrzny i wewnetrzny sa traktowane jako operacje pomocnicze pod przyszla kompensacje kerfu. Pelna kompensacja kerfu, klasyfikacja konturow jako zewnetrznych/wewnetrznych oraz obsluga otworow pozostaja zakresem fazy 9.

## Faza 2 - Sketch

- Obiekty:
  - line,
  - rectangle,
  - circle,
  - arc,
  - polyline,
  - text.
- Edycja:
  - dodawanie,
  - usuwanie,
  - kopiowanie,
  - obracanie,
  - przesuwanie.
- Warstwy:
  - cut,
  - engrave,
  - score,
  - ignore.
- Przechowywanie relacji miedzy sketchami, parametrami i generatorami.

### Ograniczenia parametrycznego szkicu MVP

Parametryczny szkic w MVP przechowuje proste powiazania wymiaru encji z parametrem dokumentu. Obslugiwane sa szerokosc i wysokosc prostokata oraz srednica okregu, a wartosci wymiarow musza pochodzic z parametrow typu `Length`.

Przebudowa szkicu jest niemutujaca i zachowuje identyfikatory encji oraz powiazania wymiarow. Prostokat jest przebudowywany jako prostokat osiowy opisany przez bounding box, wiec MVP nie jest jeszcze solverem constraintow i nie zachowuje dowolnych obrotow ani relacji geometrycznych po przebudowie parametrycznej.

Osobny model wymiarow szkicu jest dostepny dla dlugosci odcinka, szerokosci i wysokosci prostokata oraz srednicy i promienia okregu. Wymiary moga byc sterowane parametrami typu `Length` i przebudowywac wskazana encje.

Ograniczenia MVP nadal sa istotne: wymiary sa stosowane sekwencyjnie na pojedynczej encji, nie tworza jeszcze pelnego solvera zaleznosci geometrycznych i nie obsluguja luk, polilinii ani tekstu.

## Faza 3 - Unity

- Kamera:
  - zoom,
  - pan,
  - rotate.
- Grid:
  - nieskonczona siatka,
  - skok 1 mm,
  - skok 5 mm,
  - skok 10 mm.
- Snap:
  - do siatki,
  - do punktow,
  - do srodka,
  - do koncow linii.
- Zaznaczanie:
  - klik,
  - prostokat,
  - multi-select.
- Panel parametrow dokumentu i zaznaczonych obiektow.

## Faza 4 - Eksport

### SVG

- `line`.
- `rect`.
- `circle`.
- `arc`.
- `path`.
- Kolory warstw.
- Jednostki w milimetrach.
- Poprawne mapowanie warstw cut, engrave, score i ignore.

### DXF

- `LINE`.
- `ARC`.
- `CIRCLE`.
- `POLYLINE`.
- `LWPOLYLINE`.
- Warstwy DXF zgodne z warstwami dokumentu CAD.

### Ustawienia

- Jednostki: mm.
- Grubosc linii.
- Kolory warstw.
- Eksport tylko wybranych warstw.
- Eksport z lub bez kompensacji kerfu.

## Faza 5 - Edycja

- Operacje:
  - move,
  - rotate,
  - scale,
  - mirror,
  - array,
  - align.
- Historia:
  - undo,
  - redo.
- Feature tree:
  - lista operacji,
  - lista generatorow,
  - mozliwosc edycji parametrow operacji.

## Faza 6 - Constraints i parametry

- Constraints:
  - horizontal,
  - vertical,
  - parallel,
  - perpendicular,
  - coincident,
  - equal.
- Dimensions:
  - dlugosc,
  - szerokosc,
  - wysokosc,
  - srednica,
  - promien.
- Parametry produkcyjne:
  - `MaterialThickness`,
  - `Kerf`,
  - `FingerWidth`,
  - `TabLength`,
  - `Clearance`.
- Powiazanie wymiarow szkicu z parametrami dokumentu.

### Ograniczenia solvera constraints MVP

Solver constraints w pierwszym kroku jest deterministycznym solverem sekwencyjnym dla prostych relacji liniowych. Obsluguje `LineEntity` oraz constrainty horizontal, vertical, parallel, perpendicular, coincident i equal przez zastosowanie ich w przekazanej kolejnosci. Nie jest to jeszcze pelny solver ukladu rownan geometrycznych: nie wykrywa sprzecznych constraintow, nie iteruje do zbieznosci i nie optymalizuje wielu rozwiazan.

## Faza 7 - Generator finger joint

### Algorytm

- Podzial krawedzi.
- Dobor liczby zebow.
- Symetria.
- Dopasowanie naroznikow.
- Uwzglednienie grubosci materialu.
- Uwzglednienie kerfu i luzu montazowego.

### Opcje

- Szerokosc palca.
- Minimalna szerokosc.
- Maksymalna szerokosc.
- Zaczyna od zeba.
- Konczy zebem.
- Tryb ciasny, neutralny lub luzny.

### Ograniczenia modelu finger joint MVP

Model danych polaczenia palcowego zawiera opcje potrzebne do przyszlego generatora: szerokosc palca, minimalna i maksymalna szerokosc, poczatek i koniec zebem, tryb dopasowania oraz kerf i clearance. Sekcja 7.0 nie generuje jeszcze geometrii krawedzi; algorytm podzialu, symetria i kompensacja zostaja w sekcji 7.1.

## Faza 8 - Generatory

- Box:
  - zamkniety,
  - otwarty,
  - z pokrywa.
- Tray.
- Organizer.
- Drawer.
- Divider.
- Pegboard.
- Ramki.
- Stojaki i podstawki.
- Kazdy generator powinien byc instancja parametryczna, ktora mozna edytowac po utworzeniu.

## Faza 9 - Kerf

- Kompensacja:
  - dodatnia,
  - ujemna.
- Offset:
  - wewnetrzny,
  - zewnetrzny.
- Rozroznienie konturow wewnetrznych i zewnetrznych.
- Uwzglednienie kerfu w finger jointach.
- Podglad geometrii przed i po kompensacji.

## Faza 10 - Tekst

- Font:
  - import,
  - wybor fontu,
  - rozmiar,
  - pozycja.
- Zamiana tekstu na krzywe.
- Grawer.
- Opcjonalnie tekst parametryczny, np. etykiety zalezne od parametrow projektu.

## Faza 11 - Podglad 3D

- Mesh.
- Extrusion.
- Grubosc materialu.
- Podglad zlozonych elementow.
- Animacja:
  - skladanie pudelka,
  - rozkladanie.
- Kolizje.
- Wizualizacja materialu.

## Faza 12 - Produkcja

- Nesting:
  - automatyczne ukladanie,
  - obrot elementow,
  - margines,
  - odstep miedzy elementami.
- Statystyki:
  - zuzycie materialu,
  - dlugosc ciecia,
  - szacowany czas ciecia.
- Manufacturing checks:
  - wykrywanie podwojnych linii,
  - wykrywanie otwartych konturow,
  - zbyt male odstepy miedzy liniami,
  - zbyt cienkie mostki materialu,
  - kolejnosc ciecia: otwory wewnetrzne przed konturem zewnetrznym.

## Faza 13 - Biblioteka

- Materialy:
  - sklejka 3 mm,
  - sklejka 4 mm,
  - MDF,
  - akryl.
- Szablony:
  - pudelka,
  - organizery,
  - podstawki,
  - stojaki.
- Biblioteka profili materialowych.
- Biblioteka ustawien generatorow.

## Faza 14 - Pluginy

- API:
  - wlasne generatory,
  - wlasne eksportery,
  - wlasne narzedzia.
- Stabilne kontrakty dla pluginow.
- Izolacja pluginow od wewnetrznych szczegolow UI.
- Przyklady pluginow referencyjnych.

## Faza 15 - Wersja 1.0

- Instalator.
- Auto update.
- Ustawienia aplikacji.
- Skroty klawiszowe.
- Dokumentacja.
- Tutoriale.
- Przykladowe projekty.
- Stabilny format pliku.
- Zestaw testow regresyjnych.

## Wyrozniki produktu

### Pelna parametrycznosc

Uzytkownik powinien moc projektowac przez intencje i wymiary, nie tylko przez reczne rysowanie linii. Zmiana parametru, np. grubosci materialu albo szerokosci pudelka, powinna przebudowywac caly model.

### Generatory jako modele parametryczne

Generator pudelka nie powinien tworzyc tylko statycznego zestawu linii. Powinien pozostac w dokumencie jako edytowalna instancja z parametrami:

- `Width`,
- `Depth`,
- `Height`,
- `MaterialThickness`,
- `Kerf`,
- `FingerWidth`,
- `Clearance`.

### Kalibracja kerfu

Program powinien umozliwiac wygenerowanie probnika kerfu, wyciecie go, wpisanie pomiaru i automatyczne dobranie wartosci `Kerf`.

### Test patterns

- Probnik kerfu.
- Probnik finger joint.
- Probnik press-fit.
- Probnik grawerowania.

### Manufacturing checks

Program powinien ostrzegac uzytkownika przed problemami produkcyjnymi jeszcze przed eksportem pliku do lasera.

### Feature tree

Historia modelu powinna zawierac operacje i generatory, aby uzytkownik mogl wrocic do parametrow zamiast recznie poprawiac wynikowa geometrie.

## MVP - pierwsza realna wersja

Pierwsza wersja powinna byc mala, ale kompletna produkcyjnie. Proponowany zakres MVP:

- Core math + geometry.
- Document model.
- Parameters + expressions.
- Units.
- Material profiles.
- Layers.
- SVG export.
- Box generator z finger joints.
- Kerf compensation.
- Unity viewport 2D.
- Zapis i odczyt projektu.
- Testy jednostkowe.

MVP powinno pozwolic uzytkownikowi:

1. Wybrac material i grubosc.
2. Ustawic kerf.
3. Wygenerowac parametryczne pudelko.
4. Zmienic wymiary pudelka i zobaczyc automatyczna przebudowe.
5. Wyeksportowac poprawny plik SVG do ciecia.

## Kryteria weryfikacji dokumentu

- Plik `ROADMAP.md` istnieje w katalogu projektu.
- Dokument jest zapisany w jezyku polskim.
- Markdown ma czytelne naglowki i logiczna kolejnosc.
- Plan zawiera oryginalne fazy 0-15.
- Plan zawiera rozszerzenia dotyczace parametrycznosci:
  - `Parametric Core`,
  - `Document Model`,
  - `Units`,
  - `Material Profiles`.
- Plan zawiera sekcje MVP.
- Dokument nie tworzy jeszcze szczegolowej specyfikacji klas ani implementacji kodu.
