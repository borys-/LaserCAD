# Laser CAD

Laser CAD to projekt parametrycznego programu CAD do projektowania elementow pod wycinarke laserowa, szczegolnie dla materialow takich jak sklejka, MDF i akryl.

Glownym celem aplikacji jest latwe tworzenie projektow produkcyjnych: pudelek, organizerow, przegrodek, ramek i innych elementow z finger jointami, kompensacja kerfu oraz eksportem do formatow uzywanych przy cieciu laserowym.

Kluczowym zalozeniem projektu jest pelna parametrycznosc. Uzytkownik powinien moc zmienic wymiary, grubosc materialu, kerf albo szerokosc palca, a model powinien automatycznie przebudowac geometrie wynikowa.

## Nazwa solution

Solution projektu nosi nazwe `LaserCad`.

## Workflow pracy

Po kazdym tasku zaakceptowanym przez uzytkownika nalezy wykonac osobny commit i push do repozytorium GitHub.

Jeden task z `TASKS.md` powinien odpowiadac jednemu commitowi, chyba ze uzytkownik wyraznie zdecyduje inaczej.

Wiadomosc commita powinna zaczynac sie od numeru taska, np. `0.0.8 Add test instructions`.

Repozytorium zdalne:

- `origin`: `https://github.com/borys-/LaserCAD.git`

## Dokumenty projektu

- [docs/ROADMAP.md](docs/ROADMAP.md) - ogolna roadmapa produktu.
- [TASKS.md](TASKS.md) - szczegolowa lista taskow implementacyjnych.

## Testy

Docelowo testy beda uruchamiane komenda:

```bash
dotnet test LaserCad.sln
```

Komenda zacznie wykonywac testy po dodaniu projektow testowych do solution.
