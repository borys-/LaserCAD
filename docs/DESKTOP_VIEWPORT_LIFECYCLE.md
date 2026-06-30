# Desktop shell + Unity viewport - lifecycle

## Procesy

`LaserCad.Desktop` jest procesem nadrzednym. Przechowuje stan workflow aplikacji, pokazuje menu, toolbar, panele parametrow, materialy, warstwy, properties oraz eksport.

Unity player jest procesem viewportu. Jest uruchamiany przez shell z argumentem `--viewport` i znajduje sie w katalogu `Viewport` obok opublikowanej aplikacji desktopowej.

## Uruchamianie

Shell szuka viewportu pod sciezka:

```text
<katalog aplikacji desktop>\Viewport\LaserCad.exe
```

Klikniecie `Start viewport` uruchamia Unity z argumentem `--viewport`. Klikniecie `Restart` zamyka aktualny proces i uruchamia nowy. Klikniecie `Stop` zamyka proces viewportu. Zamkniecie okna desktop shell rowniez zamyka proces viewportu.

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
