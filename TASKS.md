# Lista taskow projektu Laser CAD

Ten dokument rozbija roadmapę na male, stopniowe taski implementacyjne. Numeracja ma format `faza.obszar.krok`, np. `0.2.3`.

Zalecenie pracy:

- realizowac taski po kolei w ramach danej fazy,
- kazdy task konczyc testem, przykladem albo widoczna weryfikacja,
- nie przechodzic do Unity, dopoki logika domenowa nie ma testow,
- utrzymywac parametrycznosc jako fundament, nie jako pozniejszy dodatek.

## 0.0 Organizacja repozytorium

- [x] 0.0.0 Utworzyc plik `README.md` z krotkim opisem celu projektu.
- [x] 0.0.1 Utworzyc plik `.gitignore` dla C#, .NET, Unity i plikow tymczasowych IDE.
- [x] 0.0.2 Ustalic nazwe solution: `LaserCad`.
- [x] 0.0.3 Utworzyc solution `LaserCad.sln`.
- [x] 0.0.4 Utworzyc katalog `src`.
- [x] 0.0.5 Utworzyc katalog `tests`.
- [x] 0.0.6 Utworzyc katalog `docs`.
- [x] 0.0.7 Przeniesc lub skopiowac roadmapę do `docs` albo zostawic w root jako dokument glowny.
- [x] 0.0.8 Dodac krotka instrukcje uruchamiania testow.
- [x] 0.0.9 Zweryfikowac, ze repo buduje sie na czystym checkoutcie.

## 0.1 Projekty i zaleznosci

- [x] 0.1.0 Utworzyc projekt `LaserCad.Core`.
- [x] 0.1.1 Utworzyc projekt `LaserCad.Geometry`.
- [x] 0.1.2 Utworzyc projekt `LaserCad.Export.Svg`.
- [x] 0.1.3 Utworzyc projekt `LaserCad.Export.Dxf`.
- [x] 0.1.4 Utworzyc projekt `LaserCad.Tests`.
- [x] 0.1.5 Dodac projekty do solution.
- [x] 0.1.6 Ustawic docelowa wersje .NET dla bibliotek domenowych.
- [x] 0.1.7 Dodac referencje `LaserCad.Core` -> `LaserCad.Geometry`, jesli Core ma korzystac z geometrii.
- [x] 0.1.8 Dodac referencje exporterow do wymaganych projektow domenowych.
- [x] 0.1.9 Dodac referencje testow do wszystkich bibliotek domenowych.
- [x] 0.1.10 Dodac framework testowy.
- [x] 0.1.11 Dodac pierwszy test pusty/sanity, aby potwierdzic konfiguracje.
- [x] 0.1.12 Uruchomic testy i zapisac komende w `README.md`.

## 0.2 Standardy techniczne

- [x] 0.2.0 Ustalic konwencje namespace, np. `LaserCad.Geometry`.
- [x] 0.2.1 Ustalic konwencje nazewnictwa jednostek: klasy w PascalCase, pola prywatne w camelCase.
- [x] 0.2.2 Dodac podstawowy `.editorconfig`.
- [x] 0.2.3 Ustalic tolerancje geometryczna jako stala domenowa.
- [x] 0.2.4 Ustalic, ze wszystkie wymiary domenowe sa w milimetrach.
- [x] 0.2.5 Ustalic, ze biblioteki domenowe nie zaleza od Unity.
- [x] 0.2.6 Dodac dokument `docs/ARCHITECTURE.md`.
- [x] 0.2.7 Opisac w architekturze granice projektow.
- [x] 0.2.8 Opisac zasade: UI wyswietla dane, domena wykonuje obliczenia.

## 0.3 Jednostki i wartosci wymiarowe

- [x] 0.3.0 Utworzyc typ `Length`.
- [x] 0.3.1 Dodac fabryke `Length.FromMillimeters`.
- [x] 0.3.2 Dodac fabryke `Length.FromCentimeters`.
- [x] 0.3.3 Dodac fabryke `Length.FromInches`.
- [x] 0.3.4 Dodac wlasciwosc `Millimeters`.
- [x] 0.3.5 Dodac operatory dodawania i odejmowania `Length`.
- [x] 0.3.6 Dodac mnozenie `Length` przez liczbe.
- [x] 0.3.7 Dodac dzielenie `Length` przez liczbe.
- [x] 0.3.8 Dodac porownania z tolerancja.
- [x] 0.3.9 Dodac formatowanie do tekstu w mm.
- [x] 0.3.10 Dodac testy konwersji jednostek.
- [x] 0.3.11 Dodac testy operacji arytmetycznych.
- [x] 0.3.12 Dodac testy porownan z tolerancja.

## 0.4 Matematyka 2D

- [x] 0.4.0 Utworzyc `Point2D`.
- [x] 0.4.1 Dodac `Point2D.X` i `Point2D.Y`.
- [x] 0.4.2 Dodac przesuniecie punktu o `Vector2D`.
- [x] 0.4.3 Dodac odleglosc miedzy punktami.
- [x] 0.4.4 Utworzyc `Vector2D`.
- [x] 0.4.5 Dodac dlugosc wektora.
- [x] 0.4.6 Dodac normalizacje wektora.
- [x] 0.4.7 Dodac iloczyn skalarny.
- [x] 0.4.8 Dodac iloczyn wektorowy 2D jako wartosc skalarna.
- [x] 0.4.9 Dodac kat miedzy wektorami.
- [x] 0.4.10 Utworzyc `BoundingBox`.
- [x] 0.4.11 Dodac tworzenie `BoundingBox` z punktow.
- [x] 0.4.12 Dodac laczenie dwoch bounding boxow.
- [x] 0.4.13 Dodac sprawdzanie, czy punkt jest wewnatrz bounding boxa.
- [x] 0.4.14 Utworzyc `Matrix3x3`.
- [x] 0.4.15 Dodac macierz przesuniecia.
- [x] 0.4.16 Dodac macierz obrotu.
- [x] 0.4.17 Dodac macierz skalowania.
- [x] 0.4.18 Dodac macierz odbicia.
- [x] 0.4.19 Dodac mnozenie macierzy.
- [x] 0.4.20 Dodac transformowanie punktu.
- [x] 0.4.21 Dodac transformowanie wektora.
- [x] 0.4.22 Dodac testy dla wszystkich operacji matematycznych.

## 0.5 Parametry

- [x] 0.5.0 Utworzyc enum `ParameterType`.
- [x] 0.5.1 Dodac typy: length, number, boolean, text, choice.
- [x] 0.5.2 Utworzyc `ParameterId`.
- [x] 0.5.3 Utworzyc `Parameter`.
- [x] 0.5.4 Dodac nazwe parametru.
- [x] 0.5.5 Dodac wartosc parametru.
- [x] 0.5.6 Dodac opcjonalna jednostke wyswietlania.
- [x] 0.5.7 Dodac opcjonalne minimum i maksimum.
- [x] 0.5.8 Dodac walidacje wartosci parametru.
- [x] 0.5.9 Dodac kolekcje `ParameterSet`.
- [x] 0.5.10 Dodac wyszukiwanie parametru po id.
- [x] 0.5.11 Dodac wyszukiwanie parametru po nazwie.
- [x] 0.5.12 Dodac aktualizacje wartosci parametru.
- [x] 0.5.13 Dodac testy tworzenia parametrow.
- [x] 0.5.14 Dodac testy walidacji minimum i maksimum.
- [x] 0.5.15 Dodac testy aktualizacji wartosci.

## 0.6 Wyrazenia parametryczne

- [ ] 0.6.0 Ustalic minimalna skladnie wyrazen, np. `Width - 2 * MaterialThickness`.
- [ ] 0.6.1 Utworzyc reprezentacje `Expression`.
- [ ] 0.6.2 Dodac wyrazenie stale.
- [ ] 0.6.3 Dodac wyrazenie referencji do parametru.
- [ ] 0.6.4 Dodac operacje dodawania.
- [ ] 0.6.5 Dodac operacje odejmowania.
- [ ] 0.6.6 Dodac operacje mnozenia.
- [ ] 0.6.7 Dodac operacje dzielenia.
- [ ] 0.6.8 Dodac ewaluator wyrazen.
- [ ] 0.6.9 Dodac obsluge brakujacego parametru.
- [ ] 0.6.10 Dodac obsluge dzielenia przez zero.
- [ ] 0.6.11 Dodac parser prostych wyrazen tekstowych albo zostawic jawne budowanie AST w MVP.
- [ ] 0.6.12 Dodac test: `Width - 2 * MaterialThickness`.
- [ ] 0.6.13 Dodac test: aktualizacja parametru zmienia wynik wyrazenia.
- [ ] 0.6.14 Dodac test: brakujaca referencja zwraca czytelny blad.

## 0.7 Graf zaleznosci parametrow

- [ ] 0.7.0 Utworzyc `DependencyGraph`.
- [ ] 0.7.1 Dodac rejestrowanie zaleznosci parametru od innych parametrow.
- [ ] 0.7.2 Dodac wyliczanie kolejnosci przeliczania.
- [ ] 0.7.3 Dodac wykrywanie cyklu zaleznosci.
- [ ] 0.7.4 Dodac wynik bledu dla cyklu zaleznosci.
- [ ] 0.7.5 Dodac liste parametrow dotknietych zmiana.
- [ ] 0.7.6 Dodac test prostego lancucha zaleznosci.
- [ ] 0.7.7 Dodac test wielu zaleznosci jednego parametru.
- [ ] 0.7.8 Dodac test wykrywania cyklu.

## 0.8 Model dokumentu

- [ ] 0.8.0 Utworzyc `CadDocument`.
- [ ] 0.8.1 Dodac identyfikator dokumentu.
- [ ] 0.8.2 Dodac nazwe dokumentu.
- [ ] 0.8.3 Dodac wersje formatu pliku.
- [ ] 0.8.4 Dodac kolekcje parametrow dokumentu.
- [ ] 0.8.5 Dodac kolekcje warstw.
- [ ] 0.8.6 Dodac kolekcje szkicow.
- [ ] 0.8.7 Dodac kolekcje generatorow.
- [ ] 0.8.8 Dodac profil materialu dokumentu.
- [ ] 0.8.9 Utworzyc `Sketch`.
- [ ] 0.8.10 Dodac kolekcje encji w szkicu.
- [ ] 0.8.11 Utworzyc bazowy kontrakt `Entity`.
- [ ] 0.8.12 Utworzyc `GeneratorInstance`.
- [ ] 0.8.13 Dodac powiazanie generatora z parametrami.
- [ ] 0.8.14 Dodac test tworzenia pustego dokumentu.
- [ ] 0.8.15 Dodac test dodawania szkicu.
- [ ] 0.8.16 Dodac test dodawania parametrow dokumentu.

## 0.9 Warstwy i profile materialow

- [ ] 0.9.0 Utworzyc enum `LayerRole`.
- [ ] 0.9.1 Dodac role: cut, engrave, score, ignore.
- [ ] 0.9.2 Utworzyc `Layer`.
- [ ] 0.9.3 Dodac nazwe warstwy.
- [ ] 0.9.4 Dodac kolor warstwy.
- [ ] 0.9.5 Dodac role warstwy.
- [ ] 0.9.6 Dodac domyslne warstwy dokumentu.
- [ ] 0.9.7 Utworzyc `MaterialProfile`.
- [ ] 0.9.8 Dodac nazwe materialu.
- [ ] 0.9.9 Dodac grubosc materialu.
- [ ] 0.9.10 Dodac domyslny kerf.
- [ ] 0.9.11 Dodac domyslny clearance.
- [ ] 0.9.12 Dodac minimalna szerokosc palca.
- [ ] 0.9.13 Dodac profile: sklejka 3 mm, sklejka 4 mm, MDF, akryl.
- [ ] 0.9.14 Dodac test domyslnych warstw.
- [ ] 0.9.15 Dodac test domyslnych profili materialow.

## 0.10 Zapis i odczyt projektu

- [ ] 0.10.0 Ustalic format pliku projektu, np. JSON.
- [ ] 0.10.1 Utworzyc `DocumentSerializer`.
- [ ] 0.10.2 Dodac serializacje pustego dokumentu.
- [ ] 0.10.3 Dodac deserializacje pustego dokumentu.
- [ ] 0.10.4 Dodac serializacje parametrow.
- [ ] 0.10.5 Dodac serializacje warstw.
- [ ] 0.10.6 Dodac serializacje profilu materialu.
- [ ] 0.10.7 Dodac pole wersji formatu.
- [ ] 0.10.8 Dodac blad dla nieobslugiwanej wersji.
- [ ] 0.10.9 Dodac test round-trip pustego dokumentu.
- [ ] 0.10.10 Dodac test round-trip dokumentu z parametrami i warstwami.

## 1.0 Linie i odcinki

- [ ] 1.0.0 Utworzyc `Line2D` jako linie nieskonczona albo kontrakt pomocniczy.
- [ ] 1.0.1 Utworzyc `LineSegment2D`.
- [ ] 1.0.2 Dodac punkt poczatkowy odcinka.
- [ ] 1.0.3 Dodac punkt koncowy odcinka.
- [ ] 1.0.4 Dodac dlugosc odcinka.
- [ ] 1.0.5 Dodac punkt posredni dla parametru `t` od 0 do 1.
- [ ] 1.0.6 Dodac kierunek odcinka.
- [ ] 1.0.7 Dodac bounding box odcinka.
- [ ] 1.0.8 Dodac transformacje odcinka.
- [ ] 1.0.9 Dodac test dlugosci.
- [ ] 1.0.10 Dodac test punktu posredniego.
- [ ] 1.0.11 Dodac test transformacji odcinka.

## 1.1 Przeciecia

- [ ] 1.1.0 Utworzyc typ wyniku przeciecia.
- [ ] 1.1.1 Obsluzyc brak przeciecia.
- [ ] 1.1.2 Obsluzyc jedno przeciecie.
- [ ] 1.1.3 Obsluzyc odcinki rownolegle.
- [ ] 1.1.4 Obsluzyc odcinki wspolliniowe.
- [ ] 1.1.5 Dodac przeciecie linia-linia.
- [ ] 1.1.6 Dodac przeciecie odcinek-odcinek.
- [ ] 1.1.7 Dodac przeciecie linia-odcinek.
- [ ] 1.1.8 Dodac test przeciecia krzyzowego.
- [ ] 1.1.9 Dodac test braku przeciecia.
- [ ] 1.1.10 Dodac test przeciecia na koncu odcinka.
- [ ] 1.1.11 Dodac test odcinkow wspolliniowych.

## 1.2 Okregi i luki

- [ ] 1.2.0 Utworzyc `Circle2D`.
- [ ] 1.2.1 Dodac srodek okregu.
- [ ] 1.2.2 Dodac promien okregu.
- [ ] 1.2.3 Dodac obwod okregu.
- [ ] 1.2.4 Dodac bounding box okregu.
- [ ] 1.2.5 Utworzyc `Arc2D`.
- [ ] 1.2.6 Dodac srodek luku.
- [ ] 1.2.7 Dodac promien luku.
- [ ] 1.2.8 Dodac kat poczatkowy i koncowy.
- [ ] 1.2.9 Dodac kierunek luku.
- [ ] 1.2.10 Dodac dlugosc luku.
- [ ] 1.2.11 Dodac punkt na luku dla parametru `t`.
- [ ] 1.2.12 Dodac transformacje okregu i luku.
- [ ] 1.2.13 Dodac testy okregu.
- [ ] 1.2.14 Dodac testy luku.

## 1.3 Polilinie i polygony

- [ ] 1.3.0 Utworzyc `Polyline2D`.
- [ ] 1.3.1 Dodac liste punktow polilinii.
- [ ] 1.3.2 Dodac informacje, czy polilinia jest zamknieta.
- [ ] 1.3.3 Dodac dlugosc polilinii.
- [ ] 1.3.4 Dodac bounding box polilinii.
- [ ] 1.3.5 Dodac transformacje polilinii.
- [ ] 1.3.6 Utworzyc `Polygon2D`.
- [ ] 1.3.7 Dodac pole powierzchni polygonu.
- [ ] 1.3.8 Dodac kierunek polygonu: clockwise/counter-clockwise.
- [ ] 1.3.9 Dodac test zamknietej polilinii.
- [ ] 1.3.10 Dodac test pola prostokata.
- [ ] 1.3.11 Dodac test kierunku polygonu.

## 1.4 Offsety i kontury

- [ ] 1.4.0 Utworzyc kontrakt `Contour2D`.
- [ ] 1.4.1 Dodac wykrywanie, czy kontur jest domkniety.
- [ ] 1.4.2 Dodac wykrywanie samoprzeciec dla prostych polygonow.
- [ ] 1.4.3 Dodac prosty offset polygonu wypuklego.
- [ ] 1.4.4 Dodac offset zewnetrzny.
- [ ] 1.4.5 Dodac offset wewnetrzny.
- [ ] 1.4.6 Dodac test offsetu kwadratu na zewnatrz.
- [ ] 1.4.7 Dodac test offsetu kwadratu do wewnatrz.
- [ ] 1.4.8 Oznaczyc ograniczenia offsetu MVP w dokumentacji.

## 2.0 Encje szkicu

- [ ] 2.0.0 Utworzyc bazowy interfejs `ISketchEntity`.
- [ ] 2.0.1 Dodac identyfikator encji.
- [ ] 2.0.2 Dodac identyfikator warstwy encji.
- [ ] 2.0.3 Dodac bounding box encji.
- [ ] 2.0.4 Dodac transformacje encji.
- [ ] 2.0.5 Utworzyc `LineEntity`.
- [ ] 2.0.6 Utworzyc `RectangleEntity`.
- [ ] 2.0.7 Utworzyc `CircleEntity`.
- [ ] 2.0.8 Utworzyc `ArcEntity`.
- [ ] 2.0.9 Utworzyc `PolylineEntity`.
- [ ] 2.0.10 Utworzyc `TextEntity` jako placeholder.
- [ ] 2.0.11 Dodac test tworzenia kazdej encji.

## 2.1 Operacje na szkicu

- [ ] 2.1.0 Dodac metode dodawania encji do szkicu.
- [ ] 2.1.1 Dodac metode usuwania encji ze szkicu.
- [ ] 2.1.2 Dodac metode kopiowania encji.
- [ ] 2.1.3 Dodac przesuwanie encji.
- [ ] 2.1.4 Dodac obracanie encji.
- [ ] 2.1.5 Dodac skalowanie encji.
- [ ] 2.1.6 Dodac odbicie encji.
- [ ] 2.1.7 Dodac test dodania i usuniecia encji.
- [ ] 2.1.8 Dodac test kopiowania encji.
- [ ] 2.1.9 Dodac test transformacji encji.

## 2.2 Relacje szkicu z parametrami

- [ ] 2.2.0 Dodac mozliwosc przypisania wymiaru encji do parametru.
- [ ] 2.2.1 Dodac aktualizacje prostokata po zmianie parametru szerokosci.
- [ ] 2.2.2 Dodac aktualizacje prostokata po zmianie parametru wysokosci.
- [ ] 2.2.3 Dodac aktualizacje okregu po zmianie parametru srednicy.
- [ ] 2.2.4 Dodac test przebudowy szkicu po zmianie parametru.
- [ ] 2.2.5 Opisac ograniczenia parametrycznego szkicu w MVP.

## 3.0 Integracja Unity - baza

- [ ] 3.0.0 Utworzyc projekt Unity `LaserCad.Unity`.
- [ ] 3.0.1 Podlaczyc biblioteki domenowe do Unity.
- [ ] 3.0.2 Utworzyc scene robocza 2D.
- [ ] 3.0.3 Utworzyc glowny kontroler aplikacji.
- [ ] 3.0.4 Zaladowac pusty `CadDocument` w Unity.
- [ ] 3.0.5 Wyswietlic podstawowe informacje o dokumencie w UI.
- [ ] 3.0.6 Zweryfikowac, ze Unity nie zawiera logiki geometrycznej duplikujacej Core.

## 3.1 Kamera

- [ ] 3.1.0 Dodac kamere ortograficzna.
- [ ] 3.1.1 Dodac zoom kolkiem myszy.
- [ ] 3.1.2 Dodac pan srodkowym przyciskiem lub prawym przyciskiem myszy.
- [ ] 3.1.3 Dodac reset widoku.
- [ ] 3.1.4 Dodac ograniczenia minimalnego i maksymalnego zoomu.
- [ ] 3.1.5 Dodac test manualny kamery do checklisty QA.

## 3.2 Grid

- [ ] 3.2.0 Utworzyc renderer siatki.
- [ ] 3.2.1 Dodac siatke 1 mm.
- [ ] 3.2.2 Dodac mocniejsza linie co 5 mm.
- [ ] 3.2.3 Dodac mocniejsza linie co 10 mm.
- [ ] 3.2.4 Dodac dopasowanie grubosci linii do zoomu.
- [ ] 3.2.5 Dodac mozliwosc wlaczenia i wylaczenia siatki.

## 3.3 Snap

- [ ] 3.3.0 Utworzyc `SnapService`.
- [ ] 3.3.1 Dodac snap do siatki.
- [ ] 3.3.2 Dodac snap do punktow encji.
- [ ] 3.3.3 Dodac snap do srodka okregu/prostokata.
- [ ] 3.3.4 Dodac snap do koncow linii.
- [ ] 3.3.5 Dodac priorytety snapowania.
- [ ] 3.3.6 Dodac wizualny marker snapu.

## 3.4 Zaznaczanie

- [ ] 3.4.0 Utworzyc `SelectionService`.
- [ ] 3.4.1 Dodac zaznaczanie kliknieciem.
- [ ] 3.4.2 Dodac odznaczanie kliknieciem w puste miejsce.
- [ ] 3.4.3 Dodac multi-select z klawiszem modyfikatora.
- [ ] 3.4.4 Dodac zaznaczanie prostokatem.
- [ ] 3.4.5 Dodac highlight zaznaczonych encji.
- [ ] 3.4.6 Dodac panel wlasciwosci zaznaczenia.

## 4.0 Eksport SVG - fundament

- [ ] 4.0.0 Utworzyc `SvgExportOptions`.
- [ ] 4.0.1 Dodac jednostke eksportu: mm.
- [ ] 4.0.2 Dodac ustawienie grubosci linii.
- [ ] 4.0.3 Dodac ustawienie eksportowanych warstw.
- [ ] 4.0.4 Utworzyc `SvgExporter`.
- [ ] 4.0.5 Dodac eksport pustego dokumentu.
- [ ] 4.0.6 Dodac poprawny `viewBox`.
- [ ] 4.0.7 Dodac test snapshot dla pustego SVG.

## 4.1 Eksport SVG - encje

- [ ] 4.1.0 Dodac eksport `LineEntity` jako `line`.
- [ ] 4.1.1 Dodac eksport `RectangleEntity` jako `rect` albo `path`.
- [ ] 4.1.2 Dodac eksport `CircleEntity` jako `circle`.
- [ ] 4.1.3 Dodac eksport `ArcEntity` jako `path`.
- [ ] 4.1.4 Dodac eksport `PolylineEntity` jako `polyline` albo `path`.
- [ ] 4.1.5 Dodac mapowanie koloru warstwy.
- [ ] 4.1.6 Dodac pomijanie warstwy `ignore`.
- [ ] 4.1.7 Dodac test eksportu kazdej encji.
- [ ] 4.1.8 Dodac test eksportu warstw.

## 4.2 Eksport DXF

- [ ] 4.2.0 Utworzyc `DxfExportOptions`.
- [ ] 4.2.1 Utworzyc `DxfExporter`.
- [ ] 4.2.2 Dodac eksport `LINE`.
- [ ] 4.2.3 Dodac eksport `CIRCLE`.
- [ ] 4.2.4 Dodac eksport `ARC`.
- [ ] 4.2.5 Dodac eksport `LWPOLYLINE`.
- [ ] 4.2.6 Dodac warstwy DXF.
- [ ] 4.2.7 Dodac test podstawowego pliku DXF.

## 5.0 Komendy edycyjne

- [ ] 5.0.0 Utworzyc interfejs `ICommand`.
- [ ] 5.0.1 Dodac metode wykonania komendy.
- [ ] 5.0.2 Dodac metode cofniecia komendy.
- [ ] 5.0.3 Utworzyc `MoveCommand`.
- [ ] 5.0.4 Utworzyc `RotateCommand`.
- [ ] 5.0.5 Utworzyc `ScaleCommand`.
- [ ] 5.0.6 Utworzyc `MirrorCommand`.
- [ ] 5.0.7 Utworzyc `DeleteCommand`.
- [ ] 5.0.8 Utworzyc `AddEntityCommand`.
- [ ] 5.0.9 Dodac test wykonania kazdej komendy.
- [ ] 5.0.10 Dodac test cofniecia kazdej komendy.

## 5.1 Undo i redo

- [ ] 5.1.0 Utworzyc `UndoRedoStack`.
- [ ] 5.1.1 Dodac stos undo.
- [ ] 5.1.2 Dodac stos redo.
- [ ] 5.1.3 Dodac czyszczenie redo po nowej komendzie.
- [ ] 5.1.4 Dodac limit historii.
- [ ] 5.1.5 Dodac grupowanie komend.
- [ ] 5.1.6 Dodac test undo.
- [ ] 5.1.7 Dodac test redo.
- [ ] 5.1.8 Dodac test czyszczenia redo.

## 5.2 Feature tree

- [ ] 5.2.0 Utworzyc model elementu drzewa historii.
- [ ] 5.2.1 Dodac wpis dla generatora.
- [ ] 5.2.2 Dodac wpis dla operacji edycyjnej.
- [ ] 5.2.3 Dodac aktywowanie/dezaktywowanie operacji.
- [ ] 5.2.4 Dodac przebudowe dokumentu od feature tree.
- [ ] 5.2.5 Dodac test prostego drzewa: generator + move.

## 6.0 Constraints

- [ ] 6.0.0 Utworzyc bazowy kontrakt constraintu.
- [ ] 6.0.1 Dodac `HorizontalConstraint`.
- [ ] 6.0.2 Dodac `VerticalConstraint`.
- [ ] 6.0.3 Dodac `ParallelConstraint`.
- [ ] 6.0.4 Dodac `PerpendicularConstraint`.
- [ ] 6.0.5 Dodac `CoincidentConstraint`.
- [ ] 6.0.6 Dodac `EqualConstraint`.
- [ ] 6.0.7 Dodac prosty solver dla ograniczonego zestawu przypadkow MVP.
- [ ] 6.0.8 Dodac test horizontal.
- [ ] 6.0.9 Dodac test vertical.
- [ ] 6.0.10 Dodac test coincident.

## 6.1 Dimensions

- [ ] 6.1.0 Utworzyc `Dimension`.
- [ ] 6.1.1 Dodac wymiar dlugosci odcinka.
- [ ] 6.1.2 Dodac wymiar szerokosci prostokata.
- [ ] 6.1.3 Dodac wymiar wysokosci prostokata.
- [ ] 6.1.4 Dodac wymiar srednicy okregu.
- [ ] 6.1.5 Dodac wymiar promienia okregu.
- [ ] 6.1.6 Powiazac wymiar z parametrem.
- [ ] 6.1.7 Dodac test zmiany wymiaru przez parametr.

## 7.0 Finger joint - model danych

- [ ] 7.0.0 Utworzyc `FingerJointOptions`.
- [ ] 7.0.1 Dodac szerokosc palca.
- [ ] 7.0.2 Dodac minimalna szerokosc palca.
- [ ] 7.0.3 Dodac maksymalna szerokosc palca.
- [ ] 7.0.4 Dodac flage zaczynania od zeba.
- [ ] 7.0.5 Dodac flage konczenia zebem.
- [ ] 7.0.6 Dodac tryb ciasny/neutralny/luzny.
- [ ] 7.0.7 Dodac kerf i clearance.
- [ ] 7.0.8 Dodac walidacje opcji.
- [ ] 7.0.9 Dodac test walidacji opcji.

## 7.1 Finger joint - algorytm

- [ ] 7.1.0 Utworzyc `FingerJointGenerator`.
- [ ] 7.1.1 Dodac podzial krawedzi na segmenty.
- [ ] 7.1.2 Dodac dobor liczby zebow.
- [ ] 7.1.3 Dodac wymuszanie symetrii.
- [ ] 7.1.4 Dodac generowanie profilu krawedzi.
- [ ] 7.1.5 Dodac kompensacje grubosci materialu.
- [ ] 7.1.6 Dodac kompensacje kerfu.
- [ ] 7.1.7 Dodac kompensacje clearance.
- [ ] 7.1.8 Dodac test krawedzi 100 mm.
- [ ] 7.1.9 Dodac test symetrii.
- [ ] 7.1.10 Dodac test zaczynania od zeba.
- [ ] 7.1.11 Dodac test zaczynania od wciecia.

## 8.0 Generator pudelka

- [ ] 8.0.0 Utworzyc `BoxGeneratorOptions`.
- [ ] 8.0.1 Dodac `Width`.
- [ ] 8.0.2 Dodac `Depth`.
- [ ] 8.0.3 Dodac `Height`.
- [ ] 8.0.4 Dodac `MaterialThickness`.
- [ ] 8.0.5 Dodac `Kerf`.
- [ ] 8.0.6 Dodac `FingerWidth`.
- [ ] 8.0.7 Dodac `Clearance`.
- [ ] 8.0.8 Dodac typ pudelka: zamkniete, otwarte, z pokrywa.
- [ ] 8.0.9 Dodac walidacje wymiarow.
- [ ] 8.0.10 Dodac test walidacji zbyt malego pudelka.

## 8.1 Generator pudelka - geometria

- [ ] 8.1.0 Utworzyc `BoxGenerator`.
- [ ] 8.1.1 Wygenerowac przednia scianke.
- [ ] 8.1.2 Wygenerowac tylna scianke.
- [ ] 8.1.3 Wygenerowac lewa scianke.
- [ ] 8.1.4 Wygenerowac prawa scianke.
- [ ] 8.1.5 Wygenerowac dno.
- [ ] 8.1.6 Wygenerowac pokrywe dla trybu z pokrywa.
- [ ] 8.1.7 Dodac finger jointy na krawedziach.
- [ ] 8.1.8 Rozlozyc elementy na plaszczyznie 2D.
- [ ] 8.1.9 Dodac marginesy miedzy elementami.
- [ ] 8.1.10 Dodac warstwy cut dla konturow.
- [ ] 8.1.11 Dodac test liczby wygenerowanych scianek.
- [ ] 8.1.12 Dodac test zmiany `Width` przebudowuje geometrie.
- [ ] 8.1.13 Dodac test zmiany `MaterialThickness` przebudowuje finger jointy.

## 8.2 Pozostale generatory

- [ ] 8.2.0 Utworzyc wspolny interfejs generatora.
- [ ] 8.2.1 Dodac generator tray.
- [ ] 8.2.2 Dodac generator organizer.
- [ ] 8.2.3 Dodac generator drawer.
- [ ] 8.2.4 Dodac generator divider.
- [ ] 8.2.5 Dodac generator pegboard.
- [ ] 8.2.6 Dodac generator ramki.
- [ ] 8.2.7 Dodac generator stojaka/podstawki.
- [ ] 8.2.8 Dodac test, ze kazdy generator zwraca poprawny dokument/sketch.

## 9.0 Kerf compensation

- [ ] 9.0.0 Utworzyc `KerfCompensationOptions`.
- [ ] 9.0.1 Dodac wartosc kerfu.
- [ ] 9.0.2 Dodac tryb dodatni.
- [ ] 9.0.3 Dodac tryb ujemny.
- [ ] 9.0.4 Dodac offset wewnetrzny.
- [ ] 9.0.5 Dodac offset zewnetrzny.
- [ ] 9.0.6 Dodac klasyfikacje konturu jako wewnetrzny/zewnetrzny.
- [ ] 9.0.7 Dodac podglad geometrii przed kompensacja.
- [ ] 9.0.8 Dodac podglad geometrii po kompensacji.
- [ ] 9.0.9 Dodac test kompensacji kwadratu zewnetrznego.
- [ ] 9.0.10 Dodac test kompensacji otworu wewnetrznego.

## 9.1 Kalibracja kerfu

- [ ] 9.1.0 Utworzyc generator probnika kerfu.
- [ ] 9.1.1 Dodac zestaw szczelin testowych.
- [ ] 9.1.2 Dodac opis wartosci na probniku jako grawer.
- [ ] 9.1.3 Dodac formularz wpisania pomiaru.
- [ ] 9.1.4 Dodac przeliczenie rekomendowanego kerfu.
- [ ] 9.1.5 Dodac zapis rekomendacji do profilu materialu.
- [ ] 9.1.6 Dodac test obliczenia kerfu z pomiaru.

## 10.0 Tekst i fonty

- [ ] 10.0.0 Utworzyc `TextEntity` z tekstem, pozycja i rozmiarem.
- [ ] 10.0.1 Dodac wybor fontu.
- [ ] 10.0.2 Dodac import fontu.
- [ ] 10.0.3 Dodac alignment tekstu.
- [ ] 10.0.4 Dodac eksport tekstu jako SVG text dla MVP.
- [ ] 10.0.5 Dodac konwersje tekstu na krzywe.
- [ ] 10.0.6 Dodac tekst na warstwie engrave.
- [ ] 10.0.7 Dodac tekst parametryczny z wartosci parametrow.
- [ ] 10.0.8 Dodac test eksportu tekstu.

## 11.0 Podglad 3D

- [ ] 11.0.0 Utworzyc model czesci 3D na bazie konturu 2D.
- [ ] 11.0.1 Dodac extrusion o grubosc materialu.
- [ ] 11.0.2 Dodac generowanie mesh dla prostokata.
- [ ] 11.0.3 Dodac generowanie mesh dla polygonu.
- [ ] 11.0.4 Dodac material wizualny sklejki.
- [ ] 11.0.5 Dodac widok zlozonego pudelka.
- [ ] 11.0.6 Dodac animacje skladania.
- [ ] 11.0.7 Dodac animacje rozkladania.
- [ ] 11.0.8 Dodac prosta detekcje kolizji elementow.
- [ ] 11.0.9 Dodac test/manual QA dla podgladu 3D.

## 12.0 Produkcja i nesting

- [ ] 12.0.0 Utworzyc `SheetSize`.
- [ ] 12.0.1 Dodac szerokosc arkusza.
- [ ] 12.0.2 Dodac wysokosc arkusza.
- [ ] 12.0.3 Dodac margines arkusza.
- [ ] 12.0.4 Utworzyc `NestingOptions`.
- [ ] 12.0.5 Dodac odstep miedzy elementami.
- [ ] 12.0.6 Dodac mozliwosc obrotu elementow.
- [ ] 12.0.7 Dodac prosty nesting rzedami.
- [ ] 12.0.8 Dodac statystyke zuzycia materialu.
- [ ] 12.0.9 Dodac statystyke dlugosci ciecia.
- [ ] 12.0.10 Dodac szacowany czas ciecia.
- [ ] 12.0.11 Dodac test nestingu kilku prostokatow.

## 12.1 Manufacturing checks

- [ ] 12.1.0 Utworzyc `ManufacturingCheck`.
- [ ] 12.1.1 Dodac poziomy: info, warning, error.
- [ ] 12.1.2 Dodac wykrywanie podwojnych linii.
- [ ] 12.1.3 Dodac wykrywanie otwartych konturow.
- [ ] 12.1.4 Dodac wykrywanie zbyt malych odstepow.
- [ ] 12.1.5 Dodac wykrywanie zbyt cienkich mostkow.
- [ ] 12.1.6 Dodac sugestie kolejnosci ciecia.
- [ ] 12.1.7 Dodac test podwojnej linii.
- [ ] 12.1.8 Dodac test otwartego konturu.
- [ ] 12.1.9 Dodac test zbyt malego odstepu.

## 13.0 Biblioteka

- [ ] 13.0.0 Utworzyc katalog biblioteki materialow.
- [ ] 13.0.1 Dodac profil sklejki 3 mm.
- [ ] 13.0.2 Dodac profil sklejki 4 mm.
- [ ] 13.0.3 Dodac profil MDF.
- [ ] 13.0.4 Dodac profil akrylu.
- [ ] 13.0.5 Utworzyc katalog szablonow.
- [ ] 13.0.6 Dodac szablon pudelka.
- [ ] 13.0.7 Dodac szablon organizera.
- [ ] 13.0.8 Dodac szablon podstawki.
- [ ] 13.0.9 Dodac szablon stojaka.
- [ ] 13.0.10 Dodac UI wyboru szablonu.
- [ ] 13.0.11 Dodac test ladowania biblioteki.

## 14.0 Pluginy

- [ ] 14.0.0 Zdefiniowac minimalne API pluginu.
- [ ] 14.0.1 Dodac kontrakt generatora pluginowego.
- [ ] 14.0.2 Dodac kontrakt eksportera pluginowego.
- [ ] 14.0.3 Dodac kontrakt narzedzia pluginowego.
- [ ] 14.0.4 Dodac manifest pluginu.
- [ ] 14.0.5 Dodac ladowanie pluginu z katalogu.
- [ ] 14.0.6 Dodac izolacje bledow pluginu.
- [ ] 14.0.7 Dodac przykladowy plugin generatora.
- [ ] 14.0.8 Dodac przykladowy plugin eksportera.
- [ ] 14.0.9 Dodac test ladowania manifestu.

## 15.0 Przygotowanie wersji 1.0

- [ ] 15.0.0 Ustalic minimalne wymagania systemowe.
- [ ] 15.0.1 Utworzyc instalator.
- [ ] 15.0.2 Dodac auto update.
- [ ] 15.0.3 Dodac ekran ustawien.
- [ ] 15.0.4 Dodac konfiguracje skrotow klawiszowych.
- [ ] 15.0.5 Dodac dokumentacje uzytkownika.
- [ ] 15.0.6 Dodac tutorial tworzenia pierwszego pudelka.
- [ ] 15.0.7 Dodac tutorial kalibracji kerfu.
- [ ] 15.0.8 Dodac przykladowe projekty.
- [ ] 15.0.9 Zamrozic wersje formatu pliku.
- [ ] 15.0.10 Dodac testy regresyjne dla przykladowych projektow.
- [ ] 15.0.11 Dodac checklist QA przed wydaniem.
- [ ] 15.0.12 Przygotowac release notes.

## MVP.0 Minimalna sciezka do pierwszej uzywalnej wersji

- [ ] MVP.0.0 Zbudowac solution i projekty domenowe.
- [ ] MVP.0.1 Zaimplementowac `Length`, `Point2D`, `Vector2D`, `BoundingBox`, `Matrix3x3`.
- [ ] MVP.0.2 Zaimplementowac podstawowe encje: line, rectangle, circle, polyline.
- [ ] MVP.0.3 Zaimplementowac parametry: width, depth, height, material thickness, kerf, finger width, clearance.
- [ ] MVP.0.4 Zaimplementowac prosty graf zaleznosci parametrow.
- [ ] MVP.0.5 Zaimplementowac `CadDocument`, `Sketch`, `Layer`, `MaterialProfile`.
- [ ] MVP.0.6 Zaimplementowac zapis i odczyt JSON.
- [ ] MVP.0.7 Zaimplementowac generator finger joint dla prostych krawedzi.
- [ ] MVP.0.8 Zaimplementowac generator otwartego pudelka.
- [ ] MVP.0.9 Zaimplementowac kompensacje kerfu dla prostych konturow.
- [ ] MVP.0.10 Zaimplementowac eksport SVG.
- [ ] MVP.0.11 Zaimplementowac prosty widok Unity 2D.
- [ ] MVP.0.12 Zaimplementowac panel zmiany parametrow pudelka.
- [ ] MVP.0.13 Potwierdzic, ze zmiana parametru przebudowuje pudelko.
- [ ] MVP.0.14 Wyeksportowac SVG pudelka testowego.
- [ ] MVP.0.15 Otworzyc SVG w zewnetrznym programie i potwierdzic skale w mm.
- [ ] MVP.0.16 Uruchomic komplet testow jednostkowych.

## MVP.1 Kryteria akceptacji MVP

- [ ] MVP.1.0 Uzytkownik moze wybrac profil materialu.
- [ ] MVP.1.1 Uzytkownik moze ustawic grubosc materialu.
- [ ] MVP.1.2 Uzytkownik moze ustawic kerf.
- [ ] MVP.1.3 Uzytkownik moze ustawic szerokosc, glebokosc i wysokosc pudelka.
- [ ] MVP.1.4 Uzytkownik widzi geometrie 2D wygenerowanego pudelka.
- [ ] MVP.1.5 Uzytkownik zmienia parametr i widzi przebudowe modelu.
- [ ] MVP.1.6 Uzytkownik eksportuje poprawny SVG w milimetrach.
- [ ] MVP.1.7 SVG zawiera warstwy cut i engrave, jesli sa uzyte.
- [ ] MVP.1.8 Testy jednostkowe przechodza lokalnie.
- [ ] MVP.1.9 Dokumentacja opisuje minimalny workflow: material -> parametry -> generuj -> eksportuj.
