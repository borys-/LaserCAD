# Minimalny workflow MVP

Ten dokument opisuje najkrotsza sciezke uzycia Laser CAD w obecnym MVP: wybor materialu, ustawienie parametrow pudelka, wygenerowanie geometrii i eksport SVG do ciecia.

## 1. Uruchomienie aplikacji

1. Uruchomic `bin\release\LaserCad.Desktop\LaserCad.Desktop.exe`.
2. Poczekac, az centralny viewport Unity zostanie zaladowany w obszarze roboczym.
3. Jesli viewport nie pojawi sie automatycznie, sprawdzic czy istnieje `bin\release\LaserCad.Desktop\Viewport\LaserCad.exe`.

## 2. Material

1. W lewym panelu `Material i warstwy` wybrac profil materialu, np. `Sklejka 3 mm`.
2. Sprawdzic grubosc, kerf, clearance i minimalna szerokosc palca pokazane dla profilu.
3. Jesli potrzeba skorygowac kerf w biezacej sesji, uzyc panelu kalibracji kerfu dostepnego przez `View -> Advanced Panels`.

## 3. Parametry pudelka

1. Otworzyc panel generatora przez `View -> Box Generator Panel`.
2. Ustawic podstawowe wymiary:
   - szerokosc,
   - glebokosc,
   - wysokosc,
   - szerokosc palca.
3. Kliknac `Przebuduj`.
4. Viewport powinien pokazac rozlozone scianki pudelka jako geometrie 2D na warstwie `Cut`.

## 4. Weryfikacja podgladu

1. Zmienic jeden z wymiarow pudelka, np. szerokosc.
2. Kliknac `Przebuduj`.
3. Sprawdzic, czy geometria 2D w viewportcie zmienila rozmiar i uklad zgodnie z nowym parametrem.
4. Uzyc `Home` albo `Ctrl+0`, jesli trzeba dopasowac widok po przebudowie.

## 5. Eksport SVG

1. Wybrac `Export -> Export SVG...` albo skrot `Ctrl+Shift+S`.
2. Zapisac plik `*.svg`.
3. Otworzyc SVG w zewnetrznym programie i sprawdzic, czy dokument ma jednostki `mm` oraz skale zgodna z ustawionymi wymiarami pudelka.
4. Warstwa `Cut` powinna byc eksportowana jako czerwona geometria ciecia. Warstwa `Engrave` pojawi sie w SVG, jesli dokument zawiera encje grawerowania, np. tekst.

## 6. Opcjonalnie zapis projektu

1. Wybrac `File -> Save As...`.
2. Zapisac projekt jako `*.lasercad.json`.
3. Przy kolejnym uruchomieniu uzyc `File -> Open...`, zeby odtworzyc dokument i kontynuowac prace.

## Znane ograniczenia MVP

- Generator pudelka tworzy wynikowy szkic 2D; instancja generatora nie jest jeszcze zapisywana jako w pelni edytowalny wezel dokumentu.
- Pelna kontrola skali SVG wymaga recznego otwarcia pliku w zewnetrznym programie.
- Po minimalizacji i maksymalizacji okna znany problem `3.6.27` moze nadal wymagac dodatkowej interakcji z viewportem, zeby scroll/zoom znow dzialal.
