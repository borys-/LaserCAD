# Podglad 3D - checklista QA

## Zakres MVP

Podglad 3D jest uproszczona wizualizacja pomocnicza. Viewport nadal pozostaje przede wszystkim roboczym widokiem 2D, a podglad 3D pokazuje ekstrudowane prostokaty i zamkniete polilinie z aktualnego dokumentu.

## Manualna weryfikacja

1. Uruchomic `LaserCad.Desktop.exe`.
2. Wygenerowac domyslne pudelko albo otworzyc projekt z prostokatami/zamknietymi poliliniami.
3. W centralnym viewportcie sprawdzic, czy obok geometrii 2D pojawia sie izometryczny podglad 3D w kolorze sklejki.
4. Zmienic grubosc materialu w panelu parametrow i wygenerowac model ponownie.
5. Sprawdzic, czy podglad 3D przebudowuje sie razem z dokumentem.
6. W trybie uruchomionej aplikacji sprawdzic powolna animacje przejscia miedzy rozlozeniem i zlozeniem.
7. Sprawdzic, czy elementy z nachodzacymi bounding boxami sa oznaczane kolorem kolizji.

## Ograniczenia

- MVP tworzy meshe tylko dla prostokatow i zamknietych polilinii.
- Polygon jest triangulowany wachlarzem, wiec najlepiej dziala dla prostych konturow wypuklych.
- Podglad zlozonego pudelka jest heurystyczny i nie zna jeszcze semantyki konkretnych scianek generatora.
- Detekcja kolizji uzywa bounding boxow rendererow Unity, a nie dokladnych testow bryl.
