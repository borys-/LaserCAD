# Checklist QA Unity

## Kamera 2D

- Uruchomic scene `Assets/Scenes/Workspace.unity`.
- Potwierdzic, ze kamera jest ortograficzna i pokazuje obszar roboczy 2D.
- Obracac kolkiem myszy i potwierdzic plynny zoom.
- Sprawdzic, ze zoom zatrzymuje sie na minimalnym i maksymalnym limicie.
- Przeciagnac widok srodkowym przyciskiem myszy.
- Przeciagnac widok prawym przyciskiem myszy.
- Nacisnac `Home` i potwierdzic powrot do poczatkowego widoku.

## Siatka 2D

- Uruchomic scene `Assets/Scenes/Workspace.unity`.
- Potwierdzic, ze siatka jest widoczna w obszarze roboczym.
- Przyblizyc widok i potwierdzic, ze widac podzialke 1 mm.
- Potwierdzic, ze linie co 5 mm i 10 mm sa mocniejsze od podstawowej siatki.
- Zmienic zoom i potwierdzic, ze grubosc linii pozostaje czytelna.
- Nacisnac `G` i potwierdzic wylaczenie oraz ponowne wlaczenie siatki.

## Snap

- Uruchomic scene `Assets/Scenes/Workspace.unity`.
- Poruszyc kursorem po obszarze roboczym i potwierdzic, ze marker snapu trafia w punkty siatki 1 mm.
- Na dokumencie z encjami potwierdzic snap do naroznikow prostokata i punktow polilinii.
- Potwierdzic snap do srodka okregu oraz srodka prostokata.
- Potwierdzic snap do koncow linii.
- Przy nalozonych kandydatach potwierdzic priorytet: koniec linii, srodek encji, punkt encji, siatka.

## Zaznaczanie

- Uruchomic scene `Assets/Scenes/Workspace.unity`.
- Potwierdzic, ze panel `Selection` jest widoczny w lewym gornym obszarze widoku.
- Na dokumencie z encjami kliknac encje i potwierdzic, ze licznik `Selected` zmienia sie na `1`.
- Kliknac puste miejsce i potwierdzic, ze zaznaczenie zostaje wyczyszczone.
- Przytrzymac `Ctrl` albo `Shift` i kliknac kilka encji, zeby potwierdzic multi-select.
- Przeciagnac lewym przyciskiem myszy prostokat przez encje i potwierdzic, ze encje w obszarze sa zaznaczone.
- Potwierdzic, ze zaznaczone encje maja niebieski obrys bounding boxa.
- Potwierdzic, ze panel pokazuje typ, warstwe i bounding box pierwszej zaznaczonej encji.

## GUI funkcji domenowych

- Uruchomic aplikacje albo scene `Assets/Scenes/Workspace.unity`.
- Potwierdzic, ze po lewej stronie widoczny jest panel `Generator pudelka`.
- Zmienic `Szerokosc` i kliknac `Zastosuj`; geometria dokumentu powinna zostac przebudowana.
- Wybrac typ `Otwarte` i potwierdzic, ze widac 5 paneli pudelka.
- Wybrac typ `Z pokrywa` i potwierdzic, ze widac 6 paneli pudelka.
- W panelu `Eksport SVG` kliknac `Eksportuj SVG` i potwierdzic komunikat z pelna sciezka zapisu.
- W panelu `Eksport DXF` kliknac `Eksportuj DXF` i potwierdzic komunikat z pelna sciezka zapisu.
- Otworzyc zapisane pliki z katalogu `Application.persistentDataPath` i potwierdzic, ze zawieraja aktualny dokument.
- W panelu `Material i warstwy` wybrac inny profil materialu i kliknac `Zastosuj material`.
- Potwierdzic, ze panel `Material i warstwy` pokazuje grubosc, kerf, clearance, minimalna szerokosc palca oraz liste warstw.
- Potwierdzic, ze panel `Historia` pokazuje liczbe pozycji undo i redo.
- Potwierdzic, ze panel `Constraints / Dimensions` pokazuje liczbe szkicow oraz liczbe powiazan wymiarow.
