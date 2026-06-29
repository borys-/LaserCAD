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
