# Desktop shell + Unity viewport - QA

Manualna checklista dla sekcji 3.6.

## Desktop shell

- Uruchom `bin\release\LaserCad.Desktop\LaserCad.Desktop.exe`.
- Sprawdz, ze widoczne jest menu `File`, `Edit`, `View`, `Export`, `Help`.
- Sprawdz, ze toolbar ma akcje `New`, `Open`, `Save`, `SVG`, `DXF`, `Start viewport`, `Restart`, `Stop`.
- Zmien wartosci w panelu `Parametry pudelka` i kliknij `Przebuduj`.
- Sprawdz, ze panel `Properties` pokazuje dokument `Projekt pudelka`, liczbe szkicow oraz status przebudowy.
- Zmien profil w panelu `Material i warstwy`.
- Sprawdz, ze lista warstw pokazuje nazwe, role i kolor.

## Eksport

- Kliknij `SVG` i zapisz plik testowy.
- Otworz zapisany plik i sprawdz, ze zawiera elementy SVG oraz jednostki w mm.
- Kliknij `DXF` i zapisz plik testowy.
- Otworz plik tekstowo i sprawdz, ze zawiera sekcje DXF oraz warstwy.

## Viewport

- Kliknij `Start viewport`.
- Jesli build zawiera `Viewport\LaserCad.exe`, powinien uruchomic sie osobny proces Unity.
- Kliknij `View -> Reset View`, `View -> Zoom To Fit` oraz przelacz `View -> Grid`.
- Sprawdz, ze do `%LOCALAPPDATA%\LaserCad\viewport-outbox.jsonl` dopisuja sie komunikaty JSON.
- Kliknij `Restart` i sprawdz, ze proces viewportu zostaje zamkniety i uruchomiony ponownie.
- Kliknij `Stop` i sprawdz, ze proces viewportu zostaje zamkniety.
