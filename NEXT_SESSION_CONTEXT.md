# Kontekst dla nastepnej sesji Codex

## Aktualizacja po poprawkach zaznaczania 3.4.8-3.4.9

- W tej sesji poprawiono dwa problemy UX zaznaczania w Unity:
  - podczas przeciagania widoczny jest prostokat zaznaczania,
  - klikniecie trafia w realna geometrie linii, prostokatow i polilinii, zamiast polegac tylko na bounding boxie.
- Nowe commity:
  - `23b76c7 3.4.8 Pokaz prostokat zaznaczania`,
  - `18b07bd 3.4.9 Popraw hit-test zaznaczania`.
- Do `TASKS.md` dopisano i odhaczono:
  - `3.4.8 Pokazac prostokat zaznaczania podczas przeciagania`,
  - `3.4.9 Poprawic hit-test klikniecia dla linii, prostokatow i polilinii`.
- Zmiany techniczne:
  - `SelectionService` wystawia `IsDraggingSelection`, `DragStartScreenPosition` i `DragCurrentScreenPosition`,
  - `SelectionHighlightRenderer` rysuje obrys prostokata przeciagania,
  - `SelectionService` ma geometryczny hit-test segmentow dla `LineEntity`, `RectangleEntity`, `PolylineEntity`, a dodatkowo poprawione trafianie w `CircleEntity` i `ArcEntity`.
- Do sprawdzenia w aplikacji:
  - klik na obrys prostokata demo powinien zaznaczyc prostokat,
  - klik na lamana/polilinie powinien ja zaznaczyc,
  - przeciaganie LPM powinno pokazywac niebieski prostokat zaznaczania,
  - zaznaczanie prostokatem powinno nadal wybierac encje przecinajace jego obszar.
- Weryfikacja:
  - `dotnet test LaserCad.sln --no-restore` przechodzi: `390/390`,
  - `dotnet build src\LaserCad.Core\LaserCad.Core.csproj -f netstandard2.1 --no-restore` przechodzi,
  - `cmd /c build.bat` zakonczyl sie sukcesem i wygenerowal `C:\borys\CAD\bin\release\LaserCad\LaserCad.exe`.

### Doprecyzowanie po screenie uzytkownika

- Uzytkownik pokazal screen, na ktorym panel mial `Selected: 6`, ale dwa obiekty wygladaly jak niezaznaczone.
- Przyczyna byla wizualna: highlight mial podobna grubosc/kolor do geometrii albo mogl byc przykrywany przez rysowanie geometrii dokumentu.
- Dodano commit `fa2c425 3.4.10 Popraw czytelnosc highlightu zaznaczenia`.
- `SelectionHighlightRenderer`:
  - ma `DefaultExecutionOrder(1000)`,
  - rysuje zolty obrys zaznaczenia z czarnym halo,
  - uzywa `ZTest Always`,
  - scena ustawia `lineWidthPixels = 3` i `haloWidthPixels = 6`.
- Do sprawdzenia: po zaznaczeniu wszystkiego wszystkie 6 encji powinno miec widoczny zolty/czarny obrys, w tym czerwony prostokat i niebieska L-ka.

## Aktualizacja po sekcji 3.5 GUI funkcji domenowych - start

- W tej sesji rozpoczeto prace nad widocznym GUI dla dotychczasowych funkcjonalnosci domenowych.
- Nowe commity tej sesji:
  - `6835393 3.5 Dodaj plan GUI funkcji domenowych`,
  - `1fdfb9a 3.5.0 Dodaj renderer geometrii dokumentu`,
  - `d826880 3.5.1 Dodaj dokument demonstracyjny`,
  - `3b70db6 3.5.2 Dodaj panel parametrow pudelka`.
- Do `TASKS.md` dodano sekcje `3.5 GUI funkcji domenowych`.
- Odhaczono:
  - `3.5.0` renderer geometrii dokumentu 2D w Unity,
  - `3.5.1` startowy dokument demonstracyjny,
  - `3.5.2` panel parametrow generatora pudelka.
- Dodano `DocumentGeometryRenderer` w `LaserCad.Unity/Assets/Scripts`:
  - czyta `CadDocument` z `LaserCadApplicationController`,
  - rysuje `LineEntity`, `RectangleEntity`, `CircleEntity`, `ArcEntity`, `PolylineEntity` oraz prosty placeholder `TextEntity`,
  - uzywa kolorow warstw dokumentu (`Cut`, `Engrave`, `Score` itd.),
  - jest podpiety w scenie jako obiekt `Document Geometry`.
- `LaserCadApplicationController` ma teraz:
  - `CurrentBoxOptions`,
  - `SetBoxOptions(BoxGeneratorOptions)`,
  - flage `loadDemoDocument`,
  - startowy dokument `Demo dokument` z prostokatem, linia, okregiem, lukiem, zamknieta polilinia i placeholderem tekstu.
- Dodano `BoxGeneratorPanel`:
  - widoczny panel IMGUI `Generator pudelka`,
  - pola: szerokosc, glebokosc, wysokosc, grubosc materialu, kerf, szerokosc palca, clearance,
  - wybor typu: zamkniete, otwarte, z pokrywa,
  - przycisk `Zastosuj`,
  - walidacja idzie przez domenowy `BoxGeneratorOptions`.
- Po przegladzie planu dopisano przy `3.5.3`, ze przebudowa podgladu pudelka powinna bazowac na domenowym `BoxGenerator` z sekcji `8.1`. Nie nalezy generowac geometrii pudelka w Unity, zeby nie zlamac granicy UI/domena.
- Nastepny najlepszy krok:
  - albo wrocic do `8.1 Generator pudelka - geometria` i zrobic domenowy generator,
  - albo kontynuowac GUI dla funkcji juz gotowych domenowo, np. panele eksportu SVG/DXF, materialow/warstw, undo/redo i informacje constraints/dimensions.
- Testy:
  - `dotnet test LaserCad.sln --no-restore` przechodzi: `390/390` testow zielone.
  - `dotnet build src\LaserCad.Core\LaserCad.Core.csproj -f netstandard2.1 --no-restore` przechodzi bez ostrzezen.
- Pelny build aplikacji uruchomiono przez `cmd /c build.bat` i zakonczyl sie sukcesem:
  - aplikacja: `C:\borys\CAD\bin\release\LaserCad\LaserCad.exe`,
  - log: `C:\borys\CAD\bin\release\unity-build.log`,
  - log zawiera `Build Finished, Result: Success.`.
- Widoczne po odpaleniu aplikacji:
  - scena nie jest juz pusta: widoczny jest demo dokument z kolorowa geometria 2D,
  - nadal dziala grid, kamera, snap marker i zaznaczanie,
  - panel `Generator pudelka` jest po lewej stronie pod panelem zaznaczenia,
  - po zmianie wartosci i kliknieciu `Zastosuj` panel pokazuje status walidacji opcji; geometria pudelka jeszcze sie nie przebudowuje, bo brakuje `BoxGenerator` z `8.1`.

## Aktualizacja po sekcji 8.0 Generator pudelka

- W tej sesji wykonano cala sekcje `8.0 Generator pudelka` i odhaczono ja w `TASKS.md`.
- Nowe commity tej sesji:
  - `3b517e4 8.0.0 Utworz BoxGeneratorOptions`,
  - `cc9d36f 8.0.1 Dodaj szerokosc pudelka`,
  - `3b95340 8.0.2 Dodaj glebokosc pudelka`,
  - `36726f8 8.0.3 Dodaj wysokosc pudelka`,
  - `68938d6 8.0.4 Dodaj grubosc materialu pudelka`,
  - `dc57048 8.0.5 Dodaj kerf pudelka`,
  - `d691db0 8.0.6 Dodaj szerokosc palca pudelka`,
  - `625328d 8.0.7 Dodaj clearance pudelka`,
  - `7ae9139 8.0.8 Dodaj typ pudelka`,
  - `2fdeae7 8.0.9 Dodaj walidacje opcji pudelka`,
  - `af0f66c 8.0.10 Dodaj test walidacji zbyt malego pudelka`,
  - `b57805e 8.0 Odhacz model opcji pudelka`.
- Dodano namespace `LaserCad.Core.BoxGenerators`.
- Dodano `BoxGeneratorType`: `Closed`, `Open`, `WithLid`.
- Dodano `BoxGeneratorOptions`:
  - `Width`,
  - `Depth`,
  - `Height`,
  - `MaterialThickness`,
  - `Kerf`,
  - `FingerWidth`,
  - `Clearance`,
  - `BoxType`.
- Domyslne opcje:
  - szerokosc 100 mm,
  - glebokosc 80 mm,
  - wysokosc 50 mm,
  - grubosc materialu 3 mm,
  - kerf 0 mm,
  - szerokosc palca 10 mm,
  - clearance 0 mm,
  - typ pudelka `Open`.
- Walidacja:
  - szerokosc, glebokosc, wysokosc, grubosc materialu i szerokosc palca musza byc dodatnie,
  - kerf i clearance nie moga byc ujemne,
  - szerokosc i glebokosc musza byc wieksze niz dwukrotnosc grubosci materialu,
  - wysokosc musi byc wieksza niz grubosc materialu.
- Dodano testy w `tests/LaserCad.Tests/Core/BoxGenerators/BoxGeneratorOptionsTests.cs`:
  - wartosci domyslne,
  - przechowywanie jawnych opcji,
  - zerowa szerokosc,
  - ujemny kerf,
  - ujemny clearance,
  - zbyt male pudelko wzgledem grubosci materialu.
- Po przegladzie planu odhaczono `MVP.0.3`, bo parametry produkcyjne width/depth/height/material thickness/kerf/finger width/clearance sa juz zaimplementowane w modelu opcji generatora pudelka.
- Nie odhaczono `MVP.0.8`, bo nie ma jeszcze generatora geometrii otwartego pudelka.
- `docs/ROADMAP.md` ma nowa sekcje `Ograniczenia modelu generatora pudelka MVP`.
- `dotnet test LaserCad.sln --no-restore` przechodzi: `390/390` testow zielone.
- Odwiezono DLL domenowe dla Unity komenda:
  `dotnet build src\LaserCad.Core\LaserCad.Core.csproj -f netstandard2.1`.
- Unity build aplikacji zostal uruchomiony przez Unity `6000.0.0f1` i zakonczyl sie sukcesem:
  - aplikacja: `C:\borys\CAD\bin\release\LaserCad\LaserCad.exe`,
  - log: `C:\borys\CAD\bin\release\LaserCad\unity-build.log`,
  - log zawiera `Build Finished, Result: Success.`.
- Do sprawdzenia w aplikacji Unity po buildzie: sekcja `8.0` dodaje tylko logike domenowa modelu opcji generatora pudelka, bez UI i bez widocznej geometrii pudelka. Po uruchomieniu aplikacji widoczny ekran powinien pozostac jak po poprzednich sekcjach: scena robocza z kamera, gridem, snap markerem, zaznaczaniem i panelem informacji/wlasciwosci.
- Nastepna niewykonana sekcja wedlug `TASKS.md`: `8.1 Generator pudelka - geometria`.

## Aktualizacja po sekcji 7.1 Finger joint - algorytm

- W tej sesji wykonano cala sekcje `7.1 Finger joint - algorytm` i odhaczono ja w `TASKS.md`.
- Nowe commity tej sesji:
  - `901a395 7.1.0 Utworz wynik generatora finger joint`,
  - `ba99c85 7.1.1 Dodaj algorytm profilu finger joint`,
  - `dd3721b 7.1.8 Dodaj testy generatora finger joint`,
  - `a662e94 7.1 Odhacz algorytm finger joint`.
- Dodano domenowy generator dla prostych krawedzi w `LaserCad.Core.FingerJoints`.
- Nowe typy:
  - `FingerJointSegmentKind`: `Finger`, `Slot`,
  - `FingerJointSegment`: typ segmentu, offset od poczatku krawedzi, dlugosc, punkt startu i konca na krawedzi,
  - `FingerJointProfile`: segmenty, punkty lamanej profilu, grubosc materialu, wysuniecie palca, cofniecie wciecia, kompensacja kerfu i clearance,
  - `FingerJointGenerator`.
- `FingerJointGenerator.GenerateEdge(LineSegment2D edge, Length materialThickness, FingerJointOptions? options = null)`:
  - obsluguje tylko pojedyncza prosta krawedz,
  - normalna zewnetrzna jest po lewej stronie kierunku krawedzi,
  - dobiera rowna liczbe segmentow wedlug `FingerWidth` i zakresu min/max,
  - wymusza zgodnosc poczatku i konca z `StartWithFinger`/`EndWithFinger`,
  - generuje punkty lamanej profilu,
  - uzywa grubosci materialu jako bazowej glebokosci palca,
  - dodaje polowe kerfu do wysuniecia palca i do cofniecia wciecia,
  - dla clearance: `Tight = 0`, `Neutral = clearance / 2`, `Loose = clearance`.
- Dodano testy w `tests/LaserCad.Tests/Core/FingerJoints/FingerJointGeneratorTests.cs`:
  - podstawowe generowanie profilu,
  - krawedz 100 mm,
  - symetria segmentow,
  - start od palca,
  - start od wciecia,
  - kompensacja grubosci materialu,
  - kompensacja kerfu,
  - kompensacja clearance.
- `docs/ROADMAP.md` ma nowa sekcje `Ograniczenia algorytmu finger joint MVP`.
- Po przegladzie planu odhaczono `MVP.0.7`, bo generator finger joint dla prostych krawedzi jest zaimplementowany w domenie.
- Nie odhaczono taskow `8.x`, `9.x` ani Unity:
  - nie ma jeszcze generatora pelnych scianek pudelka,
  - nie ma laczenia naroznikow wielu krawedzi,
  - nie ma rozkladania elementow 2D,
  - nie ma jeszcze widocznej geometrii finger joint w aplikacji Unity.
- `dotnet test LaserCad.sln --no-restore` przechodzi: `384/384` testow zielone.
- Odwiezono DLL domenowe dla Unity komenda:
  `dotnet build src\LaserCad.Core\LaserCad.Core.csproj -f netstandard2.1`.
- Unity build aplikacji zostal sprobowany przez Unity `6000.0.0f1`, ale nie wygenerowal nowego builda, bo inna instancja Unity ma otwarty projekt `C:\borys\CAD\LaserCad.Unity`.
  - log: `C:\borys\CAD\bin\release\LaserCad\unity-build.log`,
  - log zawiera `It looks like another Unity instance is running with this project open.`,
  - przed kolejnym buildem zamknac otwarty edytor Unity albo uruchomic build po zwolnieniu locka projektu.
- Do sprawdzenia w aplikacji Unity po buildzie: sekcja `7.1` dodaje logike domenowa generatora profilu finger joint, ale nie dodaje UI ani renderowania tej geometrii. Po uruchomieniu aplikacji widoczny ekran powinien pozostac jak po poprzednich sekcjach: scena robocza z kamera, gridem, snap markerem, zaznaczaniem i panelem informacji/wlasciwosci.
- Nastepna niewykonana sekcja wedlug `TASKS.md`: `8.0 Generator pudelka`.

## Aktualizacja po sekcji 7.0 Finger joint - model danych

- W tej sesji wykonano cala sekcje `7.0 Finger joint - model danych` i odhaczono ja w `TASKS.md`.
- Nowe commity tej sesji:
  - `791e394 7.0.0 Utworz FingerJointOptions`,
  - `dbf08e2 7.0.1 Dodaj szerokosc palca`,
  - `fba469e 7.0.2 Dodaj minimalna szerokosc palca`,
  - `cfbd54d 7.0.3 Dodaj maksymalna szerokosc palca`,
  - `310444d 7.0.4 Dodaj flage zaczynania od zeba`,
  - `f21e943 7.0.5 Dodaj flage konczenia zebem`,
  - `13db55a 7.0.6 Dodaj tryb dopasowania finger joint`,
  - `c953e3f 7.0.7 Dodaj kerf i clearance finger joint`,
  - `e75d9c0 7.0.8 Dodaj walidacje opcji finger joint`,
  - `df94c88 7.0.9 Dodaj test walidacji opcji finger joint`,
  - `f3accef 7.0 Odhacz model danych finger joint`,
  - `8303298 Dopisz kontekst po sekcji 7.0`.
- Dodano namespace `LaserCad.Core.FingerJoints`.
- Dodano `FingerJointFitMode`: `Tight`, `Neutral`, `Loose`.
- Dodano `FingerJointOptions`:
  - `FingerWidth`,
  - `MinimumFingerWidth`,
  - `MaximumFingerWidth`,
  - `StartWithFinger`,
  - `EndWithFinger`,
  - `FitMode`,
  - `Kerf`,
  - `Clearance`.
- Domyslne opcje sa poprawne domenowo:
  - szerokosc/min/max palca = `1 mm`,
  - start i koniec zebem = `true`,
  - tryb = `Neutral`,
  - kerf i clearance = `0 mm`.
- Walidacja:
  - szerokosci palca musza byc dodatnie,
  - kerf i clearance nie moga byc ujemne,
  - minimum nie moze byc wieksze od maksimum,
  - docelowa szerokosc palca musi miescic sie w zakresie min/max.
- Dodano testy w `tests/LaserCad.Tests/Core/FingerJoints/FingerJointOptionsTests.cs` dla wartosci domyslnych, przechowywania opcji i walidacji przypadkow brzegowych.
- `docs/ROADMAP.md` ma nowa sekcje `Ograniczenia modelu finger joint MVP`.
- Po przegladzie planu nie odhaczono zadnego nowego taska MVP:
  - `MVP.0.7` nadal wymaga generatora finger joint dla prostych krawedzi,
  - sekcja `7.0` dodaje tylko model danych opcji, bez algorytmu i bez geometrii wynikowej.
- `dotnet test LaserCad.sln --no-restore` przechodzi: `376/376` testow zielone.
- Odwiezono DLL domenowe dla Unity komenda:
  `dotnet build src\LaserCad.Core\LaserCad.Core.csproj -f netstandard2.1`.
- Unity build zostal uruchomiony przez Unity `6000.0.0f1` i zakonczyl sie sukcesem:
  - aplikacja: `C:\borys\CAD\bin\release\LaserCad\LaserCad.exe`,
  - log: `C:\borys\CAD\bin\release\LaserCad\unity-build.log`,
  - log zawiera `Build Finished, Result: Success.`.
- Nastepna niewykonana sekcja wedlug `TASKS.md`: `7.1 Finger joint - algorytm`.
- Do sprawdzenia w aplikacji Unity po buildzie: sekcja `7.0` dodaje logike domenowa modelu opcji finger joint, ale nie dodaje jeszcze UI ani widocznej geometrii. Po uruchomieniu aplikacji widoczny ekran powinien pozostac jak po poprzednich sekcjach: scena robocza z kamera, gridem, snap markerem, zaznaczaniem i panelem informacji/wlasciwosci. Weryfikacja `7.0` jest aktualnie przez testy/API `LaserCad.Core.FingerJoints`.

## Aktualizacja po sekcji 6.1 Dimensions

- W tej sesji wykonano cala sekcje `6.1 Dimensions` i odhaczono ja w `TASKS.md`.
- Nowe commity tej sesji:
  - `5aea0ba 6.1.0 Utworz Dimension`,
  - `581c8bc 6.1.1 Dodaj wymiar dlugosci odcinka`,
  - `374e4d9 6.1.2 Dodaj wymiar szerokosci prostokata`,
  - `e53efdf 6.1.3 Dodaj wymiar wysokosci prostokata`,
  - `8240b30 6.1.4 Dodaj wymiar srednicy okregu`,
  - `cde18ef 6.1.5 Dodaj wymiar promienia okregu`,
  - `6a25d8a 6.1.6 Powiaz wymiar z parametrem`,
  - `fd8a22b 6.1.7 Dodaj test zmiany wymiaru przez parametr`,
  - `ad931d1 6.1 Odhacz dimensions`.
- Dodano namespace `LaserCad.Core.Dimensions`.
- Dodano `DimensionKind`: `Length`, `Width`, `Height`, `Diameter`, `Radius`.
- Dodano `Dimension`:
  - przechowuje `Id`, `EntityId`, `Kind`, `Name`, `Value` i opcjonalny `ParameterId`,
  - waliduje dodatnia wartosc wymiaru oraz niepuste identyfikatory/nazwe,
  - `BindToParameter(ParameterId)` zwraca wymiar powiazany z parametrem,
  - `Apply(Sketch)` przebudowuje wskazana encje wartoscia wymiaru,
  - `Apply(Sketch, ParameterSet)` uzywa wartosci powiazanego parametru typu `Length`, jesli `ParameterId` jest ustawiony.
- Obslugiwane przebudowy wymiarow:
  - `Length` dla `LineEntity`: zachowuje punkt startowy i kierunek odcinka, zmienia punkt koncowy,
  - `Width` dla `RectangleEntity`: zachowuje lewy dolny rog i wysokosc bounding boxa,
  - `Height` dla `RectangleEntity`: zachowuje lewy dolny rog i szerokosc bounding boxa,
  - `Diameter` dla `CircleEntity`: zachowuje srodek i ustawia promien na polowe wymiaru,
  - `Radius` dla `CircleEntity`: zachowuje srodek i ustawia promien na wartosc wymiaru.
- Wazne ograniczenie MVP: model wymiarow jest sekwencyjna operacja na pojedynczej encji, a nie pelny solver zaleznosci geometrycznych. Nie obsluguje jeszcze luk, polilinii ani tekstu.
- Dodano testy w `tests/LaserCad.Tests/Core/Dimensions`:
  - tworzenie i walidacje `Dimension`,
  - `BindToParameter`,
  - stosowanie wymiaru dlugosci linii,
  - stosowanie szerokosci i wysokosci prostokata,
  - stosowanie srednicy i promienia okregu,
  - uzycie wartosci z parametru `Length`,
  - blad dla parametru innego typu niz `Length`,
  - zmiana wartosci parametru przebudowuje wymiar.
- `docs/ROADMAP.md` zaktualizowano: ograniczenia parametrycznego szkicu nie twierdza juz, ze promien okregu i osobny model wymiarow sa poza zakresem; opisano aktualne ograniczenia `Dimension` MVP.
- Po przegladzie planu nie odhaczono zadnego nowego taska MVP:
  - `MVP.0.11` nadal wymaga renderowania geometrii 2D dokumentu w Unity,
  - `MVP.0.12` i `MVP.0.13` nadal wymagaja panelu parametrow pudelka i przebudowy pudelka,
  - `MVP.0.3` nadal wymaga konkretnych parametrow produkcyjnych/generatora, nie tylko ogolnego mechanizmu wymiarow.
- `dotnet test LaserCad.sln --no-restore` przechodzi: `368/368` testow zielone.
- Odwiezono DLL domenowe dla Unity komenda:
  `dotnet build src\LaserCad.Core\LaserCad.Core.csproj -f netstandard2.1`.
- Nastepna niewykonana sekcja wedlug `TASKS.md`: `7.0 Finger joint - model danych`.
- Do sprawdzenia w aplikacji Unity po buildzie: sekcja `6.1` dodaje logike domenowa wymiarow, ale nie dodaje jeszcze UI wymiarow ani widocznych przyciskow w Unity. Po uruchomieniu aplikacji widoczny ekran powinien pozostac jak po poprzednich sekcjach: scena robocza z kamera, gridem, snap markerem, zaznaczaniem i panelem informacji/wlasciwosci. Weryfikacja `6.1` jest aktualnie przez testy/API `LaserCad.Core.Dimensions`.

## Aktualizacja po sekcji 6.0 Constraints

- W tej sesji wykonano cala sekcje `6.0 Constraints` i odhaczono ja w `TASKS.md`.
- Nowe commity tej sesji:
  - `4586f6c 6.0.0 Utworz bazowy kontrakt constraintu`,
  - `cd2de8b 6.0.1 Dodaj HorizontalConstraint`,
  - `d51fbfd 6.0.2 Dodaj VerticalConstraint`,
  - `426f668 6.0.3 Dodaj ParallelConstraint`,
  - `d628e81 6.0.4 Dodaj PerpendicularConstraint`,
  - `68e58c3 6.0.5 Dodaj CoincidentConstraint`,
  - `e96a054 6.0.6 Dodaj EqualConstraint`,
  - `935981f 6.0.7 Dodaj prosty solver constraintow`,
  - `74ce382 Napraw podmiane encji constraintu`,
  - `5a078fe 6.0.8 Dodaj test horizontal`,
  - `b77de81 6.0.9 Dodaj test vertical`,
  - `61a4741 6.0.10 Dodaj test coincident`,
  - `71c653f 6.0 Odhacz constraints`.
- Dodano namespace `LaserCad.Core.Constraints`.
- Dodano bazowy kontrakt constraints:
  - `ISketchConstraint` z `Id`, `Kind` i `Apply(Sketch)`,
  - `SketchConstraintKind`: `Horizontal`, `Vertical`, `Parallel`, `Perpendicular`, `Coincident`, `Equal`.
- Dodano constrainty domenowe:
  - `HorizontalConstraint` wymusza pozioma `LineEntity`, zachowujac start i ustawiajac `End.Y = Start.Y`,
  - `VerticalConstraint` wymusza pionowa `LineEntity`, zachowujac start i ustawiajac `End.X = Start.X`,
  - `ParallelConstraint` dopasowuje druga linie do kierunku pierwszej i zachowuje jej dlugosc,
  - `PerpendicularConstraint` dopasowuje druga linie do kierunku prostopadlego wzgledem pierwszej i zachowuje jej dlugosc,
  - `EqualConstraint` ustawia dlugosc drugiej linii na dlugosc pierwszej, zachowujac kierunek drugiej,
  - `CoincidentConstraint` laczy dwa punkty szkicu przez przesuniecie punktu ograniczanego do punktu referencyjnego.
- Dodano referencje punktow dla coincident:
  - `SketchPointRole` z `Start` i `End`,
  - `SketchPointReference`.
- Dodano `SketchConstraintSolver`, ktory stosuje constrainty sekwencyjnie w przekazanej kolejnosci i zwraca nowy `Sketch`.
- Wazne ograniczenie MVP: solver nie jest pelnym solverem ukladu rownan geometrycznych. Nie wykrywa sprzecznych constraintow, nie iteruje do zbieznosci i dziala obecnie na `LineEntity` dla prostych relacji liniowych.
- Dodano testy w `tests/LaserCad.Tests/Core/Constraints`:
  - horizontal przez solver,
  - vertical przez solver,
  - coincident przez solver.
- Podczas testowania znaleziono i naprawiono bug w `SketchConstraintHelpers.ReplaceEntity`: leniwe `Select` bylo sprawdzane przed materializacja, przez co helper nie ustawial flagi `replaced`.
- `docs/ROADMAP.md` ma nowa sekcje `Ograniczenia solvera constraints MVP`.
- Po przegladzie planu nie odhaczono zadnego nowego taska MVP:
  - constraints sa fundamentem pod pozniejsze wymiary i parametry szkicu,
  - `MVP.0.11` nadal wymaga renderowania geometrii 2D dokumentu w Unity,
  - `MVP.0.12` i `MVP.0.13` nadal wymagaja panelu parametrow pudelka i przebudowy pudelka.
- `dotnet test LaserCad.sln --no-restore` przechodzi: `357/357` testow zielone.
- Odwiezono DLL domenowe dla Unity komenda:
  `dotnet build src\LaserCad.Core\LaserCad.Core.csproj -f netstandard2.1`.
- Unity build zostal uruchomiony przez Unity `6000.0.0f1` i zakonczyl sie sukcesem:
  - aplikacja: `C:\borys\CAD\bin\release\LaserCad\LaserCad.exe`,
  - log: `C:\borys\CAD\bin\release\LaserCad\unity-build.log`,
  - log zawiera `Build Finished, Result: Success.`.
- Nastepna niewykonana sekcja wedlug `TASKS.md`: `6.1 Dimensions`.
- Do sprawdzenia w aplikacji Unity po buildzie: sekcja `6.0` dodaje logike domenowa constraints, ale nie dodaje jeszcze UI constraintow ani widocznych przyciskow w Unity. Po uruchomieniu aplikacji widoczny ekran pozostaje jak po poprzednich sekcjach: scena robocza z kamera, gridem, snap markerem, zaznaczaniem i panelem informacji/wlasciwosci. Weryfikacja `6.0` jest aktualnie przez testy/API `LaserCad.Core.Constraints`.

## Aktualizacja po sekcji 5.2 Feature tree

- W tej sesji wykonano cala sekcje `5.2 Feature tree` i odhaczono ja w `TASKS.md`.
- Nowe commity tej sesji:
  - `74cf066 5.2.0 Utworz model elementu drzewa historii`,
  - `5bea1f3 5.2.1 Dodaj wpis generatora`,
  - `57a4dfc 5.2.2 Dodaj wpis operacji edycyjnej`,
  - `7a17969 5.2.3 Dodaj aktywowanie operacji`,
  - `b659eea 5.2.4 Dodaj przebudowe dokumentu z feature tree`,
  - `3e3a4a9 5.2.5 Dodaj test feature tree generator move`,
  - `93f3beb 5.2 Odhacz feature tree`.
- Dodano namespace `LaserCad.Core.FeatureTree`.
- Dodano bazowy model drzewa historii:
  - `FeatureTreeItemKind` z wartosciami `Generator` i `EditOperation`,
  - abstrakcyjny `FeatureTreeItem` z `Id`, `Name`, `Kind`, `IsEnabled`, `WithEnabled(bool)` i `Apply(CadDocument)`.
- Dodano `GeneratorFeatureTreeItem`:
  - przechowuje `GeneratorInstance`,
  - opcjonalnie przechowuje szkice wygenerowane przez generator,
  - podczas `Apply` dodaje generator i wygenerowane szkice do dokumentu.
- Dodano `EditOperationFeatureTreeItem`:
  - przechowuje `ICommand`,
  - podczas `Apply` wykonuje `Command.Execute(document)`.
- Dodano `FeatureTree`:
  - przechowuje uporzadkowane `Items`,
  - `Add(FeatureTreeItem)` zwraca nowe drzewo z dopisanym wpisem,
  - `Enable(Guid)` i `Disable(Guid)` zwracaja nowe drzewo z przelaczonym wpisem,
  - `Rebuild(CadDocument baseDocument)` wykonuje aktywne wpisy w kolejnosci i pomija nieaktywne.
- Dodano testy w `tests/LaserCad.Tests/Core/FeatureTree`:
  - bazowe wlasciwosci i walidacje `FeatureTreeItem`,
  - zachowanie wpisu generatora,
  - zachowanie wpisu operacji edycyjnej,
  - dodawanie, wlaczanie i wylaczanie wpisow w `FeatureTree`,
  - przebudowe aktywnych wpisow w kolejnosci,
  - przekrojowy test `generator + move`, ktory tworzy szkic z linia przez wpis generatora i przesuwa ja przez `MoveCommand`.
- `dotnet test LaserCad.sln` przechodzi: `354/354` testow zielone.
- Odwiezono DLL domenowe dla Unity komenda:
  `dotnet build src\LaserCad.Core\LaserCad.Core.csproj -f netstandard2.1`.
- Unity build zostal uruchomiony przez Unity `6000.0.0f1` i zakonczyl sie sukcesem:
  - aplikacja: `C:\borys\CAD\bin\release\LaserCad\LaserCad.exe`,
  - log: `C:\borys\CAD\bin\release\LaserCad\unity-build.log`,
  - log zawiera `Build Finished, Result: Success.`.
- Po przegladzie planu nie odhaczono zadnego nowego taska MVP:
  - `MVP.0.11` nadal wymaga renderowania geometrii 2D dokumentu w Unity,
  - `MVP.0.12` i `MVP.0.13` nadal wymagaja panelu parametrow pudelka i przebudowy pudelka,
  - feature tree jest na razie domenowym fundamentem, bez UI w Unity.
- Nastepna niewykonana sekcja wedlug `TASKS.md`: `6.0 Constraints`.
- Do sprawdzenia w aplikacji Unity po buildzie: ta sekcja dodaje logike domenowa feature tree, ale nie dodaje jeszcze panelu drzewa historii ani przyciskow w UI. Po uruchomieniu aplikacji widoczny ekran pozostaje jak po poprzednich sekcjach: scena robocza z kamera, gridem, snap markerem, zaznaczaniem i panelem informacji/wlasciwosci. Weryfikacja `5.2` jest aktualnie przez testy/API `FeatureTree`.

## Aktualizacja po sekcji 5.1 Undo i redo

- W tej sesji wykonano cala sekcje `5.1 Undo i redo` i odhaczono ja w `TASKS.md`.
- Nowe commity tej sesji:
  - `a605c3f 5.1.0 Utworz UndoRedoStack`,
  - `8eb1d18 5.1.1 Dodaj stos undo`,
  - `4055515 5.1.2 Dodaj stos redo`,
  - `f0c1aef 5.1.3 Czysc redo po nowej komendzie`,
  - `65669bc 5.1.4 Dodaj limit historii`,
  - `620b0ef 5.1.5 Dodaj grupowanie komend`,
  - `f929356 5.1.6 Dodaj test undo`,
  - `04d82bc 5.1.7 Dodaj test redo`,
  - `d3a81bb 5.1.8 Dodaj test czyszczenia redo`,
  - `c064ce3 5.1 Odhacz undo i redo`.
- Dodano `UndoRedoStack` w `LaserCad.Core.Commands`:
  - trzyma `CurrentDocument`,
  - `Execute(ICommand)` wykonuje komende, dopisuje ja do undo i czysci redo,
  - `Undo()` cofa ostatnia komende, przenosi ja na redo i zwraca nowy dokument,
  - `Redo()` wykonuje ponownie ostatnio cofnieta komende i przenosi ja z powrotem na undo,
  - udostepnia `CanUndo`, `CanRedo`, `UndoCount`, `RedoCount`, `HistoryLimit`,
  - domyslny limit historii to `UndoRedoStack.DefaultHistoryLimit = 100`,
  - limit usuwa najstarsze wpisy undo po przekroczeniu pojemnosci.
- Dodano `CommandGroup` jako kompozyt `ICommand`:
  - wykonuje komendy w kolejnosci,
  - cofa je w odwrotnej kolejnosci,
  - pozwala zapisac kilka operacji jako jeden wpis historii undo/redo.
- Dodano testy `UndoRedoStackTests`:
  - undo przywraca poprzedni dokument,
  - redo ponawia cofnieta komende,
  - nowa komenda po undo czysci redo,
  - limit historii usuwa najstarszy wpis,
  - grupa komend cofa sie jako jedna pozycja historii.
- `dotnet test LaserCad.sln` przechodzi: `338/338` testow zielone.
- Odwiezono DLL domenowe dla Unity komenda:
  `dotnet build src\LaserCad.Core\LaserCad.Core.csproj -f netstandard2.1`.
- Po przegladzie planu nie odhaczono zadnego nowego taska MVP:
  - `MVP.0.11` nadal wymaga renderowania geometrii 2D w Unity,
  - undo/redo jest juz w domenie, ale nie jest jeszcze podpiete do UI Unity.
- Nastepna niewykonana sekcja wedlug `TASKS.md`: `5.2 Feature tree`.
- Do sprawdzenia w aplikacji Unity po buildzie: ta sekcja dodaje logike domenowa historii operacji, ale nie podlacza jeszcze przyciskow ani skrotow undo/redo w UI. Po uruchomieniu aplikacji widoczny ekran pozostaje jak po poprzednich sekcjach: scena robocza z kamera, gridem, snap markerem, zaznaczaniem i panelem informacji/wlasciwosci. Weryfikacja `5.1` jest aktualnie przez testy/API `UndoRedoStack`.

## Aktualizacja po sekcji 5.0 Komendy edycyjne

- W tej sesji wykonano cala sekcje `5.0 Komendy edycyjne` i odhaczono ja w `TASKS.md`.
- Nowe commity tej sesji:
  - `4e76a4e 5.0.0 Utworz interfejs ICommand`,
  - `884c44b 5.0.1 Dodaj wykonanie komendy`,
  - `7054869 5.0.2 Dodaj cofanie komendy`,
  - `e352791 5.0.3 Utworz MoveCommand`,
  - `7e92d94 5.0.4 Utworz RotateCommand`,
  - `fbbe39a 5.0.5 Utworz ScaleCommand`,
  - `f7c73ac 5.0.6 Utworz MirrorCommand`,
  - `cc80e95 5.0.7 Utworz DeleteCommand`,
  - `35c4823 5.0.8 Utworz AddEntityCommand`,
  - `7626036 5.0.9 Dodaj test wykonania komend`,
  - `9fa6874 5.0.10 Dodaj test cofania komend`,
  - `2043de6 5.0 Odhacz komendy edycyjne`.
- Dodano namespace `LaserCad.Core.Commands`.
- Dodano `ICommand` z metodami:
  - `CadDocument Execute(CadDocument document)`,
  - `CadDocument Undo(CadDocument document)`.
- Dodano komendy edycyjne pracujace niemutujaco na `CadDocument` i wskazanym `Sketch.Id`:
  - `MoveCommand`,
  - `RotateCommand`,
  - `ScaleCommand`,
  - `MirrorCommand`,
  - `DeleteCommand`,
  - `AddEntityCommand`.
- Komendy transformacji uzywaja istniejacych operacji `Sketch`: `MoveEntity`, `RotateEntity`, `ScaleEntity`, `MirrorEntity`.
- `ScaleCommand` odrzuca zerowa skale, bo undo korzysta z odwrotnosci wspolczynnikow.
- `MirrorCommand` cofa sie przez ponowne odbicie wzgledem tej samej osi.
- `DeleteCommand` przyjmuje usuwana encje w konstruktorze, zeby undo moglo ja deterministycznie przywrocic.
- `AddEntityCommand` cofa sie przez usuniecie dodanej encji po `Id`.
- Dodano wewnetrzny helper `DocumentCommandHelpers.ReplaceSketch`, ktory podmienia jeden szkic w niemutowalnym dokumencie i rzuca czytelny blad, jesli szkic nie istnieje.
- Dodano testy w `tests/LaserCad.Tests/Core/Commands`:
  - kontrakt `ICommand`,
  - wykonanie kazdej komendy,
  - cofniecie kazdej komendy,
  - podstawowe testy jednostkowe dla kazdej konkretnej komendy.
- `dotnet test LaserCad.sln` przechodzi: `333/333` testow zielone.
- Odwiezono DLL domenowe dla Unity komenda:
  `dotnet build src\LaserCad.Core\LaserCad.Core.csproj -f netstandard2.1`.
- Unity build zostal uruchomiony przez Unity `6000.0.0f1` i zakonczyl sie sukcesem wedlug logu:
  - aplikacja: `C:\borys\CAD\bin\release\LaserCad\LaserCad.exe`,
  - log: `C:\borys\CAD\bin\release\LaserCad\unity-build.log`,
  - log zawiera `Build Finished, Result: Success.`.
- Po przegladzie planu nie odhaczono zadnego nowego taska MVP:
  - `MVP.0.11` nadal wymaga prostego renderowania geometrii 2D w Unity,
  - `MVP.0.3`, `MVP.0.7`, `MVP.0.8`, `MVP.0.9` nadal dotycza parametrow/generatorow/kerfu.
- Nastepna niewykonana sekcja wedlug `TASKS.md`: `5.1 Undo i redo`.
- Do sprawdzenia w aplikacji Unity po buildzie: ta sekcja dodaje logike domenowa komend edycyjnych, ale nie podlacza jej jeszcze do UI Unity. Po uruchomieniu aplikacji nie pojawia sie nowy przycisk ani nowe narzedzie; widoczny ekran pozostaje jak po poprzednich sekcjach: scena robocza z kamera, gridem, snap markerem, zaznaczaniem i panelem informacji/wlasciwosci. Weryfikacja `5.0` jest aktualnie przez testy/API komend.

## Aktualizacja po sekcji 4.2 Eksport DXF

- W tej sesji wykonano cala sekcje `4.2 Eksport DXF` i odhaczono ja w `TASKS.md`.
- Nowe commity tej sesji:
  - `64126da 4.2.0 Utworz opcje eksportu DXF`,
  - `853d4fe 4.2.1 Utworz eksporter DXF`,
  - `c162005 4.2.2 Dodaj eksport LINE DXF`,
  - `667a35f 4.2.3 Dodaj eksport CIRCLE DXF`,
  - `a8ef7db 4.2.4 Dodaj eksport ARC DXF`,
  - `b3174b2 4.2.5 Dodaj eksport LWPOLYLINE DXF`,
  - `add984a 4.2.6 Dodaj warstwy DXF`,
  - `323426f 4.2.7 Dodaj test podstawowego pliku DXF`,
  - `c8915de 4.2 Odhacz eksport DXF`,
  - `60e5295 Usun placeholder eksportera DXF`.
- Dodano `DxfExportOptions` i `DxfExportUnit` w `src/LaserCad.Export.Dxf`:
  - domyslna jednostka to milimetry,
  - `ExportedLayerNames` dziala jak w SVG: trimuje nazwy, usuwa puste wpisy i duplikaty.
- Dodano `DxfExporter.Export(CadDocument, DxfExportOptions?)`.
- Eksporter DXF generuje tekstowy plik DXF z sekcjami `HEADER`, `TABLES`, `ENTITIES` i koncowym `EOF`.
- Eksportowane encje:
  - `LineEntity` jako `LINE`,
  - `CircleEntity` jako `CIRCLE`,
  - `ArcEntity` jako `ARC` z katami w stopniach; dla lukow clockwise zamienia kat start/end, bo DXF opisuje luki counterclockwise,
  - `PolylineEntity` jako `LWPOLYLINE`,
  - `RectangleEntity` jako zamknieta `LWPOLYLINE`.
- Eksporter zapisuje tabele warstw `LAYER`:
  - warstwy `Ignore` sa pomijane,
  - encje na warstwie `Ignore` sa pomijane,
  - `ExportedLayerNames` filtruje zarowno tabele warstw, jak i encje,
  - znane kolory sa mapowane na podstawowe indeksy ACI DXF: czerwony 1, zielony 3, niebieski 5, szary 8, fallback 7.
- Usunieto nieaktualny placeholder `src/LaserCad.Export.Dxf/Class1.cs`.
- Dodano testy DXF w `tests/LaserCad.Tests/Export/Dxf`:
  - opcje eksportu,
  - pusty dokument,
  - eksport `LINE`, `CIRCLE`, `ARC`, `LWPOLYLINE` i prostokata,
  - warstwy, ignorowanie warstwy `Ignore`, filtr warstw,
  - przekrojowy test podstawowego pliku DXF.
- `dotnet test LaserCad.sln` przechodzi: `322/322` testow zielone.
- Odwiezono DLL domenowe dla Unity komenda:
  `dotnet build src\LaserCad.Core\LaserCad.Core.csproj -f netstandard2.1`.
- Unity build zostal uruchomiony przez Unity `6000.0.0f1` i zakonczyl sie sukcesem wedlug logu:
  - aplikacja: `C:\borys\CAD\bin\release\LaserCad\LaserCad.exe`,
  - log: `C:\borys\CAD\bin\release\LaserCad\unity-build.log`,
  - log zawiera `Build Finished, Result: Success.`.
- Po przegladzie planu nie odhaczono zadnego nowego taska MVP, bo DXF nie jest osobna pozycja w `MVP.0`.
- Nastepna niewykonana sekcja wedlug `TASKS.md`: `5.0 Komendy edycyjne`.
- Do sprawdzenia w aplikacji Unity po buildzie: zmiany DXF nie maja jeszcze UI w Unity, wiec w uruchomionej aplikacji nie pojawi sie nowy przycisk eksportu. Weryfikacja tej sekcji jest przez testy/API `DxfExporter`.

## Aktualizacja po sekcji 4.1 Eksport SVG - encje

- W tej sesji wykonano cala sekcje `4.1 Eksport SVG - encje` i odhaczono ja w `TASKS.md`.
- Nowe commity tej sesji:
  - `20ebf4d 4.1.0 Dodaj eksport linii SVG`,
  - `ac11541 4.1.1 Dodaj eksport prostokata SVG`,
  - `c673da1 4.1.2 Dodaj eksport okregu SVG`,
  - `095f932 4.1.3 Dodaj eksport luku SVG`,
  - `3789a13 4.1.4 Dodaj eksport polilinii SVG`,
  - `857f493 4.1.5 Dodaj kolory warstw SVG`,
  - `1db1daf 4.1.6 Pomijaj warstwe ignore w SVG`,
  - `fad8305 4.1.7 Dodaj testy eksportu encji SVG`,
  - `8782516 4.1.8 Dodaj testy warstw SVG`,
  - `a7128db 4.1 Odhacz eksport encji SVG`.
- `SvgExporter` eksportuje teraz encje szkicu:
  - `LineEntity` jako `line`,
  - `RectangleEntity` jako zamkniety `path`, zeby obslugiwac takze prostokaty po transformacji,
  - `CircleEntity` jako `circle`,
  - `ArcEntity` jako `path` z komenda `A`,
  - `PolylineEntity` jako otwarty albo zamkniety `path`.
- Eksport SVG mapuje kolor warstwy na atrybut `stroke` kazdej encji.
- Encje z warstw o roli `LayerRole.Ignore` sa pomijane w eksporcie i nie wplywaja na `viewBox`.
- `SvgExportOptions.ExportedLayerNames` jest teraz stosowane przy eksporcie encji i liczeniu `viewBox`.
- Jesli encja wskazuje nieznana warstwe, kolor fallbacku to `#000000`.
- Dodano testy eksportu wszystkich obslugiwanych encji oraz testy kolorow, filtrowania warstw i pomijania warstwy `Ignore`.
- Po przegladzie planu odhaczono `MVP.0.10`, bo eksport SVG zawiera fundament, encje, kolory warstw i podstawowe zachowanie produkcyjne.
- `dotnet test LaserCad.sln` przechodzi: `308/308` testow zielone.
- Nastepna niewykonana sekcja wedlug `TASKS.md`: `4.2 Eksport DXF`.
- Do sprawdzenia w aplikacji Unity po buildzie: widok Unity nadal nie ma UI eksportu SVG, wiec zmiany z tej sekcji sa widoczne glownie przez testy/API eksportera, nie jako nowy przycisk w aplikacji.

## Aktualizacja po sekcji 4.0 Eksport SVG - fundament

- W tej sesji wykonano cala sekcje `4.0 Eksport SVG - fundament` i odhaczono ja w `TASKS.md`.
- Nowe commity tej sesji:
  - `7012752 4.0.0 Utworz opcje eksportu SVG`,
  - `b98b1d2 4.0.1 Dodaj jednostke eksportu SVG`,
  - `f794787 4.0.2 Dodaj grubosc linii SVG`,
  - `5e13d9f 4.0.3 Dodaj wybor warstw SVG`,
  - `7ac6818 4.0.4 Utworz eksporter SVG`,
  - `1092ba0 4.0.5 Dodaj eksport pustego SVG`,
  - `7d3cfcc 4.0.6 Dodaj viewBox SVG`,
  - `dde364f 4.0.7 Dodaj snapshot pustego SVG`,
  - `d962d09 4.0 Odhacz fundament eksportu SVG`.
- Dodano fundament eksportera w `src/LaserCad.Export.Svg`:
  - `SvgExportOptions` z domyslna jednostka `SvgExportUnit.Millimeters`,
  - `StrokeWidthMillimeters` z walidacja wartosci nieujemnej,
  - `ExportedLayerNames` jako opcjonalny filtr nazw warstw, z trimowaniem, usuwaniem pustych wpisow i duplikatow,
  - `SvgExporter.Export(CadDocument, SvgExportOptions?)`.
- `SvgExporter` generuje na razie pusty element SVG:
  - namespace `http://www.w3.org/2000/svg`,
  - `width` i `height` w `mm`,
  - `viewBox` liczony z bounding boxow encji szkicow,
  - dla pustego dokumentu wynik jest deterministyczny: `viewBox="0 0 0 0"`, `width="0mm"`, `height="0mm"`,
  - `fill="none"`, `stroke="#000000"`, `stroke-width` z opcji.
- Dodano testy w `tests/LaserCad.Tests/Export/Svg`:
  - domyslna jednostka SVG to milimetry,
  - ujemna grubosc linii jest odrzucana,
  - filtr warstw normalizuje wpisy,
  - null document w exporterze rzuca `ArgumentNullException`,
  - dokument z encja szkicu ustawia `viewBox` z `Entity.Bounds`,
  - snapshot pustego SVG porownuje caly wynik XML.
- `MVP.0.10` nadal zostal nieodhaczony: jest fundament eksportera SVG, ale nie ma jeszcze eksportu encji z sekcji `4.1`.
- `MVP.0.11` nadal zostal nieodhaczony: Unity ma kamere, grid, snap i selection, ale nadal brakuje pelnego renderowania geometrii 2D dokumentu w widoku Unity.
- `dotnet test LaserCad.sln` przechodzi: `299/299` testow zielone.
- Po tej sekcji nastepna niewykonana sekcja wedlug `TASKS.md`: `4.1 Eksport SVG - encje`.
- Przed pushowaniem `master` byl `ahead 9` wzgledem `origin/master`.

## Aktualizacja po sekcji 3.4 Zaznaczanie

- W tej sesji wykonano cala sekcje `3.4 Zaznaczanie` i odhaczono ja w `TASKS.md`.
- Nowe commity tej sesji:
  - `e8756da 3.4.0 Utworz SelectionService`,
  - `51a4064 3.4.1 Dodaj zaznaczanie kliknieciem`,
  - `1e1fdb9 3.4.2 Dodaj odznaczanie pustym kliknieciem`,
  - `c71bde3 3.4.3 Dodaj multi-select modyfikatorem`,
  - `ea95936 3.4.4 Dodaj zaznaczanie prostokatem`,
  - `5be49f7 3.4.5 Dodaj highlight zaznaczenia`,
  - `3c498b2 3.4.6 Dodaj panel wlasciwosci zaznaczenia`,
  - `ca0d818 3.4.7 Podlacz zaznaczanie do sceny`,
  - `c505856 3.4 Odhacz sekcje zaznaczania w planie`.
- Dodano Unity-side selection:
  - `SelectionService` trzyma zaznaczone `Guid` encji,
  - klik LPM wybiera najblizsza encje po bounding boxie z tolerancja 2 mm,
  - klik w puste miejsce czysci zaznaczenie,
  - `Ctrl` albo `Shift` wlacza multi-select przez toggle,
  - przeciaganie LPM zaznacza encje, ktorych bounding box przecina prostokat wyboru,
  - `SelectionHighlightRenderer` rysuje niebieski obrys bounding boxa zaznaczonych encji,
  - `SelectionPropertiesPanel` pokazuje liczbe zaznaczonych encji oraz typ, warstwe i bounding box pierwszej zaznaczonej encji.
- `Workspace.unity` ma nowy obiekt `Workspace Selection` z podpietymi:
  - `SelectionService`,
  - `SelectionHighlightRenderer`,
  - `SelectionPropertiesPanel`.
- `LaserCad.Unity/Docs/QA_CHECKLIST.md` ma nowa sekcje manualnego QA dla zaznaczania.
- Po przegladzie planu dopisano i odhaczono `3.4.7 Podpiac zaznaczanie do sceny roboczej`, bo byl to brakujacy praktyczny krok.
- `MVP.0.11` nadal zostal nieodhaczony: jest kamera, grid, snap i selection, ale nadal nie ma pelnego renderowania geometrii 2D dokumentu w widoku Unity.
- `dotnet test LaserCad.sln` przechodzi: `293/293` testow zielone.
- Odwiezono DLL domenowe dla Unity komenda:
  `dotnet build src\LaserCad.Core\LaserCad.Core.csproj -f netstandard2.1`.
- Unity build zostal uruchomiony przez Unity `6000.0.0f1` i zakonczyl sie sukcesem:
  - aplikacja: `C:\borys\CAD\bin\release\LaserCad\LaserCad.exe`,
  - log: `C:\borys\CAD\bin\release\LaserCad\unity-build.log`.
- Po tej sekcji nastepna niewykonana sekcja wedlug `TASKS.md`: `4.0 Eksport SVG - fundament`.
- Repo przed pushowaniem jest `ahead 9` wzgledem `origin/master`.

## Aktualizacja po sekcji 3.3 Snap

- W tej sesji wykonano cala sekcje `3.3 Snap` i odhaczono ja w `TASKS.md`.
- Przed pushowaniem `master` byl `ahead 8` wzgledem `origin/master`.
- Nowe commity tej sesji:
  - `ae574e0 3.3.0 Utworz SnapService`,
  - `0e43884 3.3.1 Dodaj snap do siatki`,
  - `0bde80e 3.3.2 Dodaj snap do punktow encji`,
  - `dabfebb 3.3.3 Dodaj snap do srodkow encji`,
  - `c5e764a 3.3.4 Dodaj snap do koncow linii`,
  - `b5ff0da 3.3.5 Dodaj priorytety snapowania`,
  - `e7a38ec 3.3.6 Dodaj wizualny marker snapu`,
  - `0a9bfe0 3.3.7 Dodaj checklist snapu QA`.
- Dodano Unity-side snap:
  - `SnapService` w `LaserCad.Unity/Assets/Scripts`,
  - `SnapCandidate`, `SnapResult`, `SnapPriority`,
  - snap do siatki 1 mm,
  - snap do punktow encji: narozniki prostokata, punkty polilinii, punkt tekstu, start/koniec luku,
  - snap do srodka okregu i srodka prostokata,
  - snap do koncow linii,
  - priorytety: `LineEnd` > `EntityCenter` > `EntityPoint` > `Grid`,
  - `SnapMarkerRenderer`, ktory rysuje krzyzyk w miejscu aktualnego snapu.
- `Workspace.unity` ma nowy obiekt `Workspace Snap` z podpietym `SnapService` i `SnapMarkerRenderer`.
- Przy okazji podpinania snapu dopieto `LaserCadApplicationController` do obiektu `LaserCad App` w scenie, bo w pliku sceny byl tylko pusty transform; marker moze dzieki temu czytac encje z aktualnego dokumentu.
- `LaserCad.Unity/Docs/QA_CHECKLIST.md` ma nowa sekcje manualnego QA dla snapu.
- Po przegladzie planu dopisano i odhaczono `3.3.7 Dodac test manualny snapu do checklisty QA`.
- `dotnet test LaserCad.sln` przechodzi: `293/293` testow zielone.
- Unity nie bylo uruchamiane w edytorze w tej sesji; weryfikacja snapu w scenie pozostaje manualnym QA.
- Po tej sekcji nastepna niewykonana sekcja wedlug `TASKS.md`: `3.4 Zaznaczanie`.
- Po pushu repo powinno byc zsynchronizowane z `origin/master`.

## Aktualizacja po sekcji 3.2 Grid

- W tej sesji wykonano cala sekcje `3.2 Grid` i odhaczono ja w `TASKS.md`.
- Przed pushowaniem `master` byl `ahead 9` wzgledem `origin/master`: byly juz 2 lokalne commity uzytkownika (`a785e1f`, `c995a76`) oraz 7 nowych commitow tej sesji.
- Nowe commity tej sesji:
  - `a4f270a 3.2.0 Dodaj renderer siatki`,
  - `8c3d5a0 3.2.1 Dodaj siatke 1 mm`,
  - `66c1207 3.2.2 Dodaj linie siatki co 5 mm`,
  - `d85f521 3.2.3 Dodaj linie siatki co 10 mm`,
  - `cbb79d2 3.2.4 Dopasuj grubosc siatki do zoomu`,
  - `79ae9c4 3.2.5 Dodaj przelaczanie siatki`,
  - `e0ad142 3.2 Odhacz sekcje grid w planie`.
- Dodano `WorkspaceGridRenderer` w `LaserCad.Unity/Assets/Scripts`:
  - renderuje siatke w widocznym obszarze kamery,
  - ma podzialke 1 mm,
  - mocniejsze linie co 5 mm i 10 mm,
  - rysuje linie jako cienkie prostokaty, z gruboscia liczona z `orthographicSize`, zeby pozostaly czytelne przy zoomie,
  - pozwala wlaczac/wylaczac siatke klawiszem `G` oraz metoda `SetVisible(bool)`.
- `WorkspaceGridRenderer` jest podpiety w `LaserCad.Unity/Assets/Scenes/Workspace.unity` jako obiekt `Workspace Grid`.
- `LaserCad.Unity/Docs/QA_CHECKLIST.md` ma sekcje manualnego QA dla siatki.
- Po przegladzie planu dopisano i odhaczono `3.2.6 Podpiac renderer siatki do sceny roboczej`, bo byl to brakujacy praktyczny krok.
- `dotnet test LaserCad.sln` przechodzilo po kazdym commicie gridu; ostatni wynik: `293/293` testow zielone.
- W workspace pozostaly niecommitowane, nie sledzone pliki wygenerowane przez Unity:
  - meta dla zaleznosci DLL w `LaserCad.Unity/Assets/Plugins/LaserCad.Domain/*.dll.meta`,
  - `LaserCad.Unity/Assets/Scenes/scena.unity` i `.meta`,
  - `LaserCad.Unity/ProjectSettings/SceneTemplateSettings.json`.
  Nie byly ruszane ani commitowane w tej sesji.
- Nastepna niewykonana sekcja wedlug `TASKS.md`: `3.3 Snap`.

Ten plik jest lokalny i celowo dodany do `.git/info/exclude`, zeby nie trafil do commita ani na GitHuba.

## Stan repo

- Workspace: `C:\borys\CAD`.
- Branch: `master`.
- Remote: `origin` -> `https://github.com/borys-/LaserCAD.git`.
- Ostatni wypchniety commit po tej sesji: po pushu powinien byc `8a75352 Napraw kompatybilnosc projektu Unity`.
- Po ostatnim pushu repo bylo czyste i zsynchronizowane z `origin/master`.
- Testy po ostatnich zmianach: `dotnet test LaserCad.sln` przechodzilo, `293/293` testow zielone.

## Zasady pracy ustalone z uzytkownikiem

- Pracowac wedlug `TASKS.md`.
- Jeden task z `TASKS.md` = jeden commit.
- Po wykonaniu calej proszonej sekcji zwykle uruchomic `dotnet test LaserCad.sln`.
- Uzytkownik czesto oczekuje pushowania po calej sekcji; jesli powie "push", wypchnac na `origin/master`.
- Dokumentacja i komentarze maja byc po polsku, ale techniczne nazwy klas/metod zostaja po angielsku.
- Preferowac ASCII w plikach, zgodnie z `docs/CODING_STANDARDS.md`.
- Biblioteki domenowe nie moga zalezec od Unity.
- Core moze zalezec od Geometry.
- Exportery moga zalezec od Core, ale nie powinny modyfikowac dokumentu.

## Wazne dokumenty

- `TASKS.md` - glowna lista taskow. Ukonczone sa sekcje `0.0` do `2.1` wlacznie.
- `docs/ARCHITECTURE.md` - granice projektow i zaleznosci.
- `docs/CODING_STANDARDS.md` - standardy kodowania, testow, git i dokumentacji.
- `docs/ROADMAP.md` - roadmapa produktu.
- `docs/PARAMETRIC_EXPRESSIONS.md` - ustalenie MVP dla wyrazen parametrycznych: jawne AST zamiast parsera tekstowego.
- `docs/PROJECT_FILE_FORMAT.md` - kontrakt JSON dla formatu projektu MVP, wlacznie ze szkicami i encjami.

## Struktura solution

- `LaserCad.sln`.
- `src/LaserCad.Geometry` - matematyka i geometria 2D, niezalezna od Core.
- `src/LaserCad.Core` - dokument CAD, parametry, wyrazenia, graf zaleznosci.
- `src/LaserCad.Export.Svg` - placeholder eksportera SVG.
- `src/LaserCad.Export.Dxf` - placeholder eksportera DXF.
- `LaserCad.Unity` - bazowy projekt Unity dla adaptera UI.
- `tests/LaserCad.Tests` - testy NUnit.

## Co jest juz zaimplementowane

### Geometry

- `GeometryTolerance` z `Default = 0.000001`.
- `Length` w `LaserCad.Geometry.Units`:
  - fabryki `FromMillimeters`, `FromCentimeters`, `FromInches`,
  - `Millimeters`,
  - arytmetyka `+`, `-`, `*`, `/`,
  - porownania z tolerancja,
  - formatowanie jako tekst w mm.
- `Point2D`, `Vector2D`, `BoundingBox`, `Matrix3x3`.
- `Line2D`:
  - nieskonczona linia 2D opisana przez `Point` i znormalizowany `Direction`,
  - konstruktor odrzuca kierunek zerowy przez walidacje `Vector2D.Normalize()`.
- `LineSegment2D`:
  - `Start`, `End`,
  - `Length` jako odleglosc w milimetrach domenowych,
  - `Direction` jako znormalizowany wektor od startu do konca,
  - `Bounds` jako `BoundingBox` z punktow koncowych,
  - `PointAt(double t)` dla `t` od 0 do 1,
  - `Transform(Matrix3x3)` transformuje oba konce odcinka.
- `Circle2D`:
  - `Center`, `Radius`,
  - promien musi byc dodatni; `0` i wartosci ujemne sa odrzucane,
  - `Circumference`,
  - `Bounds` jako bounding box calego okregu,
  - `Transform(Matrix3x3)` obsluguje translacje, rotacje, odbicia i jednolite skalowanie,
  - transformacje z niejednolitym skalowaniem albo scinaniem sa odrzucane, bo zmienilyby okreg w elipse.
- `ArcDirection`: `Counterclockwise`, `Clockwise`.
- `Arc2D`:
  - `Center`, `Radius`,
  - `StartAngleRadians`, `EndAngleRadians`,
  - `Direction`, domyslnie `Counterclockwise`,
  - `Length` liczona jako promien razy znormalizowany kat rozwarcia,
  - `PointAt(double t)` dla `t` od 0 do 1,
  - `Transform(Matrix3x3)` obsluguje translacje, rotacje, odbicia i jednolite skalowanie,
  - odbicie odwraca `Direction`,
  - transformacje z niejednolitym skalowaniem albo scinaniem sa odrzucane.
- `Polyline2D`:
  - niemutowalny publiczny kontrakt `Points` jako `IReadOnlyList<Point2D>`,
  - konstruktor kopiuje punkty i wymaga minimum dwoch punktow,
  - `IsClosed` decyduje, czy ostatni punkt jest laczony z pierwszym,
  - `Length` sumuje kolejne odcinki i dla zamknietej polilinii dodaje odcinek domykajacy,
  - `Bounds` obejmuje wszystkie punkty,
  - `Transform(Matrix3x3)` transformuje wszystkie punkty i zachowuje `IsClosed`.
- `PolygonOrientation`: `Clockwise`, `Counterclockwise`.
- `Polygon2D`:
  - niemutowalny publiczny kontrakt `Vertices` jako `IReadOnlyList<Point2D>`,
  - konstruktor kopiuje wierzcholki, wymaga minimum trzech i odrzuca zerowe pole,
  - `Area` liczone metoda shoelace jako wartosc dodatnia,
  - `Orientation` wynika ze znaku pola skierowanego,
  - `Bounds` obejmuje wszystkie wierzcholki,
  - `Transform(Matrix3x3)` transformuje wszystkie wierzcholki.
- `Contour2D`:
  - niemutowalny publiczny kontrakt `Points` jako `IReadOnlyList<Point2D>`,
  - konstruktor kopiuje punkty i wymaga minimum dwoch punktow,
  - `IsClosed` wykrywa domkniecie przez porownanie pierwszego i ostatniego punktu w tolerancji geometrycznej,
  - `Bounds` obejmuje wszystkie punkty konturu,
  - `FromPolygon(Polygon2D)` tworzy domkniety kontur z wierzcholkow polygonu,
  - `GetSegments()` zwraca odcinki konturu, a dla domknietego konturu domyka ostatni odcinek do pierwszego punktu,
  - `HasSelfIntersections()` wykrywa przeciecia niesasiadujacych krawedzi i traktuje tylko realne `Point`/`Overlap` jako samoprzeciecie, nie samo `Parallel`.
- `PolygonOffset2D`:
  - `OffsetOuter(Polygon2D, double distanceMillimeters)`,
  - `OffsetInner(Polygon2D, double distanceMillimeters)`,
  - obsluguje tylko proste polygony wypukle z liniowymi krawedziami,
  - odrzuca samoprzeciecia, polygony wklesle, zerowy/ujemny problematyczny ksztalt przez walidacje domenowa,
  - offset liczy jako przeciecia przesunietych krawedzi; orientacja polygonu decyduje o stronie zewnetrznej/wewnetrznej.
- Przeciecia:
  - `IntersectionKind`: `None`, `Point`, `Parallel`, `Overlap`,
  - `IntersectionResult`: `Kind`, opcjonalny `Point`, opcjonalny `OverlapSegment`, pomocnicze `IsNone`, `IsPoint`, `IsParallel`, `IsOverlap`,
  - `Intersections2D.Intersect(Line2D, Line2D)`,
  - `Intersections2D.Intersect(LineSegment2D, LineSegment2D)`,
  - `Intersections2D.Intersect(Line2D, LineSegment2D)` oraz odwrotna kolejnosc argumentow,
  - `Parallel` oznacza rownolegle niewspolliniowe obiekty i jest rozroznione od `None`,
  - `Overlap` oznacza wspolliniowosc albo nakladajacy sie zakres; dla dwoch linii nieskonczonych `OverlapSegment` jest puste, dla odcinkow zawiera wspolny odcinek,
  - pojedyncze dotkniecie koncami zwraca `Point`, nie `Overlap`.

### Core - Parametry

- `ParameterType`: `Length`, `Number`, `Boolean`, `Text`, `Choice`.
- `ParameterId`: value object ze stringiem i walidacja pustej wartosci.
- `Parameter`: `Id`, `Name`, `Type`, `Value`, opcjonalne `DisplayUnit`, `MinimumValue`, `MaximumValue`, walidacja typu i zakresu.
- `ParameterSet`: niemutowalna kolekcja z `FindById`, `FindByName`, `Add`, `UpdateValue`.

### Core - Wyrazenia

- Jawne AST: `Expression`, `ConstantExpression`, `ParameterReferenceExpression`, `BinaryExpression`.
- `BinaryOperator`: `Add`, `Subtract`, `Multiply`, `Divide`.
- `ExpressionEvaluationResult` z `Success`/`Failure`.
- `ExpressionEvaluator` ewaluuje stale, referencje i operacje binarne, `Length` liczy w milimetrach, zwraca bledy dla brakujacego parametru i dzielenia przez zero.
- `Expressions` jako fabryka jawnego AST.
- Parser tekstowy nie jest implementowany w MVP; jawne budowanie AST jest swiadomie wybrane.

### Core - Graf zaleznosci

- `DependencyGraph`: `AddDependency`, `GetDependencies`, `GetRecalculationOrder`, `CalculateRecalculationOrder`, `GetAffectedParameters`, `HasCycle`.
- `RecalculationOrderResult`: `Order`, `Error`, `IsSuccess`, `Success`, `Failure`.
- Graf wykrywa cykle i zwraca czytelny blad przez `CalculateRecalculationOrder`.

### Core - Model dokumentu

- `CadDocument`:
  - `Id`, `Name`, `FormatVersion`, `Parameters`, `Layers`, `Sketches`, `Generators`, opcjonalny `MaterialProfile`,
  - niemutujace metody `AddParameter`, `AddLayer`, `AddSketch`, `AddGenerator`, `WithMaterialProfile`,
  - `new CadDocument()` tworzy dokument z `DefaultLayers.All`,
  - jesli potrzeba pustej kolekcji warstw, przekazac jawnie `layers: Array.Empty<Layer>()`.
- `Layer`:
  - `Name`,
  - `Color` jako `LayerColor`,
  - `Role` jako `LayerRole`,
  - domyslnie kolor czarny i rola `Cut`.
- `LayerColor`:
  - value object dla koloru w formacie `#RRGGBB`,
  - normalizuje zapis do wielkich liter,
  - `Black = #000000`.
- `LayerRole`: `Cut`, `Engrave`, `Score`, `Ignore`.
- `DefaultLayers`:
  - `Cut` czerwony `#FF0000`,
  - `Engrave` niebieski `#0000FF`,
  - `Score` zielony `#00AA00`,
  - `Ignore` szary `#808080`,
  - `All` zwraca liste w kolejnosci prezentacji.
- `Sketch`: `Id`, `Name`, `Entities`, niemutujace operacje `AddEntity`, `RemoveEntity`, `CopyEntity`, `TransformEntity`, `MoveEntity`, `RotateEntity`, `ScaleEntity`, `MirrorEntity`.
- `ISketchEntity`: bazowy interfejs encji szkicu z `Id`, `LayerName`, `Bounds`, `Transform(Matrix3x3)`.
- `Entity`: abstrakcyjna baza encji szkicu, implementuje `ISketchEntity`, przechowuje `Id` i `LayerName`, wymaga `Transform(Matrix3x3)` oraz `Copy(Guid? id = null)`.
- `SketchMirrorAxis`: `X`, `Y`, uzywany przez `Sketch.MirrorEntity`.
- W MVP `LayerName` pelni role identyfikatora warstwy, bo `Layer` nadal nie ma osobnego `LayerId`.
- Encje szkicu:
  - `LineEntity` przechowuje `LineSegment2D`, `Bounds` deleguje do odcinka, `Transform` transformuje segment.
  - `RectangleEntity` przechowuje 4 narozniki; konstruktor osiowy bierze `origin`, `width`, `height`; transformacja zachowuje narozniki po transformacji.
  - `CircleEntity` przechowuje `Circle2D`; transformacja korzysta z walidacji okregu w geometrii.
  - `ArcEntity` przechowuje `Arc2D`; `Bounds` liczy punkt startu, konca i punkty kardynalne lezace na sweepie luku.
  - `PolylineEntity` przechowuje `Polyline2D`.
  - `TextEntity` jest placeholderem tekstu z `Text`, `Position`, `Height`, domyslnie na warstwie `Engrave`.
- Transformacje encji szkicu zachowuja `Id`, a kopiowanie przez `Copy`/`CopyEntity` tworzy nowa encje z nowym `Id` albo z jawnie podanym `Guid`.
- Parametryczne wymiary encji szkicu:
  - `EntityDimensionKind`: `Width`, `Height`, `Diameter`,
  - `EntityDimensionBinding`: laczy wymiar encji z `ParameterId`,
  - `Entity.DimensionBindings` przechowuje powiazania,
  - `RectangleEntity.BindDimension(...)` i `CircleEntity.BindDimension(...)` dopisuja powiazanie niemutujaco,
  - kopiowanie i transformacje prostokata/okregu przenosza powiazania wymiarow.
- Przebudowa szkicu z parametrow:
  - `RectangleEntity.RebuildFromParameters(ParameterSet)` obsluguje `Width` i `Height` z parametrow typu `Length`,
  - `CircleEntity.RebuildFromParameters(ParameterSet)` obsluguje `Diameter` z parametru typu `Length`,
  - `Sketch.RebuildFromParameters(ParameterSet)` przebudowuje prostokaty i okregi, a inne encje pozostawia bez zmian,
  - przebudowa jest niemutujaca i zachowuje `Id`, `LayerName` oraz powiazania,
  - ograniczenie MVP: prostokat jest przebudowywany jako osiowy prostokat z bounding boxa; to nie jest solver constraints i nie zachowuje dowolnych obrotow po przebudowie parametrycznej.
- `GeneratorInstance`: `Id`, `GeneratorType`, `Parameters`, niemutujace `AddParameter`.
- `MaterialProfile`:
  - `Name`, `Thickness`, `DefaultKerf`, `DefaultClearance`, `MinimumFingerWidth`,
  - wymiary sa typu `Length`,
  - wartosci ujemne sa odrzucane,
  - konstruktor zachowuje zgodnosc wsteczna przez opcjonalne parametry i domyslne `0 mm`.
- `DefaultMaterialProfiles`:
  - `Plywood3Mm` -> `Sklejka 3 mm`, grubosc 3 mm, kerf 0.15 mm, clearance 0.1 mm, minimalny palec 3 mm,
  - `Plywood4Mm` -> `Sklejka 4 mm`, grubosc 4 mm, kerf 0.16 mm, clearance 0.1 mm, minimalny palec 4 mm,
  - `Mdf` -> `MDF 3 mm`, grubosc 3 mm, kerf 0.18 mm, clearance 0.1 mm, minimalny palec 3 mm,
  - `Acrylic` -> `Akryl 3 mm`, grubosc 3 mm, kerf 0.14 mm, clearance 0.08 mm, minimalny palec 3 mm,
  - `All` zwraca liste profili w kolejnosci prezentacji.

### Core - Zapis i odczyt projektu

- Format pliku projektu MVP to JSON oparty o `System.Text.Json`.
- `DocumentSerializer` w `src/LaserCad.Core/Documents/DocumentSerializer.cs`:
  - `SupportedFormatVersion = 1`,
  - `Serialize(CadDocument)` zapisuje dokument do indented camelCase JSON,
  - `Deserialize(string)` odtwarza dokument przez konstruktory domenowe,
  - odrzuca nieobslugiwana wersje formatu przez `NotSupportedException` przy zapisie i odczycie.
- Aktualnie serializowane pola dokumentu:
  - `formatVersion`,
  - `id`,
  - `name`,
  - `parameters`,
  - `layers`,
  - `sketches`,
  - opcjonalny `materialProfile`.
- Parametry sa zapisywane jako DTO z `id`, `name`, `type`, `value`, opcjonalnym `displayUnit`, `minimumValue`, `maximumValue`.
- `Length` w parametrach i profilu materialu jest zapisywany jako liczba milimetrow.
- Warstwy zapisuja `name`, `color` jako `#RRGGBB` i `role`.
- Profil materialu zapisuje:
  - `name`,
  - `thicknessMillimeters`,
  - `defaultKerfMillimeters`,
  - `defaultClearanceMillimeters`,
  - `minimumFingerWidthMillimeters`.
- Jesli JSON nie ma pola `layers`, `CadDocument` odtworzy domyslne warstwy; jesli ma `"layers": []`, zachowa jawnie pusta liste.
- Szkice sa serializowane w `sketches` jako lista `id`, `name`, `entities`.
- Encje szkicu maja wspolny kontrakt DTO:
  - `type`,
  - `id`,
  - `layerName`,
  - opcjonalne `dimensionBindings`.
- Obslugiwane typy encji w JSON:
  - `Line` z `start`, `end`,
  - `Rectangle` z `corners`,
  - `Circle` z `center`, `radius`,
  - `Arc` z `center`, `radius`, `startAngleRadians`, `endAngleRadians`, `direction`,
  - `Polyline` z `points`, `isClosed`,
  - `Text` z `text`, `position`, `height`.
- Punkty sa zapisywane jako `{ "x": ..., "y": ... }`.
- Powiazania wymiarow encji zapisuja `dimension` i `parameterId`; round-trip zachowuje powiazania prostokata i okregu.
- Generatory nadal nie sa jeszcze serializowane.

### Unity - baza

- Dodano bazowy projekt `LaserCad.Unity`.
- `Packages/manifest.json` zawiera minimalne pakiety Unity: 2D Sprite, TextMeshPro i uGUI.
- `Assets/Scenes/Workspace.unity` jest scena robocza 2D z kamera ortograficzna i rootem aplikacji.
- `Assets/Scripts/LaserCad.Unity.asmdef` definiuje assembly `LaserCad.Unity` z referencjami do `LaserCad.Core` i `LaserCad.Geometry`.
- `Assets/Plugins/LaserCad.Domain/README.md` opisuje podpiecie DLL domenowych do Unity; binarne DLL nie sa commitowane.
- `LaserCadApplicationController` tworzy pusty `CadDocument` przy starcie.
- `DocumentInfoView` wyswietla podstawowe informacje o dokumencie: nazwe, wersje formatu, liczbe warstw i szkicow.
- `Docs/DOMAIN_BOUNDARY.md` opisuje granice: Unity jest adapterem UI i nie dubluje logiki geometrycznej ani walidacji domenowej.
- Projekt Unity nie zostal uruchomiony w edytorze w tej sesji; zweryfikowano tylko testy domenowe `dotnet test LaserCad.sln`.

### Unity - kamera

- Dodano `WorkspaceCameraController` w `LaserCad.Unity/Assets/Scripts`.
- Kontroler:
  - wymusza kamere ortograficzna,
  - ustawia poczatkowy `orthographicSize = 100`,
  - obsluguje zoom kolkiem myszy,
  - obsluguje pan widoku srodkowym albo prawym przyciskiem myszy,
  - resetuje widok klawiszem `Home`,
  - ogranicza zoom do zakresu `5..500`.
- `WorkspaceCameraController` jest podpiety do `Main Camera` w `Assets/Scenes/Workspace.unity`.
- Dodano `WorkspaceCameraController.cs.meta` ze stabilnym GUID `404b08749df74d32ad64bf4b79e0ec2c`, bo scena odwoluje sie do skryptu przez ten GUID.
- Dodano `LaserCad.Unity/Docs/QA_CHECKLIST.md` z manualnym testem kamery.
- Sekcja `3.1.*` zostala odhaczona w `TASKS.md`.
- Po przegladzie planu dodano i odhaczono brakujacy task `3.1.6 Podpiac kontroler kamery do sceny roboczej`, bo sam skrypt bez komponentu w scenie nie uruchamialby zachowania kamery.
- Po uwadze uzytkownika potraktowano brak widocznosci Unity w `LaserCad.sln` jako blad ergonomii repo:
  - dodano do `LaserCad.sln` folder solution `LaserCad.Unity`,
  - folder pokazuje najwazniejsze pliki Unity: `README.md`, scene `Workspace.unity`, asmdef, skrypty i dokumenty Unity,
  - dopisano i odhaczono task `3.1.7 Pokazac projekt Unity w solution jako folder projektu`.
- Wazne: `LaserCad.Unity` nadal nie jest projektem `.csproj` budowanym przez `dotnet`; uruchamia sie go przez Unity Hub/Unity Editor z katalogu `C:\borys\CAD\LaserCad.Unity`.
- Po pierwszym otwarciu Unity wyszly dwa bledy integracji i zostaly naprawione:
  - skrypty Unity nie uzywaja juz file-scoped namespace ani nullability skladni wymagajacej C# 10; sa zgodne z C# 9 Unity,
  - `LaserCad.Core` i `LaserCad.Geometry` multi-targetuja `net9.0;netstandard2.1`,
  - `Directory.Build.props` ustawia domyslny `net9.0` tylko dla projektow bez wlasnego targetu oraz `LangVersion=latest` dla buildow .NET SDK,
  - dodano shimy `IsExternalInit` dla `netstandard2.1`,
  - zastapiono `ArgumentNullException.ThrowIfNull` klasyczna walidacja kompatybilna z `netstandard2.1`,
  - `LaserCad.Core` ma target kopiujacy DLL do `LaserCad.Unity/Assets/Plugins/LaserCad.Domain` po buildzie `netstandard2.1`,
  - DLL pluginow Unity sa lokalnymi artefaktami i sa ignorowane przez `.gitignore`.
- Aby odswiezyc DLL domenowe dla Unity, uruchomic:
  `dotnet build src\LaserCad.Core\LaserCad.Core.csproj -f netstandard2.1`
- Po tej komendzie lokalnie powinny istniec:
  - `LaserCad.Unity/Assets/Plugins/LaserCad.Domain/LaserCad.Core.dll`,
  - `LaserCad.Unity/Assets/Plugins/LaserCad.Domain/LaserCad.Geometry.dll`.
- `MVP.0.11` nadal nie zostal odhaczony po `3.1`, bo jest juz podstawowa kamera/widok, ale nie ma jeszcze renderowania geometrii 2D ani weryfikacji w edytorze Unity.

## Komentarze XML

- Publiczne API w `src` ma polskie komentarze XML.
- Placeholdery `Class1` w projektach sa opisane jako tymczasowe i nie powinny byc uzywane w nowym kodzie.

## Ostatnio wykonane sekcje

- `0.5.*` - parametry.
- `0.6.*` - wyrazenia parametryczne.
- `0.7.*` - graf zaleznosci parametrow.
- `0.8.*` - model dokumentu.
- `0.9.*` - warstwy i profile materialow.
- `0.10.*` - zapis i odczyt projektu JSON.
- `1.0.*` - linie i odcinki.
- `1.1.*` - przeciecia linii i odcinkow.
- `1.2.*` - okregi i luki.
- `1.3.*` - polilinie i polygony.
- `1.4.*` - offsety i kontury.
- `2.0.*` - encje szkicu.
- `2.1.*` - operacje na szkicu.
- `2.3.*` - trwalosc szkicow w JSON.
- `3.0.*` - integracja Unity - baza.
- Dodatkowy commit bez numeru taska: `f970787 Dodaj polskie komentarze XML do API`.
- Dodatkowy commit po `0.10`: `44efd0f Doprecyzuj plan i wersje formatu po 0.10`:
  - odhaczyl `MVP.0.6`,
  - dodal symetryczna walidacje nieobslugiwanej wersji przy `Serialize`.
- Dodatkowy commit po `1.0`: `e0e0e28 Odhacz sekcje 1.0 w planie`.
- Dodatkowy commit po `1.1`: `b6f84ba Odhacz sekcje 1.1 w planie`:
  - odhaczyl cala sekcje `1.1.*`,
  - odhaczyl zalegle `MVP.0.1`, bo `Length`, `Point2D`, `Vector2D`, `BoundingBox` i `Matrix3x3` sa juz zaimplementowane.
- Dodatkowy commit po `1.2`: `376fcc9 Odhacz sekcje 1.2 w planie`:
  - odhaczyl cala sekcje `1.2.*`,
  - `MVP.0.2` zostal nieodhaczony, bo nadal brakuje prostokata i polilinii jako podstawowych encji.
- Commit po `1.3`: `df77caf 1.3 Dodaj polilinie i polygony`:
  - dodal `Polyline2D`, `Polygon2D`, `PolygonOrientation`,
  - odhaczyl cala sekcje `1.3.*`,
  - doprecyzowal plan o `1.3.12` bounding box polygonu i `1.3.13` transformacje polygonu,
  - `MVP.0.2` nadal zostal nieodhaczony, bo brakuje jeszcze prostokata oraz encji szkicu.
- Commity po `1.4`:
  - `2f3ea6a 1.4.0 Dodaj kontury 2D`,
  - `323c95c 1.4.3 Dodaj offset polygonu wypuklego`,
  - `de601cc 1.4.8 Opisz ograniczenia offsetu MVP`.
- Commit po `2.0`: `8e677be 2.0 Dodaj encje szkicu`:
  - dodal `ISketchEntity`, wspolne `LayerName`, `Bounds` i `Transform(Matrix3x3)`,
  - dodal `LineEntity`, `RectangleEntity`, `CircleEntity`, `ArcEntity`, `PolylineEntity`, `TextEntity`,
  - odhaczyl cala sekcje `2.0.*`,
  - odhaczyl zalegle `2.1.0`, bo `Sketch.AddEntity` juz istnieje,
  - odhaczyl `MVP.0.2`, bo podstawowe encje line/rectangle/circle/polyline sa zaimplementowane,
  - dopisal w `TASKS.md` sekcje `2.3 Trwalosc szkicow`.
- Commit po `2.1`: `763ac97 2.1 Dodaj operacje na szkicu`:
  - dodal `Entity.Copy(Guid? id = null)` oraz implementacje kopiowania dla wszystkich encji szkicu,
  - dodal `SketchMirrorAxis`,
  - dodal niemutujace operacje `Sketch.RemoveEntity`, `CopyEntity`, `TransformEntity`, `MoveEntity`, `RotateEntity`, `ScaleEntity`, `MirrorEntity`,
  - dodal testy dodawania/usuwania, kopiowania i transformacji encji,
  - odhaczyl cala sekcje `2.1.*`,
  - po przegladzie planu odhaczyl zalegle `MVP.0.0`, `MVP.0.4` i `MVP.0.5`.
- Sekcja `1.4.*` zostala odhaczona w `TASKS.md`.
- Do `TASKS.md` dopisano i odhaczono `1.4.9` na test odrzucenia polygonu niewypuklego przez offset MVP.
- `docs/ROADMAP.md` opisuje ograniczenia offsetu MVP: tylko zamkniete, proste, wypukle polygony z krawedziami liniowymi; bez luk, otwartych polilinii, polygonow wkleslych, samoprzeciec i pelnej kompensacji kerfu.
- `docs/ROADMAP.md` opisuje tez ograniczenia parametrycznego szkicu MVP: tylko szerokosc/wysokosc prostokata i srednica okregu z parametrow `Length`, bez pelnego solvera constraints.
- Po przegladzie planu po `2.2` dodano do `TASKS.md` task `2.3.8` na serializacje powiazan wymiarow encji z parametrami oraz dopisano uwage do `docs/PROJECT_FILE_FORMAT.md`.
- Commity po `2.3`:
  - `ca9b8b2 2.3.0 Ustal kontrakt JSON szkicow`,
  - `fd5b47f 2.3 Dodaj trwalosc szkicow`.
- Sekcja `2.3.*` zostala odhaczona w `TASKS.md`.
- `DocumentSerializer` zapisuje i odczytuje szkice oraz encje `LineEntity`, `RectangleEntity`, `CircleEntity`, `ArcEntity`, `PolylineEntity`, `TextEntity`.
- Testy dodane w `DocumentSerializerTests`:
  - `RoundTrip_WithSketchAndEntities_ShouldPreserveSketchGeometry`,
  - `Serialize_WithSketchEntities_ShouldWriteEntityTypes`,
  - `RoundTrip_WithDimensionBindings_ShouldPreserveBindings`.
- Commity po `3.0`:
  - `4ce8153 3.0.0 Utworz projekt Unity`,
  - `1b4cd9e 3.0.1 Podlacz biblioteki domenowe do Unity`,
  - `e36ff5f 3.0.2 Utworz scene robocza Unity`,
  - `a50ef37 3.0.3 Dodaj kontroler aplikacji Unity`,
  - `ab14406 3.0.4 Zaladuj pusty dokument w Unity`,
  - `de57c24 3.0.5 Wyswietl informacje o dokumencie w UI`,
  - `da4e919 3.0.6 Opisz granice logiki Unity`.
- Sekcja `3.0.*` zostala odhaczona w `TASKS.md`.
- Commity po `3.1`:
  - `8972b73 3.1.0 Dodaj kamere ortograficzna`,
  - `a36a050 3.1.1 Dodaj zoom kolkiem myszy`,
  - `cfa2efb 3.1.2 Dodaj pan kamery`,
  - `953696f 3.1.3 Dodaj reset widoku`,
  - `f469328 3.1.4 Ogranicz zoom kamery`,
  - `3010730 3.1.5 Dodaj checklist kamery QA`,
  - `50649d4 3.1.6 Podlacz kontroler kamery do sceny`,
  - `b85a70f 3.1.7 Pokaz projekt Unity w solution`.
- Dodatkowy commit po pierwszym uruchomieniu Unity:
  - `8a75352 Napraw kompatybilnosc projektu Unity`.
- Sekcja `3.1.*` zostala odhaczona w `TASKS.md`.
- `MVP.0.11` nie zostal odhaczony po `3.0`, bo jest tylko baza Unity; nie bylo jeszcze weryfikacji w edytorze ani renderowania geometrii 2D.
- `MVP.0.9` nadal nie zostal odhaczony, bo istnieje tylko podstawowy offset polygonow wypuklych; nie ma jeszcze pelnej kompensacji kerfu, klasyfikacji konturow i obslugi otworow.

## Nastepny logiczny krok

Nastepna niewykonana sekcja wedlug kolejnosci TASKS to `3.2 Grid`.

Praktyczna uwaga po przegladzie planu: po `3.1` jest juz kamera Unity, ale priorytet produktowy moze byc dwojaki. Mozna isc zgodnie z kolejnoscia do `3.2 Grid`, albo swiadomie przeskoczyc do eksportu SVG `4.0/4.1`, jesli priorytetem jest szybciej uzyskac produkcyjny plik wyjsciowy.

## Komendy pomocnicze

```powershell
git status --short --branch
dotnet test LaserCad.sln
git log --oneline -20
```

## Oczekiwany styl finalnej odpowiedzi w nastepnej sesji

- Krotko po polsku.
- Podac zakres zrobionych taskow, najwazniejsze typy/metody i wynik testow.
- Jesli push byl wykonany, podac zakres push i potwierdzic czysty status.
