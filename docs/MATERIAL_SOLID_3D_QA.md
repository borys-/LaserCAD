# Manualna checklista QA plyt materialowych 3D

Ta checklista dotyczy sekcji `16.1 Edycja i laczenie elementow 3D`.

## Przygotowanie

- Uruchomic `C:\borys\CAD\bin\release\LaserCad.Desktop\LaserCad.Desktop.exe`.
- Upewnic sie, ze w panelu `Material i warstwy` wybrany jest material, np. `Sklejka 3 mm`.
- W razie potrzeby uzyc `View -> Clean Workspace`, zeby viewport byl dobrze widoczny.

## Rysowanie plyty

- W toolbarze kliknac narzedzie `Rysuj plyte materialowa 3D`.
- Kliknac i przeciagac w viewportcie tak jak przy prostokacie.
- Po puszczeniu myszy licznik `Plyty 3D` w panelu `Properties` powinien wzrosnac.
- W viewportcie powinien pojawic sie niebieski obrys plyty materialowej.
- Zapis projektu powinien zawierac nowy wpis w `materialSolids`.

## Kilka plyt

- Narysowac co najmniej dwie plyty materialowe.
- Sprawdzic, ze licznik `Plyty 3D` odpowiada liczbie narysowanych plyt.
- Zapisac projekt i wczytac go ponownie.
- Po wczytaniu licznik oraz obrysy plyt powinny zostac odtworzone.

## Snap i laczenie domenowe

- Snap do narozy i krawedzi jest zaimplementowany w domenie przez `MaterialSolidSnapService`.
- Przyciaganie do powierzchni i laczenie pod katem 90 stopni jest zaimplementowane w domenie przez `MaterialSolidAssemblyService`.
- Podglad relacji montazowej jest zaimplementowany przez `MaterialSolidConnectionPreview`.
- Kolizje plyt sa wykrywane domenowo przez `MaterialSolidCollisionDetector`.

## Ograniczenia aktualnego UI

- UI pozwala narysowac plyte materialowa 3D i pokazuje jej obrys.
- Snap, przyciaganie do powierzchni, laczenie 90 stopni, podglad relacji i kolizje sa gotowe domenowo, ale nie maja jeszcze pelnych kontrolek interaktywnych w desktop shell.
- Edycja przesun/obroc/usun plyt 3D istnieje w modelu dokumentu, ale aktualne przyciski transformacji nadal dzialaja na encjach szkicu 2D.
