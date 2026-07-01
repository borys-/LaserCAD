# Desktop clean UI - QA

Manualna checklista dla sekcji `3.10 Porzadek UI i przestrzen robocza`.

## Start aplikacji

- Uruchom `bin\release\LaserCad.Desktop\LaserCad.Desktop.exe`.
- Sprawdz, ze domyslny widok ma duzy centralny viewport.
- Sprawdz, ze po lewej widoczny jest panel `Material i warstwy`.
- Sprawdz, ze po prawej widoczny jest panel `Properties`.
- Sprawdz, ze panele `Parametry pudelka`, `Biblioteka szablonow`, `Kerf`, `Kalibracja kerfu`, `Transformacje` i `Historia` nie sa widoczne po czystym starcie.

## Menu View

- Wlacz `View -> Box Generator Panel` i sprawdz, ze pojawia sie panel `Parametry pudelka`.
- Wlacz `View -> Template Library Panel` i sprawdz, ze pojawia sie panel `Biblioteka szablonow`.
- Wlacz `View -> Advanced Panels` i sprawdz, ze pojawiaja sie panele `Kerf`, `Kalibracja kerfu` i `Transformacje`.
- Wlacz `View -> History Panel` i sprawdz, ze pojawia sie panel `Historia`.
- Kliknij `View -> Clean Workspace` albo przycisk czystego widoku w toolbarze i sprawdz, ze dodatkowe panele znikaja.

## Preferencje widocznosci

- Wlacz dowolny opcjonalny panel.
- Zamknij aplikacje.
- Uruchom aplikacje ponownie.
- Sprawdz, ze wybrany panel zostal przywrocony.
- Kliknij czysty widok, zamknij i uruchom aplikacje ponownie.
- Sprawdz, ze aplikacja wraca do czystego widoku.

## Widoczne akcje

- Sprawdz `File -> New`, `Open`, `Save`, `Save As` i `Exit`.
- Sprawdz `Edit -> Undo` i `Redo`.
- Sprawdz `View -> Reset View`, `Zoom To Fit` i `Grid`.
- Sprawdz `Tools -> Settings`.
- Sprawdz `Help -> About Laser CAD`.
- Sprawdz eksport `SVG` i `DXF`.
- Sprawdz toolbar: nowy, otworz, zapisz, zaznaczanie, czysty widok, rysowanie prostokata, linii, okregu, usuwanie zaznaczonych, eksport SVG i DXF.

## Oczekiwany wynik

- Nie ma widocznych domyslnie przyciskow ani pozycji menu, ktore nic nie robia.
- Funkcje poza aktualnym zakresem nie udaja edytowalnych ustawien.
- Czysty widok daje najwiecej miejsca viewportowi, a dodatkowe panele sa dostepne jawnie z menu.
