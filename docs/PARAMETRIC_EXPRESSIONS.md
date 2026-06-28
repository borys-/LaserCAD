# Wyrazenia parametryczne

Ten dokument ustala minimalna skladnie wyrazen parametrycznych dla MVP.

## Zakres MVP

Minimalne wyrazenia sa budowane jawnie jako AST w kodzie domenowym. Parser tekstowy moze zostac dodany pozniej bez zmiany modelu wyrazen.

Obslugiwane elementy:

- stale liczbowe,
- referencje do parametrow po `ParameterId`,
- dodawanie: `A + B`,
- odejmowanie: `A - B`,
- mnozenie: `A * B`,
- dzielenie: `A / B`.

Przykladowe wyrazenie:

```text
Width - 2 * MaterialThickness
```

Priorytety operatorow sa zgodne z typowa arytmetyka:

- mnozenie i dzielenie maja wyzszy priorytet niz dodawanie i odejmowanie,
- jawnie budowane AST rozstrzyga kolejnosc dzialan bez parsera.

## Typ wartosci

W MVP wyrazenia zwracaja wartosc liczbowa `double`. Parametry typu `Number` sa uzywane bez konwersji, a parametry typu `Length` sa ewaluowane jako milimetry.

## Bledy

Evaluator powinien zwracac czytelny blad dla:

- brakujacego parametru,
- parametru o nieobslugiwanym typie wartosci,
- dzielenia przez zero.
