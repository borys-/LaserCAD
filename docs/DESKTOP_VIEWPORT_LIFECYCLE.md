# Desktop shell + Unity viewport - lifecycle

## Procesy

`LaserCad.Desktop` jest procesem nadrzednym. Przechowuje stan workflow aplikacji, pokazuje menu, toolbar, panele parametrow, materialy, warstwy, properties oraz eksport.

Unity player jest procesem viewportu. Jest uruchamiany przez shell z argumentem `--viewport` i znajduje sie w katalogu `Viewport` obok opublikowanej aplikacji desktopowej. Dla uzytkownika viewport jest czescia glownego okna aplikacji: shell automatycznie uruchamia proces i osadza jego okno w centralnym panelu roboczym.

## Uruchamianie

Shell szuka viewportu pod sciezka:

```text
<katalog aplikacji desktop>\Viewport\LaserCad.exe
```

Po zaladowaniu glownego okna shell uruchamia Unity z argumentem `--viewport`, osadza okno procesu w centralnym panelu i wysyla aktualny dokument przez IPC. Zamkniecie okna desktop shell zamyka proces viewportu.

Techniczne sterowanie procesem viewportu nie jest elementem docelowego UI uzytkownika. Uzytkownik ma widziec obszar roboczy CAD, a nie recznie zarzadzac osobnym procesem.

## Znane problemy

Osadzanie Unity jako child-window WPF/WinForms ma problem z przekazywaniem wejscia po minimalizacji i maksymalizacji glownego okna. Wykonano trzy proby obejscia:

- ustawianie focusu na oknie Unity po najechaniu albo kliknieciu panelu viewportu,
- ponowne ustawianie focusu po `Activated` i `StateChanged`,
- forwardowanie `WM_MOUSEWHEEL` do okna Unity, gdy kursor jest nad panelem viewportu.

Wedlug recznej weryfikacji uzytkownika problem nadal wystepuje: po minimalizacji i maksymalizacji okna scroll/zoom viewportu moze nie dzialac. Tego nie nalezy traktowac jako naprawione. Kolejny kierunek to zmiana sposobu osadzania albo transportu wejscia, np. dedykowany panel hostujacy z pelnym message pump, inny mechanizm embedowania Unity, albo render viewportu bez natywnego child-window.

## IPC MVP

Pierwszy adapter IPC uzywa plikow JSON lines:

```text
%LOCALAPPDATA%\LaserCad\viewport-outbox.jsonl
%LOCALAPPDATA%\LaserCad\viewport-inbox.jsonl
```

Shell wysyla do outbox:

- `DocumentSnapshot` z JSON aktualnego dokumentu CAD,
- `ViewCommand` dla resetu widoku, zoom to fit i wlaczenia/wylaczenia gridu.

Viewport wysyla do inbox:

- `SelectionChanged` z lista identyfikatorow zaznaczonych encji.

Ten transport jest celowo prosty dla MVP. Kontrakt komunikatow znajduje sie w `LaserCad.ViewportContract` i moze zostac przeniesiony na named pipes albo WebSocket bez zmiany znaczenia komunikatow.

## Granice odpowiedzialnosci

Desktop shell odpowiada za:

- glowne menu i toolbar,
- panele parametrow, materialow, warstw, historii i properties,
- dialogi plikow,
- eksport SVG/DXF,
- uruchamianie, restart i zamykanie viewportu,
- wysylanie aktualnego dokumentu do viewportu.

Unity viewport odpowiada za:

- kamere,
- grid,
- snap,
- zaznaczanie,
- highlight,
- renderowanie dokumentu,
- docelowy podglad 3D.

Panele IMGUI w Unity zostaja jako debugowe panele uruchomienia bez `--viewport`. W trybie viewportu nie rysuja docelowego UI aplikacji.
