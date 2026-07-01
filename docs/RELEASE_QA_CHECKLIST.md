# Checklista QA przed wydaniem

Ta checklista opisuje minimalna reczna weryfikacje przed oznaczeniem wersji aplikacji jako gotowej do wydania.

## Build i testy

- [ ] Uruchomic `dotnet test LaserCad.sln --no-restore`.
- [ ] Uruchomic `dotnet build LaserCad.sln --no-restore`.
- [ ] Uruchomic `cmd /c build.bat`.
- [ ] Potwierdzic, ze istnieje `bin/release/LaserCad.Desktop/LaserCad.Desktop.exe`.
- [ ] Potwierdzic, ze istnieje `bin/release/LaserCad.Desktop/Viewport/LaserCad.exe`.

## Start aplikacji

- [ ] Uruchomic `bin/release/LaserCad.Desktop/LaserCad.Desktop.exe`.
- [ ] Potwierdzic, ze desktop shell otwiera sie bez bledow.
- [ ] Potwierdzic, ze viewport Unity startuje w obszarze roboczym.
- [ ] Potwierdzic, ze status aplikacji przechodzi do `Gotowe`.

## Ustawienia i skroty

- [ ] Otworzyc `Tools -> Settings...`.
- [ ] Potwierdzic, ze widac sekcje widoku roboczego, jednostek i skrotow klawiszowych.
- [ ] Potwierdzic, ze `Ctrl+N` tworzy nowy projekt.
- [ ] Potwierdzic, ze `Ctrl+O` otwiera dialog projektu.
- [ ] Potwierdzic, ze `Ctrl+S` zapisuje projekt albo przechodzi do `Save As`, jesli projekt nie ma sciezki.
- [ ] Potwierdzic, ze `Ctrl+Z` i `Ctrl+Y` obsluguja undo/redo.
- [ ] Potwierdzic, ze `Home` resetuje widok.
- [ ] Potwierdzic, ze `Ctrl+0` wysyla zoom to fit.

## Minimalny workflow CAD

- [ ] Wybrac material `Sklejka 3 mm`.
- [ ] Zmienic szerokosc, glebokosc i wysokosc pudelka.
- [ ] Kliknac `Przebuduj`.
- [ ] Potwierdzic, ze viewport pokazuje geometrie 2D pudelka.
- [ ] Zmienic parametr jeszcze raz i potwierdzic przebudowe viewportu.
- [ ] Dodac prostokat, linie i okrag narzedziami z toolbaru.
- [ ] Zaznaczyc encje w viewportcie i usunac ja z desktop shell.

## Pliki projektu

- [ ] Zapisac projekt jako `*.lasercad.json`.
- [ ] Otworzyc zapisany projekt ponownie.
- [ ] Potwierdzic, ze dokument, material i geometria sa odtworzone.
- [ ] Otworzyc przykładowy projekt `examples/projects/open-box.lasercad.json`.

## Eksport

- [ ] Wyeksportowac SVG.
- [ ] Otworzyc SVG w zewnetrznym programie i potwierdzic skale w milimetrach.
- [ ] Potwierdzic, ze warstwa `Cut` jest eksportowana kolorem czerwonym.
- [ ] Potwierdzic, ze tekst/grawer trafia na warstwe `Engrave`, jesli jest uzyty.
- [ ] Wyeksportowac DXF.
- [ ] Potwierdzic, ze DXF zawiera warstwy i podstawowa geometrie.

## Biblioteka i produkcja

- [ ] W panelu `Biblioteka szablonow` wybrac szablon pudelka i kliknac `Uzyj szablonu`.
- [ ] Powtorzyc dla organizera, podstawki i stojaka.
- [ ] Potwierdzic, ze lista materialow nie duplikuje profili sklejki 3 mm i 4 mm.
- [ ] Wykonac pomiar kalibracji kerfu i zapis rekomendacji do profilu w biezacej sesji.

## Akceptacja

- [ ] Wszystkie automatyczne testy przechodza.
- [ ] Build release generuje desktop shell i viewport.
- [ ] Minimalny workflow material -> parametry -> przebudowa -> eksport SVG dziala end-to-end.
- [ ] Znane ograniczenia MVP sa nadal opisane w `docs/ROADMAP.md`.
