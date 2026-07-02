# Manualna checklista QA arkusza materialu i nestingu

Ta checklista dotyczy sekcji `16.4 Arkusz materialu i nesting czesci`.

## Przygotowanie

- Uruchomic `C:\borys\CAD\bin\release\LaserCad.Desktop\LaserCad.Desktop.exe`.
- W panelu `Material i warstwy` wybrac material, np. `Sklejka 3 mm`.
- W toolbarze wybrac `Rysuj plyte materialowa 3D`.
- Narysowac kilka plyt materialowych w viewportcie.

## Ustawienia arkusza

- W panelu `Properties` znalezc pola `Arkusz mm`.
- Ustawic `300` x `300`.
- Ustawic margines, np. `5`.
- Ustawic odstep, np. `5`.
- Opcjonalnie wlaczyc `Obrot 90°`.

## Przygotowanie arkusza

- Kliknac `Przygotuj arkusz`.
- Viewport powinien pokazac podglad 2D: ramke arkusza na warstwie `Ignore` oraz prostokatne czesci na warstwie `Cut`.
- Pole `Nesting` w panelu `Properties` powinno pokazac liczbe czesci, arkuszy, przyblizone zuzycie materialu i dlugosc ciecia.
- Dla zbyt malego arkusza pasek statusu powinien pokazac czytelny blad, ze czesc nie miesci sie na pustym arkuszu.

## Ograniczenia aktualnego UI

- Podglad pokazuje prostokatne bounding boxy czesci, a nie jeszcze rzeczywiste polygony z otworami.
- Statystyki w UI sa szacowane z prostokatnych bounding boxow czesci.
- Eksport DXF nadal dziala z aktualnego dokumentu, a nie jeszcze z wyniku nestingu.
