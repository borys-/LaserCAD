# Standardy kodowania

Ten dokument jest obowiazujacym standardem pracy nad projektem Laser CAD. Kazda kolejna zmiana w kodzie, testach i dokumentacji powinna byc z nim zgodna.

## Priorytety projektu

- Kod domenowy ma byc prosty do testowania bez Unity.
- Logika CAD, geometria, parametry i eksport maja byc niezalezne od UI.
- Parametrycznosc jest fundamentem projektu, nie dodatkiem.
- Wszystkie operacje geometryczne maja byc deterministyczne i testowalne.
- Czytelnosc i poprawna domena sa wazniejsze niz przedwczesna optymalizacja.

## Struktura projektow

- `LaserCad.Geometry` zawiera typy matematyczne i geometryczne.
- `LaserCad.Core` zawiera model dokumentu, parametry, warstwy, generatory i logike CAD.
- `LaserCad.Export.Svg` zawiera eksport SVG.
- `LaserCad.Export.Dxf` zawiera eksport DXF.
- `LaserCad.Tests` zawiera testy jednostkowe dla bibliotek domenowych.
- Projekty eksportu moga zalezec od `LaserCad.Core`.
- `LaserCad.Core` moze zalezec od `LaserCad.Geometry`.
- Biblioteki domenowe nie moga zalezec od Unity.
- Zaleznosc moze isc w strone `LaserCad.Unity` -> biblioteki domenowe, ale nigdy odwrotnie.
- Projekty domenowe nie moga referencjonowac `UnityEngine`, assetow Unity ani typow UI.

## Namespace

Namespace powinien zaczynac sie od nazwy solution `LaserCad`.

Konwencja:

- projekt `LaserCad.Core` uzywa namespace `LaserCad.Core`,
- projekt `LaserCad.Geometry` uzywa namespace `LaserCad.Geometry`,
- projekt `LaserCad.Export.Svg` uzywa namespace `LaserCad.Export.Svg`,
- projekt `LaserCad.Export.Dxf` uzywa namespace `LaserCad.Export.Dxf`,
- projekt `LaserCad.Tests` uzywa namespace `LaserCad.Tests`.

Dla podkatalogow nalezy dopisywac kolejne segmenty namespace zgodnie ze struktura domeny, np. `LaserCad.Geometry.Primitives`.

## Nazewnictwo

Konwencje nazewnictwa:

- klasy, rekordy, struktury, enumy i interfejsy: `PascalCase`,
- metody i properties: `PascalCase`,
- parametry metod i zmienne lokalne: `camelCase`,
- pola prywatne: `_camelCase`,
- stale: `PascalCase`,
- interfejsy: prefiks `I`, np. `IExporter`.

Nazwy powinny opisywac intencje domenowa, np. `MaterialThickness`, `Kerf`, `FingerWidth`, zamiast skrotow technicznych.

## Styl C#

- Uzywac file-scoped namespace.
- Uzywac `nullable enable` z konfiguracji projektu.
- Preferowac `readonly struct` albo `record struct` dla malych niezmiennych typow wartosciowych, np. `Point2D`, `Vector2D`, `Length`.
- Preferowac niezmiennosc obiektow domenowych, jesli nie utrudnia to modelowania.
- Publiczne API powinno byc jawne i male.
- Unikac setterow publicznych, jesli obiekt moze byc utworzony w poprawnym stanie przez konstruktor lub fabryke.
- Nie uzywac magicznych liczb w logice domenowej; przenosic je do nazwanych stalych albo opcji.
- Nie dodawac abstrakcji bez realnej potrzeby domenowej albo testowej.
- Komentarze dodawac tylko tam, gdzie wyjasniaja nietrywialna decyzje, tolerancje albo ograniczenie algorytmu.

## Jednostki i liczby

- Wewnetrzna jednostka dlugosci to milimetry.
- Wszystkie wymiary domenowe, zapis projektu i eksport produkcyjny powinny traktowac wartosci dlugosci jako milimetry, chyba ze API jawnie mowi inaczej.
- Centymetry, cale i inne jednostki moga pojawiac sie tylko na granicy systemu jako jawna konwersja do milimetrow.
- Nie uzywac surowego `double` w publicznym API, jesli wartosc oznacza fizyczna dlugosc.
- Docelowo uzywac typu `Length` dla wymiarow.
- Parametry takie jak `Kerf`, `MaterialThickness`, `Clearance` i `FingerWidth` musza miec jednoznaczna jednostke.
- Porownania geometryczne musza uzywac tolerancji, a nie prostego `==` na liczbach zmiennoprzecinkowych.

## Geometria

- Typy geometryczne powinny byc male, przewidywalne i latwe do testowania.
- Operacje geometryczne nie powinny modyfikowac wejscia; powinny zwracac nowy wynik.
- Metody przeciec, offsetow i transformacji powinny jawnie obslugiwac przypadki brzegowe.
- Wyniki operacji, ktore moga sie nie udac, powinny zwracac czytelny typ wyniku albo blad domenowy.
- Nie ukrywac tolerancji geometrycznej wewnatrz przypadkowych metod; tolerancja ma byc wspolna i nazwana.

## Parametrycznosc

- Parametry dokumentu powinny miec stabilne identyfikatory.
- Wyrazenia parametryczne powinny byc odtwarzalne po zapisie i odczycie projektu.
- Zmiana parametru powinna przebudowywac tylko zalezne elementy, gdy bedzie dostepny graf zaleznosci.
- Bledy parametrow, np. cykle zaleznosci albo niepoprawny zakres, maja byc raportowane jako bledy domenowe.
- Generatory powinny pozostawac edytowalnymi instancjami parametrycznymi, a nie tylko zbiorem statycznych linii.

## Bledy i walidacja

- Walidowac dane na granicach publicznego API.
- Dla niepoprawnych argumentow uzywac standardowych wyjatkow, np. `ArgumentException`, `ArgumentOutOfRangeException`, `ArgumentNullException`.
- Dla przewidywalnych bledow domenowych preferowac typ wyniku zamiast wyjatku, jesli blad jest normalna czescia workflow.
- Komunikaty bledow powinny podawac nazwe problematycznego parametru lub operacji.

## Testy

- Testy pisac w NUnit.
- Moq jest domyslna biblioteka do mockowania zaleznosci.
- Nazwy testow powinny opisywac zachowanie, np. `Length_FromCentimeters_ReturnsMillimeters`.
- Test powinien sprawdzac jedno zachowanie.
- Preferowany uklad testu: arrange, act, assert.
- Testy geometrii powinny uwzgledniac tolerancje.
- Kazdy nowy typ domenowy powinien miec testy dla podstawowych zachowan i przypadkow brzegowych.
- Przed commitem uruchamiac `dotnet test LaserCad.sln`, jesli zmiana dotyka kodu albo konfiguracji projektow.

## Eksport

- Eksportery nie powinny zawierac logiki edycji dokumentu.
- Eksportery czytaja model domenowy i produkuja wynik w formacie docelowym.
- SVG i DXF powinny respektowac jednostki w milimetrach.
- Warstwy `cut`, `engrave`, `score` i `ignore` musza miec jednoznaczne mapowanie w eksporcie.
- Warstwa `ignore` nie powinna byc eksportowana do plikow produkcyjnych, chyba ze opcja eksportu mowi inaczej.

## Dokumentacja

- Dokumentacja projektu jest pisana po polsku.
- Pliki techniczne moga uzywac angielskich nazw typow, metod i pojec domenowych.
- Dokumenty Markdown powinny byc czytelne w terminalu i na GitHubie.
- Nowe decyzje projektowe zapisywac w `README.md`, `docs/CODING_STANDARDS.md` albo odpowiednim dokumencie w `docs`.

## Teksty UI

- Teksty widoczne dla uzytkownika w GUI powinny byc po polsku, jezeli istnieje naturalna polska nazwa.
- Angielskie nazwy moga zostac tylko tam, gdzie sa standardem technicznym albo formatem pliku, np. DXF, SVG, kerf, snap, zoom.
- Nazwy menu, paneli, przyciskow, etykiet pol i komunikatow powinny byc spojne w calym desktop shell.
- Publiczne API i nazwy klas moga pozostac po angielsku, jezeli lepiej pasuja do kodu i istniejacych kontraktow domenowych.

## Git i taski

- Jeden task z `TASKS.md` powinien odpowiadac jednemu commitowi.
- Kazdy fix, zmiana techniczna, zmiana w kodzie, konfiguracji albo dokumentacji powinna byc osobnym commitem z opisem po polsku.
- Kazdy zaakceptowany task nalezy wypchnac na GitHuba.
- Wiadomosc commita powinna zaczynac sie od numeru taska i byc po polsku, np. `0.2.2 Dodaj editorconfig`.
- Dla zmian organizacyjnych bez numeru taska commit powinien miec krotki polski opis.
- Nie mieszac kilku taskow w jednym commicie, chyba ze uzytkownik wyraznie zdecyduje inaczej.
- Przed commitem sprawdzic `git status --short --branch`.

## Formatowanie i pliki

- Preferowac ASCII w plikach, dopoki projekt nie przejdzie swiadomie na pelne polskie znaki w UTF-8.
- Nie commitowac katalogow `bin`, `obj`, cache IDE ani plikow tymczasowych.
- Nie zostawiac pustych klas generowanych przez szablony, jesli nie sa potrzebne.
- Pliki projektowe powinny dziedziczyc wspolna konfiguracje z `Directory.Build.props`, gdy to mozliwe.

## Zasada koncowa

Jesli standard nie opisuje konkretnej sytuacji, nalezy wybrac rozwiazanie najbardziej spojne z istniejacym kodem, najprostsze do przetestowania i najmniej zaskakujace dla kolejnego programisty.
