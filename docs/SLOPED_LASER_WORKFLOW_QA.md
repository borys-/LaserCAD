# QA szybkiego workflow do lasera

## Cel

Ta checklista potwierdza przeplyw: kreator bryly trapezowej -> otwory -> przygotowanie do ciecia -> eksport DXF.

## Kroki manualne

1. Uruchom `bin\release\LaserCad.Desktop\LaserCad.Desktop.exe`.
2. W panelu `Bryla trapezowa` wybierz typ `Bryla z pochyla gora`.
3. Ustaw przykladowo: szerokosc `120`, glebokosc `80`, wysokosc przod `50`, wysokosc tyl `80`, kerf `0,15`.
4. Wybierz material w panelu `Material i warstwy`, np. `Sklejka 3 mm`.
5. Kliknij `Dodaj bryle`.
6. Sprawdz, czy viewport pokazuje obrys bryly oraz rozwiniecie 2D szesciu czesci.
7. Wybierz sciane `Front`, ustaw srednice otworu `10`, od lewej `20`, od dolu `20`.
8. Kliknij `Dodaj otwor`, a potem `Kopiuj otwor na przeciwlegla sciane`.
9. Sprawdz, czy rozwiniecie 2D pokazuje otwory jako kontury wewnetrzne na dwoch scianach.
10. Zaznacz albo odznacz `Etykiety grawerowane` i ponownie dodaj bryle albo otwor, zeby potwierdzic teksty na rozwinieciu.
11. W `Properties` ustaw arkusz `300 x 300`, margines `5`, odstep `5` i opcjonalnie `Obrot 90 stopni`.
12. Kliknij `Przygotuj do ciecia`.
13. Sprawdz podsumowanie `Nesting` oraz `Kontrole`.
14. Kliknij `Eksportuj DXF dla lasera` i wybierz pusty katalog.
15. Sprawdz, czy powstaly pliki `*-sheet-001.dxf` oraz `*-all-sheets.dxf`.

## Oczekiwany wynik

- Bryla trapezowa jest tworzona z pol kreatora.
- Rozwiniecie 2D pokazuje czesci: front, tyl, bok lewy, bok prawy, dno i pochyla gore.
- Okragle otwory przechodza do rozwiniecia i eksportu DXF jako kontury wewnetrzne.
- `Przygotuj do ciecia` wykonuje rozwiniecie, kontrole produkcyjne, nesting i pokazuje podglad arkusza.
- `Eksportuj DXF dla lasera` zapisuje pliki DXF z domyslnymi ustawieniami arkusza.

## Ograniczenia MVP

- Podglad 3D bryly trapezowej jest uproszczonym obrysem w viewportcie.
- Etykiety grawerowane sa widoczne w podgladzie rozwiniecia 2D; eksport DXF arkuszy nadal eksportuje przede wszystkim kontury ciecia.
- Kopiowanie otworu zachowuje lokalna pozycje na scianie przeciwleglej i moze zostac odrzucone, jesli otwor nie miesci sie na tej scianie.
