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

- [x] 0.6.0 Ustalic minimalna skladnie wyrazen, np. `Width - 2 * MaterialThickness`.
- [x] 0.6.1 Utworzyc reprezentacje `Expression`.
- [x] 0.6.2 Dodac wyrazenie stale.
- [x] 0.6.3 Dodac wyrazenie referencji do parametru.
- [x] 0.6.4 Dodac operacje dodawania.
- [x] 0.6.5 Dodac operacje odejmowania.
- [x] 0.6.6 Dodac operacje mnozenia.
- [x] 0.6.7 Dodac operacje dzielenia.
- [x] 0.6.8 Dodac ewaluator wyrazen.
- [x] 0.6.9 Dodac obsluge brakujacego parametru.
- [x] 0.6.10 Dodac obsluge dzielenia przez zero.
- [x] 0.6.11 Dodac parser prostych wyrazen tekstowych albo zostawic jawne budowanie AST w MVP.
- [x] 0.6.12 Dodac test: `Width - 2 * MaterialThickness`.
- [x] 0.6.13 Dodac test: aktualizacja parametru zmienia wynik wyrazenia.
- [x] 0.6.14 Dodac test: brakujaca referencja zwraca czytelny blad.

## 0.7 Graf zaleznosci parametrow

- [x] 0.7.0 Utworzyc `DependencyGraph`.
- [x] 0.7.1 Dodac rejestrowanie zaleznosci parametru od innych parametrow.
- [x] 0.7.2 Dodac wyliczanie kolejnosci przeliczania.
- [x] 0.7.3 Dodac wykrywanie cyklu zaleznosci.
- [x] 0.7.4 Dodac wynik bledu dla cyklu zaleznosci.
- [x] 0.7.5 Dodac liste parametrow dotknietych zmiana.
- [x] 0.7.6 Dodac test prostego lancucha zaleznosci.
- [x] 0.7.7 Dodac test wielu zaleznosci jednego parametru.
- [x] 0.7.8 Dodac test wykrywania cyklu.

## 0.8 Model dokumentu

- [x] 0.8.0 Utworzyc `CadDocument`.
- [x] 0.8.1 Dodac identyfikator dokumentu.
- [x] 0.8.2 Dodac nazwe dokumentu.
- [x] 0.8.3 Dodac wersje formatu pliku.
- [x] 0.8.4 Dodac kolekcje parametrow dokumentu.
- [x] 0.8.5 Dodac kolekcje warstw.
- [x] 0.8.6 Dodac kolekcje szkicow.
- [x] 0.8.7 Dodac kolekcje generatorow.
- [x] 0.8.8 Dodac profil materialu dokumentu.
- [x] 0.8.9 Utworzyc `Sketch`.
- [x] 0.8.10 Dodac kolekcje encji w szkicu.
- [x] 0.8.11 Utworzyc bazowy kontrakt `Entity`.
- [x] 0.8.12 Utworzyc `GeneratorInstance`.
- [x] 0.8.13 Dodac powiazanie generatora z parametrami.
- [x] 0.8.14 Dodac test tworzenia pustego dokumentu.
- [x] 0.8.15 Dodac test dodawania szkicu.
- [x] 0.8.16 Dodac test dodawania parametrow dokumentu.

## 0.9 Warstwy i profile materialow

- [x] 0.9.0 Utworzyc enum `LayerRole`.
- [x] 0.9.1 Dodac role: cut, engrave, score, ignore.
- [x] 0.9.2 Utworzyc `Layer`.
- [x] 0.9.3 Dodac nazwe warstwy.
- [x] 0.9.4 Dodac kolor warstwy.
- [x] 0.9.5 Dodac role warstwy.
- [x] 0.9.6 Dodac domyslne warstwy dokumentu.
- [x] 0.9.7 Utworzyc `MaterialProfile`.
- [x] 0.9.8 Dodac nazwe materialu.
- [x] 0.9.9 Dodac grubosc materialu.
- [x] 0.9.10 Dodac domyslny kerf.
- [x] 0.9.11 Dodac domyslny clearance.
- [x] 0.9.12 Dodac minimalna szerokosc palca.
- [x] 0.9.13 Dodac profile: sklejka 3 mm, sklejka 4 mm, MDF, akryl.
- [x] 0.9.14 Dodac test domyslnych warstw.
- [x] 0.9.15 Dodac test domyslnych profili materialow.

## 0.10 Zapis i odczyt projektu

- [x] 0.10.0 Ustalic format pliku projektu, np. JSON.
- [x] 0.10.1 Utworzyc `DocumentSerializer`.
- [x] 0.10.2 Dodac serializacje pustego dokumentu.
- [x] 0.10.3 Dodac deserializacje pustego dokumentu.
- [x] 0.10.4 Dodac serializacje parametrow.
- [x] 0.10.5 Dodac serializacje warstw.
- [x] 0.10.6 Dodac serializacje profilu materialu.
- [x] 0.10.7 Dodac pole wersji formatu.
- [x] 0.10.8 Dodac blad dla nieobslugiwanej wersji.
- [x] 0.10.9 Dodac test round-trip pustego dokumentu.
- [x] 0.10.10 Dodac test round-trip dokumentu z parametrami i warstwami.

## 1.0 Linie i odcinki

- [x] 1.0.0 Utworzyc `Line2D` jako linie nieskonczona albo kontrakt pomocniczy.
- [x] 1.0.1 Utworzyc `LineSegment2D`.
- [x] 1.0.2 Dodac punkt poczatkowy odcinka.
- [x] 1.0.3 Dodac punkt koncowy odcinka.
- [x] 1.0.4 Dodac dlugosc odcinka.
- [x] 1.0.5 Dodac punkt posredni dla parametru `t` od 0 do 1.
- [x] 1.0.6 Dodac kierunek odcinka.
- [x] 1.0.7 Dodac bounding box odcinka.
- [x] 1.0.8 Dodac transformacje odcinka.
- [x] 1.0.9 Dodac test dlugosci.
- [x] 1.0.10 Dodac test punktu posredniego.
- [x] 1.0.11 Dodac test transformacji odcinka.

## 1.1 Przeciecia

- [x] 1.1.0 Utworzyc typ wyniku przeciecia.
- [x] 1.1.1 Obsluzyc brak przeciecia.
- [x] 1.1.2 Obsluzyc jedno przeciecie.
- [x] 1.1.3 Obsluzyc odcinki rownolegle.
- [x] 1.1.4 Obsluzyc odcinki wspolliniowe.
- [x] 1.1.5 Dodac przeciecie linia-linia.
- [x] 1.1.6 Dodac przeciecie odcinek-odcinek.
- [x] 1.1.7 Dodac przeciecie linia-odcinek.
- [x] 1.1.8 Dodac test przeciecia krzyzowego.
- [x] 1.1.9 Dodac test braku przeciecia.
- [x] 1.1.10 Dodac test przeciecia na koncu odcinka.
- [x] 1.1.11 Dodac test odcinkow wspolliniowych.

## 1.2 Okregi i luki

- [x] 1.2.0 Utworzyc `Circle2D`.
- [x] 1.2.1 Dodac srodek okregu.
- [x] 1.2.2 Dodac promien okregu.
- [x] 1.2.3 Dodac obwod okregu.
- [x] 1.2.4 Dodac bounding box okregu.
- [x] 1.2.5 Utworzyc `Arc2D`.
- [x] 1.2.6 Dodac srodek luku.
- [x] 1.2.7 Dodac promien luku.
- [x] 1.2.8 Dodac kat poczatkowy i koncowy.
- [x] 1.2.9 Dodac kierunek luku.
- [x] 1.2.10 Dodac dlugosc luku.
- [x] 1.2.11 Dodac punkt na luku dla parametru `t`.
- [x] 1.2.12 Dodac transformacje okregu i luku.
- [x] 1.2.13 Dodac testy okregu.
- [x] 1.2.14 Dodac testy luku.

## 1.3 Polilinie i polygony

- [x] 1.3.0 Utworzyc `Polyline2D`.
- [x] 1.3.1 Dodac liste punktow polilinii.
- [x] 1.3.2 Dodac informacje, czy polilinia jest zamknieta.
- [x] 1.3.3 Dodac dlugosc polilinii.
- [x] 1.3.4 Dodac bounding box polilinii.
- [x] 1.3.5 Dodac transformacje polilinii.
- [x] 1.3.6 Utworzyc `Polygon2D`.
- [x] 1.3.7 Dodac pole powierzchni polygonu.
- [x] 1.3.8 Dodac kierunek polygonu: clockwise/counter-clockwise.
- [x] 1.3.9 Dodac test zamknietej polilinii.
- [x] 1.3.10 Dodac test pola prostokata.
- [x] 1.3.11 Dodac test kierunku polygonu.
- [x] 1.3.12 Dodac bounding box polygonu.
- [x] 1.3.13 Dodac transformacje polygonu.

## 1.4 Offsety i kontury

- [x] 1.4.0 Utworzyc kontrakt `Contour2D`.
- [x] 1.4.1 Dodac wykrywanie, czy kontur jest domkniety.
- [x] 1.4.2 Dodac wykrywanie samoprzeciec dla prostych polygonow.
- [x] 1.4.3 Dodac prosty offset polygonu wypuklego.
- [x] 1.4.4 Dodac offset zewnetrzny.
- [x] 1.4.5 Dodac offset wewnetrzny.
- [x] 1.4.6 Dodac test offsetu kwadratu na zewnatrz.
- [x] 1.4.7 Dodac test offsetu kwadratu do wewnatrz.
- [x] 1.4.8 Oznaczyc ograniczenia offsetu MVP w dokumentacji.
- [x] 1.4.9 Dodac test odrzucenia polygonu niewypuklego przez offset MVP.

## 2.0 Encje szkicu

- [x] 2.0.0 Utworzyc bazowy interfejs `ISketchEntity`.
- [x] 2.0.1 Dodac identyfikator encji.
- [x] 2.0.2 Dodac identyfikator warstwy encji.
- [x] 2.0.3 Dodac bounding box encji.
- [x] 2.0.4 Dodac transformacje encji.
- [x] 2.0.5 Utworzyc `LineEntity`.
- [x] 2.0.6 Utworzyc `RectangleEntity`.
- [x] 2.0.7 Utworzyc `CircleEntity`.
- [x] 2.0.8 Utworzyc `ArcEntity`.
- [x] 2.0.9 Utworzyc `PolylineEntity`.
- [x] 2.0.10 Utworzyc `TextEntity` jako placeholder.
- [x] 2.0.11 Dodac test tworzenia kazdej encji.

## 2.1 Operacje na szkicu

- [x] 2.1.0 Dodac metode dodawania encji do szkicu.
- [x] 2.1.1 Dodac metode usuwania encji ze szkicu.
- [x] 2.1.2 Dodac metode kopiowania encji.
- [x] 2.1.3 Dodac przesuwanie encji.
- [x] 2.1.4 Dodac obracanie encji.
- [x] 2.1.5 Dodac skalowanie encji.
- [x] 2.1.6 Dodac odbicie encji.
- [x] 2.1.7 Dodac test dodania i usuniecia encji.
- [x] 2.1.8 Dodac test kopiowania encji.
- [x] 2.1.9 Dodac test transformacji encji.

## 2.2 Relacje szkicu z parametrami

- [x] 2.2.0 Dodac mozliwosc przypisania wymiaru encji do parametru.
- [x] 2.2.1 Dodac aktualizacje prostokata po zmianie parametru szerokosci.
- [x] 2.2.2 Dodac aktualizacje prostokata po zmianie parametru wysokosci.
- [x] 2.2.3 Dodac aktualizacje okregu po zmianie parametru srednicy.
- [x] 2.2.4 Dodac test przebudowy szkicu po zmianie parametru.
- [x] 2.2.5 Opisac ograniczenia parametrycznego szkicu w MVP.

## 2.3 Trwalosc szkicow

- [x] 2.3.0 Ustalic kontrakt JSON dla szkicow i encji.
- [x] 2.3.1 Dodac serializacje `LineEntity`.
- [x] 2.3.2 Dodac serializacje `RectangleEntity`.
- [x] 2.3.3 Dodac serializacje `CircleEntity`.
- [x] 2.3.4 Dodac serializacje `ArcEntity`.
- [x] 2.3.5 Dodac serializacje `PolylineEntity`.
- [x] 2.3.6 Dodac serializacje `TextEntity` placeholder.
- [x] 2.3.7 Dodac test round-trip dokumentu ze szkicem i encjami.
- [x] 2.3.8 Dodac serializacje powiazan wymiarow encji z parametrami.

## 3.0 Integracja Unity - baza

- [x] 3.0.0 Utworzyc projekt Unity `LaserCad.Unity`.
- [x] 3.0.1 Podlaczyc biblioteki domenowe do Unity.
- [x] 3.0.2 Utworzyc scene robocza 2D.
- [x] 3.0.3 Utworzyc glowny kontroler aplikacji.
- [x] 3.0.4 Zaladowac pusty `CadDocument` w Unity.
- [x] 3.0.5 Wyswietlic podstawowe informacje o dokumencie w UI.
- [x] 3.0.6 Zweryfikowac, ze Unity nie zawiera logiki geometrycznej duplikujacej Core.

## 3.1 Kamera

- [x] 3.1.0 Dodac kamere ortograficzna.
- [x] 3.1.1 Dodac zoom kolkiem myszy.
- [x] 3.1.2 Dodac pan srodkowym przyciskiem lub prawym przyciskiem myszy.
- [x] 3.1.3 Dodac reset widoku.
- [x] 3.1.4 Dodac ograniczenia minimalnego i maksymalnego zoomu.
- [x] 3.1.5 Dodac test manualny kamery do checklisty QA.
- [x] 3.1.6 Podpiac kontroler kamery do sceny roboczej.
- [x] 3.1.7 Pokazac projekt Unity w solution jako folder projektu.

## 3.2 Grid

- [x] 3.2.0 Utworzyc renderer siatki.
- [x] 3.2.1 Dodac siatke 1 mm.
- [x] 3.2.2 Dodac mocniejsza linie co 5 mm.
- [x] 3.2.3 Dodac mocniejsza linie co 10 mm.
- [x] 3.2.4 Dodac dopasowanie grubosci linii do zoomu.
- [x] 3.2.5 Dodac mozliwosc wlaczenia i wylaczenia siatki.
- [x] 3.2.6 Podpiac renderer siatki do sceny roboczej.

## 3.3 Snap

- [x] 3.3.0 Utworzyc `SnapService`.
- [x] 3.3.1 Dodac snap do siatki.
- [x] 3.3.2 Dodac snap do punktow encji.
- [x] 3.3.3 Dodac snap do srodka okregu/prostokata.
- [x] 3.3.4 Dodac snap do koncow linii.
- [x] 3.3.5 Dodac priorytety snapowania.
- [x] 3.3.6 Dodac wizualny marker snapu.
- [x] 3.3.7 Dodac test manualny snapu do checklisty QA.

## 3.4 Zaznaczanie

- [x] 3.4.0 Utworzyc `SelectionService`.
- [x] 3.4.1 Dodac zaznaczanie kliknieciem.
- [x] 3.4.2 Dodac odznaczanie kliknieciem w puste miejsce.
- [x] 3.4.3 Dodac multi-select z klawiszem modyfikatora.
- [x] 3.4.4 Dodac zaznaczanie prostokatem.
- [x] 3.4.5 Dodac highlight zaznaczonych encji.
- [x] 3.4.6 Dodac panel wlasciwosci zaznaczenia.
- [x] 3.4.7 Podpiac zaznaczanie do sceny roboczej.
- [x] 3.4.8 Pokazac prostokat zaznaczania podczas przeciagania.
- [x] 3.4.9 Poprawic hit-test klikniecia dla linii, prostokatow i polilinii.
- [x] 3.4.10 Poprawic czytelnosc highlightu zaznaczenia na kolorowej geometrii.

## 3.5 GUI funkcji domenowych

- [x] 3.5.0 Dodac renderer geometrii dokumentu 2D w Unity.
- [x] 3.5.1 Dodac startowy dokument demonstracyjny z podstawowymi encjami.
- [x] 3.5.2 Dodac panel parametrow generatora pudelka.
- [x] 3.5.3 Dodac przebudowe podgladu po zmianie parametrow pudelka.
  Uwaga: ten task powinien bazowac na domenowym `BoxGenerator` z sekcji `8.1`, zeby UI nie duplikowalo logiki geometrii.
- [x] 3.5.4 Dodac panel eksportu SVG.
- [x] 3.5.5 Dodac panel eksportu DXF.
- [x] 3.5.6 Dodac panel profilu materialu i warstw.
- [x] 3.5.7 Dodac panel historii undo/redo i komend edycyjnych MVP.
- [x] 3.5.8 Dodac panel informacji o constraints i dimensions MVP.
- [x] 3.5.9 Dodac manualna checkliste GUI funkcji domenowych.

## 3.6 Windows shell + Unity viewport

- [x] 3.6.0 Podjac decyzje architektoniczna: glowna aplikacja Windows, Unity jako osobny proces viewportu.
- [x] 3.6.1 Wybrac technologie desktop shell MVP: WPF.
- [x] 3.6.2 Utworzyc projekt `LaserCad.Desktop`.
- [x] 3.6.3 Dodac `LaserCad.Desktop` do solution.
- [x] 3.6.4 Utworzyc glowne okno Windows z menu `File`, `Edit`, `View`, `Export`, `Help`.
- [x] 3.6.5 Dodac bazowy toolbar z akcjami: nowy, otworz, zapisz, eksport SVG, eksport DXF.
- [x] 3.6.6 Dodac dockowane panele shell: parametry pudelka, material/warstwy, historia, properties.
- [x] 3.6.7 Przeniesc kontrolki generatora pudelka z IMGUI Unity do desktop shell.
- [x] 3.6.8 Przeniesc eksport SVG/DXF z IMGUI Unity do desktop shell.
- [x] 3.6.9 Przeniesc wybor profilu materialu i liste warstw z IMGUI Unity do desktop shell.
- [x] 3.6.10 Zdefiniowac kontrakt IPC `LaserCad.ViewportContract` dla komunikacji shell -> viewport.
- [x] 3.6.11 Dodac komunikat IPC wyslania aktualnego dokumentu do viewportu.
- [x] 3.6.12 Dodac komunikat IPC zmiany widoku: reset, zoom to fit, grid on/off.
- [x] 3.6.13 Dodac komunikat IPC zaznaczenia encji z viewportu do shell.
- [x] 3.6.14 Utworzyc tryb Unity viewport process uruchamiany z argumentem `--viewport`.
- [x] 3.6.15 Uruchamiac proces Unity viewport z desktop shell.
- [x] 3.6.16 Obslugiwac zamkniecie i restart procesu Unity viewport z desktop shell.
- [x] 3.6.17 Wysylac przebudowany dokument pudelka z desktop shell do Unity viewport.
- [x] 3.6.18 Usunac albo zdegradowac panele IMGUI Unity do trybu debug viewportu.
- [x] 3.6.19 Dodac build desktop shell do `build.bat`.
- [x] 3.6.20 Dodac pakowanie Unity playera obok aplikacji desktop shell.
- [x] 3.6.21 Dodac manualna checkliste QA dla desktop shell + Unity viewport.
- [x] 3.6.22 Udokumentowac lifecycle procesu viewportu i granice odpowiedzialnosci shell/viewport.
- [x] 3.6.23 Osadzic Unity viewport w glownym oknie desktop shell i ukryc techniczna kontrole procesu.
- [x] 3.6.24 Poprawic fokus osadzonego viewportu po najechaniu i kliknieciu.
- [x] 3.6.25 Przywracac fokus viewportu po minimalizacji i maksymalizacji okna.
- [x] 3.6.26 Przekazywac scroll myszy do viewportu, gdy kursor jest nad panelem Unity.
- [ ] 3.6.27 Naprawic znany problem: po minimalizacji i maksymalizacji okna scroll/zoom viewportu nadal potrafi nie dzialac bez dodatkowej interakcji.

## 3.7 Edycja ksztaltow w desktop shell

- [x] 3.7.0 Dodac przyciski dodawania podstawowych ksztaltow w toolbarze desktop shell.
- [x] 3.7.1 Dodac dodawanie prostokata do aktualnego szkicu i wysylke dokumentu do viewportu.
- [x] 3.7.2 Dodac dodawanie linii do aktualnego szkicu i wysylke dokumentu do viewportu.
- [x] 3.7.3 Dodac dodawanie okregu do aktualnego szkicu i wysylke dokumentu do viewportu.
- [x] 3.7.4 Dodac usuwanie zaznaczonych encji odczytanych z viewportu.
- [x] 3.7.5 Dodac przesuwanie zaznaczonych encji z panelu transformacji.
- [x] 3.7.6 Dodac obracanie zaznaczonych encji z panelu transformacji.
- [x] 3.7.7 Dodac skalowanie zaznaczonych encji z panelu transformacji.
- [x] 3.7.8 Podpiac undo/redo dla operacji edycji ksztaltow.
- [x] 3.7.9 Dodac rysowanie ksztaltow kliknieciami bezposrednio w viewportcie.
- [x] 3.7.10 Dodac natychmiastowy podglad rysowanego ksztaltu w viewportcie.
- [x] 3.7.11 Odswiezac viewport po usunieciu i edycji z toolbaru bez recznej zmiany focusu.
- [x] 3.7.12 Obslugiwac rysowanie click-drag-release oraz click-click bez ramki zaznaczenia.
- [x] 3.7.13 Ukrywac systemowy kursor podczas rysowania w viewportcie.
- [x] 3.7.14 Ukrywac kursor viewportu od razu po wybraniu narzedzia rysowania w shellu.

## 3.8 Wyglad desktop shell

- [x] 3.8.0 Uladnic desktop shell w jasnym stylu narzedzia CAD.
- [x] 3.8.1 Zastapic tekstowe przyciski toolbaru ikonami z tooltipami.
- [x] 3.8.2 Dodac loader viewportu podczas startu Unity.

## 3.9 Stan projektu w desktop shell

- [x] 3.9.0 Podpiac akcje `New`, `Open`, `Save` i `Save As` w menu oraz toolbarze.
- [x] 3.9.1 Zapisywac aktualny dokument projektu przez `DocumentSerializer`.
- [x] 3.9.2 Wczytywac dokument projektu z pliku JSON i resetowac historie undo/redo.
- [x] 3.9.3 Po wczytaniu wysylac dokument do viewportu i odswiezac panele shell.
- [x] 3.9.4 Pamietac aktualna sciezke projektu dla kolejnych zapisow `Save`.

## 3.10 Porzadek UI i przestrzen robocza

- [ ] 3.10.0 Ustalic domyslny uklad shell z maksymalna przestrzenia robocza viewportu.
- [ ] 3.10.1 Ukryc domyslnie panele generatorow i pokazywac je dopiero po kliknieciu jawnej akcji.
- [ ] 3.10.2 Ukryc domyslnie panele konfiguracji zaawansowanej i udostepnic je przez zwijany panel, zakladke albo menu.
- [ ] 3.10.3 Ukryc domyslnie panel historii i udostepnic go przez przycisk albo pozycje menu.
- [ ] 3.10.4 Zapewnic szybki powrot do czystego widoku roboczego jednym kliknieciem.
- [ ] 3.10.5 Zapamietywac preferowany stan widocznosci paneli miedzy uruchomieniami aplikacji.
- [ ] 3.10.6 Zrobic audyt wszystkich widocznych elementow UI: menu, toolbaru, paneli, przyciskow i pol formularzy.
- [ ] 3.10.7 Zaimplementowac wszystkie placeholdery UI, ktore sa obecnie widoczne jako realne akcje.
- [ ] 3.10.8 Doprowadzic wszystkie widoczne akcje UI do dzialania end-to-end.
- [ ] 3.10.9 Usuwac z widoku tylko te funkcje, ktore sa swiadomie poza zakresem aktualnej wersji, zamiast zostawiac niedzialajace kontrolki.
- [ ] 3.10.10 Dodac manualna checkliste QA dla domyslnie czystego UI i wszystkich widocznych akcji.

## 4.0 Eksport SVG - fundament

- [x] 4.0.0 Utworzyc `SvgExportOptions`.
- [x] 4.0.1 Dodac jednostke eksportu: mm.
- [x] 4.0.2 Dodac ustawienie grubosci linii.
- [x] 4.0.3 Dodac ustawienie eksportowanych warstw.
- [x] 4.0.4 Utworzyc `SvgExporter`.
- [x] 4.0.5 Dodac eksport pustego dokumentu.
- [x] 4.0.6 Dodac poprawny `viewBox`.
- [x] 4.0.7 Dodac test snapshot dla pustego SVG.

## 4.1 Eksport SVG - encje

- [x] 4.1.0 Dodac eksport `LineEntity` jako `line`.
- [x] 4.1.1 Dodac eksport `RectangleEntity` jako `rect` albo `path`.
- [x] 4.1.2 Dodac eksport `CircleEntity` jako `circle`.
- [x] 4.1.3 Dodac eksport `ArcEntity` jako `path`.
- [x] 4.1.4 Dodac eksport `PolylineEntity` jako `polyline` albo `path`.
- [x] 4.1.5 Dodac mapowanie koloru warstwy.
- [x] 4.1.6 Dodac pomijanie warstwy `ignore`.
- [x] 4.1.7 Dodac test eksportu kazdej encji.
- [x] 4.1.8 Dodac test eksportu warstw.

## 4.2 Eksport DXF

- [x] 4.2.0 Utworzyc `DxfExportOptions`.
- [x] 4.2.1 Utworzyc `DxfExporter`.
- [x] 4.2.2 Dodac eksport `LINE`.
- [x] 4.2.3 Dodac eksport `CIRCLE`.
- [x] 4.2.4 Dodac eksport `ARC`.
- [x] 4.2.5 Dodac eksport `LWPOLYLINE`.
- [x] 4.2.6 Dodac warstwy DXF.
- [x] 4.2.7 Dodac test podstawowego pliku DXF.

## 5.0 Komendy edycyjne

- [x] 5.0.0 Utworzyc interfejs `ICommand`.
- [x] 5.0.1 Dodac metode wykonania komendy.
- [x] 5.0.2 Dodac metode cofniecia komendy.
- [x] 5.0.3 Utworzyc `MoveCommand`.
- [x] 5.0.4 Utworzyc `RotateCommand`.
- [x] 5.0.5 Utworzyc `ScaleCommand`.
- [x] 5.0.6 Utworzyc `MirrorCommand`.
- [x] 5.0.7 Utworzyc `DeleteCommand`.
- [x] 5.0.8 Utworzyc `AddEntityCommand`.
- [x] 5.0.9 Dodac test wykonania kazdej komendy.
- [x] 5.0.10 Dodac test cofniecia kazdej komendy.

## 5.1 Undo i redo

- [x] 5.1.0 Utworzyc `UndoRedoStack`.
- [x] 5.1.1 Dodac stos undo.
- [x] 5.1.2 Dodac stos redo.
- [x] 5.1.3 Dodac czyszczenie redo po nowej komendzie.
- [x] 5.1.4 Dodac limit historii.
- [x] 5.1.5 Dodac grupowanie komend.
- [x] 5.1.6 Dodac test undo.
- [x] 5.1.7 Dodac test redo.
- [x] 5.1.8 Dodac test czyszczenia redo.

## 5.2 Feature tree

- [x] 5.2.0 Utworzyc model elementu drzewa historii.
- [x] 5.2.1 Dodac wpis dla generatora.
- [x] 5.2.2 Dodac wpis dla operacji edycyjnej.
- [x] 5.2.3 Dodac aktywowanie/dezaktywowanie operacji.
- [x] 5.2.4 Dodac przebudowe dokumentu od feature tree.
- [x] 5.2.5 Dodac test prostego drzewa: generator + move.

## 6.0 Constraints

- [x] 6.0.0 Utworzyc bazowy kontrakt constraintu.
- [x] 6.0.1 Dodac `HorizontalConstraint`.
- [x] 6.0.2 Dodac `VerticalConstraint`.
- [x] 6.0.3 Dodac `ParallelConstraint`.
- [x] 6.0.4 Dodac `PerpendicularConstraint`.
- [x] 6.0.5 Dodac `CoincidentConstraint`.
- [x] 6.0.6 Dodac `EqualConstraint`.
- [x] 6.0.7 Dodac prosty solver dla ograniczonego zestawu przypadkow MVP.
- [x] 6.0.8 Dodac test horizontal.
- [x] 6.0.9 Dodac test vertical.
- [x] 6.0.10 Dodac test coincident.

## 6.1 Dimensions

- [x] 6.1.0 Utworzyc `Dimension`.
- [x] 6.1.1 Dodac wymiar dlugosci odcinka.
- [x] 6.1.2 Dodac wymiar szerokosci prostokata.
- [x] 6.1.3 Dodac wymiar wysokosci prostokata.
- [x] 6.1.4 Dodac wymiar srednicy okregu.
- [x] 6.1.5 Dodac wymiar promienia okregu.
- [x] 6.1.6 Powiazac wymiar z parametrem.
- [x] 6.1.7 Dodac test zmiany wymiaru przez parametr.

## 7.0 Finger joint - model danych

- [x] 7.0.0 Utworzyc `FingerJointOptions`.
- [x] 7.0.1 Dodac szerokosc palca.
- [x] 7.0.2 Dodac minimalna szerokosc palca.
- [x] 7.0.3 Dodac maksymalna szerokosc palca.
- [x] 7.0.4 Dodac flage zaczynania od zeba.
- [x] 7.0.5 Dodac flage konczenia zebem.
- [x] 7.0.6 Dodac tryb ciasny/neutralny/luzny.
- [x] 7.0.7 Dodac kerf i clearance.
- [x] 7.0.8 Dodac walidacje opcji.
- [x] 7.0.9 Dodac test walidacji opcji.

## 7.1 Finger joint - algorytm

- [x] 7.1.0 Utworzyc `FingerJointGenerator`.
- [x] 7.1.1 Dodac podzial krawedzi na segmenty.
- [x] 7.1.2 Dodac dobor liczby zebow.
- [x] 7.1.3 Dodac wymuszanie symetrii.
- [x] 7.1.4 Dodac generowanie profilu krawedzi.
- [x] 7.1.5 Dodac kompensacje grubosci materialu.
- [x] 7.1.6 Dodac kompensacje kerfu.
- [x] 7.1.7 Dodac kompensacje clearance.
- [x] 7.1.8 Dodac test krawedzi 100 mm.
- [x] 7.1.9 Dodac test symetrii.
- [x] 7.1.10 Dodac test zaczynania od zeba.
- [x] 7.1.11 Dodac test zaczynania od wciecia.

## 8.0 Generator pudelka

- [x] 8.0.0 Utworzyc `BoxGeneratorOptions`.
- [x] 8.0.1 Dodac `Width`.
- [x] 8.0.2 Dodac `Depth`.
- [x] 8.0.3 Dodac `Height`.
- [x] 8.0.4 Dodac `MaterialThickness`.
- [x] 8.0.5 Dodac `Kerf`.
- [x] 8.0.6 Dodac `FingerWidth`.
- [x] 8.0.7 Dodac `Clearance`.
- [x] 8.0.8 Dodac typ pudelka: zamkniete, otwarte, z pokrywa.
- [x] 8.0.9 Dodac walidacje wymiarow.
- [x] 8.0.10 Dodac test walidacji zbyt malego pudelka.

## 8.1 Generator pudelka - geometria

- [x] 8.1.0 Utworzyc `BoxGenerator`.
- [x] 8.1.1 Wygenerowac przednia scianke.
- [x] 8.1.2 Wygenerowac tylna scianke.
- [x] 8.1.3 Wygenerowac lewa scianke.
- [x] 8.1.4 Wygenerowac prawa scianke.
- [x] 8.1.5 Wygenerowac dno.
- [x] 8.1.6 Wygenerowac pokrywe dla trybu z pokrywa.
- [x] 8.1.7 Dodac finger jointy na krawedziach.
- [x] 8.1.8 Rozlozyc elementy na plaszczyznie 2D.
- [x] 8.1.9 Dodac marginesy miedzy elementami.
- [x] 8.1.10 Dodac warstwy cut dla konturow.
- [x] 8.1.11 Dodac test liczby wygenerowanych scianek.
- [x] 8.1.12 Dodac test zmiany `Width` przebudowuje geometrie.
- [x] 8.1.13 Dodac test zmiany `MaterialThickness` przebudowuje finger jointy.

## 8.2 Pozostale generatory

- [x] 8.2.0 Utworzyc wspolny interfejs generatora.
- [x] 8.2.1 Dodac generator tray.
- [x] 8.2.2 Dodac generator organizer.
- [x] 8.2.3 Dodac generator drawer.
- [x] 8.2.4 Dodac generator divider.
- [x] 8.2.5 Dodac generator pegboard.
- [x] 8.2.6 Dodac generator ramki.
- [x] 8.2.7 Dodac generator stojaka/podstawki.
- [x] 8.2.8 Dodac test, ze kazdy generator zwraca poprawny dokument/sketch.

## 9.0 Kerf compensation

- [x] 9.0.0 Utworzyc `KerfCompensationOptions`.
- [x] 9.0.1 Dodac wartosc kerfu.
- [x] 9.0.2 Dodac tryb dodatni.
- [x] 9.0.3 Dodac tryb ujemny.
- [x] 9.0.4 Dodac offset wewnetrzny.
- [x] 9.0.5 Dodac offset zewnetrzny.
- [x] 9.0.6 Dodac klasyfikacje konturu jako wewnetrzny/zewnetrzny.
- [x] 9.0.7 Dodac podglad geometrii przed kompensacja.
- [x] 9.0.8 Dodac podglad geometrii po kompensacji.
- [x] 9.0.9 Dodac test kompensacji kwadratu zewnetrznego.
- [x] 9.0.10 Dodac test kompensacji otworu wewnetrznego.

## 9.1 Kalibracja kerfu

- [x] 9.1.0 Utworzyc generator probnika kerfu.
- [x] 9.1.1 Dodac zestaw szczelin testowych.
- [x] 9.1.2 Dodac opis wartosci na probniku jako grawer.
- [x] 9.1.3 Dodac formularz wpisania pomiaru.
- [x] 9.1.4 Dodac przeliczenie rekomendowanego kerfu.
- [x] 9.1.5 Dodac zapis rekomendacji do profilu materialu.
- [x] 9.1.6 Dodac test obliczenia kerfu z pomiaru.

## 10.0 Tekst i fonty

- [x] 10.0.0 Utworzyc `TextEntity` z tekstem, pozycja i rozmiarem.
- [x] 10.0.1 Dodac wybor fontu.
- [x] 10.0.2 Dodac import fontu.
- [x] 10.0.3 Dodac alignment tekstu.
- [x] 10.0.4 Dodac eksport tekstu jako SVG text dla MVP.
- [x] 10.0.5 Dodac konwersje tekstu na krzywe.
- [x] 10.0.6 Dodac tekst na warstwie engrave.
- [x] 10.0.7 Dodac tekst parametryczny z wartosci parametrow.
- [x] 10.0.8 Dodac test eksportu tekstu.

## 11.0 Podglad 3D

- [x] 11.0.0 Utworzyc model czesci 3D na bazie konturu 2D.
- [x] 11.0.1 Dodac extrusion o grubosc materialu.
- [x] 11.0.2 Dodac generowanie mesh dla prostokata.
- [x] 11.0.3 Dodac generowanie mesh dla polygonu.
- [x] 11.0.4 Dodac material wizualny sklejki.
- [x] 11.0.5 Dodac widok zlozonego pudelka.
- [x] 11.0.6 Dodac animacje skladania.
- [x] 11.0.7 Dodac animacje rozkladania.
- [x] 11.0.8 Dodac prosta detekcje kolizji elementow.
- [x] 11.0.9 Dodac test/manual QA dla podgladu 3D.

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
- [ ] 13.0.3 Utworzyc katalog szablonow.
- [ ] 13.0.4 Dodac szablon pudelka.
- [ ] 13.0.5 Dodac szablon organizera.
- [ ] 13.0.6 Dodac szablon podstawki.
- [ ] 13.0.7 Dodac szablon stojaka.
- [ ] 13.0.8 Dodac UI wyboru szablonu.
- [ ] 13.0.9 Dodac test ladowania biblioteki.

## 15.0 Przygotowanie wersji 1.0

- [ ] 15.0.0 Dodac ekran ustawien.
- [ ] 15.0.1 Dodac konfiguracje skrotow klawiszowych.
- [ ] 15.0.2 Dodac testy regresyjne dla przykladowych projektow.
- [ ] 15.0.3 Dodac checklist QA przed wydaniem.

## Poza aktualnym zakresem

Te elementy sa swiadomie odlozone poza biezacy plan, dopoki nie pojawi sie realna potrzeba produktowa:

- profile biblioteki materialow dla MDF i akrylu,
- system pluginow,
- minimalne wymagania systemowe,
- instalator,
- auto update,
- release notes,
- pelna dokumentacja uzytkownika,
- tutorial tworzenia pierwszego pudelka,
- tutorial kalibracji kerfu,
- przykladowe projekty,
- zamrozenie wersji formatu pliku.

## MVP.0 Minimalna sciezka do pierwszej uzywalnej wersji

- [x] MVP.0.0 Zbudowac solution i projekty domenowe.
- [x] MVP.0.1 Zaimplementowac `Length`, `Point2D`, `Vector2D`, `BoundingBox`, `Matrix3x3`.
- [x] MVP.0.2 Zaimplementowac podstawowe encje: line, rectangle, circle, polyline.
- [x] MVP.0.3 Zaimplementowac parametry: width, depth, height, material thickness, kerf, finger width, clearance.
- [x] MVP.0.4 Zaimplementowac prosty graf zaleznosci parametrow.
- [x] MVP.0.5 Zaimplementowac `CadDocument`, `Sketch`, `Layer`, `MaterialProfile`.
- [x] MVP.0.6 Zaimplementowac zapis i odczyt JSON.
- [x] MVP.0.7 Zaimplementowac generator finger joint dla prostych krawedzi.
- [x] MVP.0.8 Zaimplementowac generator otwartego pudelka.
- [x] MVP.0.9 Zaimplementowac kompensacje kerfu dla prostych konturow.
- [x] MVP.0.10 Zaimplementowac eksport SVG.
- [x] MVP.0.11 Zaimplementowac prosty widok Unity 2D.
- [x] MVP.0.12 Zaimplementowac panel zmiany parametrow pudelka.
- [x] MVP.0.13 Potwierdzic, ze zmiana parametru przebudowuje pudelko.
- [ ] MVP.0.14 Wyeksportowac SVG pudelka testowego.
- [ ] MVP.0.15 Otworzyc SVG w zewnetrznym programie i potwierdzic skale w mm.
- [x] MVP.0.16 Uruchomic komplet testow jednostkowych.

## MVP.1 Kryteria akceptacji MVP

- [x] MVP.1.0 Uzytkownik moze wybrac profil materialu.
- [x] MVP.1.1 Uzytkownik moze ustawic grubosc materialu.
- [x] MVP.1.2 Uzytkownik moze ustawic kerf.
- [x] MVP.1.3 Uzytkownik moze ustawic szerokosc, glebokosc i wysokosc pudelka.
- [ ] MVP.1.4 Uzytkownik widzi geometrie 2D wygenerowanego pudelka.
- [ ] MVP.1.5 Uzytkownik zmienia parametr i widzi przebudowe modelu.
- [x] MVP.1.6 Uzytkownik eksportuje poprawny SVG w milimetrach.
- [ ] MVP.1.7 SVG zawiera warstwy cut i engrave, jesli sa uzyte.
- [x] MVP.1.8 Testy jednostkowe przechodza lokalnie.
- [ ] MVP.1.9 Dokumentacja opisuje minimalny workflow: material -> parametry -> generuj -> eksportuj.
