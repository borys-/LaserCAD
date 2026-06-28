# Laser CAD

Laser CAD to projekt parametrycznego programu CAD do projektowania elementow pod wycinarke laserowa, szczegolnie dla materialow takich jak sklejka, MDF i akryl.

Glownym celem aplikacji jest latwe tworzenie projektow produkcyjnych: pudelek, organizerow, przegrodek, ramek i innych elementow z finger jointami, kompensacja kerfu oraz eksportem do formatow uzywanych przy cieciu laserowym.

Kluczowym zalozeniem projektu jest pelna parametrycznosc. Uzytkownik powinien moc zmienic wymiary, grubosc materialu, kerf albo szerokosc palca, a model powinien automatycznie przebudowac geometrie wynikowa.

## Nazwa solution

Solution projektu nosi nazwe `LaserCad`.

## Workflow pracy

Po kazdym tasku zaakceptowanym przez uzytkownika nalezy wykonac osobny commit i push do repozytorium GitHub.

Jeden task z `TASKS.md` powinien odpowiadac jednemu commitowi, chyba ze uzytkownik wyraznie zdecyduje inaczej.

Wiadomosc commita powinna zaczynac sie od numeru taska i byc po polsku, np. `0.0.8 Dodaj instrukcje uruchamiania testow`.

Repozytorium zdalne:

- `origin`: `https://github.com/borys-/LaserCAD.git`

## Dokumenty projektu

- [docs/ROADMAP.md](docs/ROADMAP.md) - ogolna roadmapa produktu.
- [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md) - architektura i podzial odpowiedzialnosci projektow.
- [docs/CODING_STANDARDS.md](docs/CODING_STANDARDS.md) - standardy kodowania i pracy nad projektem.
- [TASKS.md](TASKS.md) - szczegolowa lista taskow implementacyjnych.

## Testy

Testy sa uruchamiane komenda:

```bash
dotnet test LaserCad.sln
```

Aktualnie solution zawiera projekt `LaserCad.Tests` oparty o NUnit i Moq.

## Build

Aktualna strukture solution mozna sprawdzic komenda:

```bash
dotnet build LaserCad.sln
```

Na etapie pustej solution komenda konczy sie powodzeniem, ale moze pokazac ostrzezenie o braku projektow do przywrocenia.
