# Audyt UI desktop shell

Audyt dla sekcji `3.10 Porzadek UI i przestrzen robocza`.

## Widoczne domyslnie

- Menu glowne: `File`, `Edit`, `View`, `Tools`, `Export`, `Help`.
- Toolbar: nowy projekt, otworz, zapisz, zaznaczanie, czysty widok roboczy, rysowanie prostokata, linii i okregu, usuwanie zaznaczenia, eksport SVG i DXF.
- Lewy panel domyslny: `Material i warstwy`.
- Prawy panel domyslny: `Properties`.
- Centralny obszar: osadzony viewport Unity z loaderem startowym.

## Panele ukryte domyslnie

- `Parametry pudelka`: dostep przez `View -> Box Generator Panel`.
- `Biblioteka szablonow`: dostep przez `View -> Template Library Panel`.
- `Kerf`, `Kalibracja kerfu`, `Transformacje`: dostep przez `View -> Advanced Panels`.
- `Historia`: dostep przez `View -> History Panel`.

## Akcje end-to-end

- `File -> New`, `Open`, `Save`, `Save As`, `Exit`.
- `Edit -> Undo`, `Redo`.
- `View -> Reset View`, `Zoom To Fit`, `Grid`, `Clean Workspace`.
- Przelaczniki paneli w menu `View`.
- `Tools -> Settings`.
- `Export -> SVG`, `Export -> DXF`.
- `Help -> About Laser CAD`.
- Rysowanie podstawowych ksztaltow z toolbaru przez viewport.
- Usuwanie zaznaczonych encji na podstawie zaznaczenia z viewportu.

Wszystkie widoczne pozycje menu, przyciski toolbaru i przyciski widocznych domyslnie paneli maja przypisany handler albo sa swiadomie tylko wyswietleniem stanu.

## Widoczne placeholdery albo slabe miejsca

- Panel `Properties` ma przycisk `Odswiez zaznaczenie`, bo zaznaczenie z viewportu nie jest jeszcze w pelni reaktywne w shellu.

## Decyzje po audycie

- Domyslny widok pozostaje czysty: viewport, material/warstwy i properties.
- Generatory, historia oraz konfiguracja zaawansowana sa dostepne jawnie z menu `View`.
- `Help -> About Laser CAD` ma realne okno informacyjne od `3.10.7`.
- Po `3.10.8` nie ma widocznych domyslnie akcji bez przeplywu end-to-end.
- Po `3.10.9` okno `Settings` pokazuje tylko dzialajaca liste skrotow; edycja ustawien pozostaje poza aktualnym zakresem.
