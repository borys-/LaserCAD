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
- [docs/DESKTOP_VIEWPORT_LIFECYCLE.md](docs/DESKTOP_VIEWPORT_LIFECYCLE.md) - lifecycle desktop shell i procesu Unity viewport.
- [docs/DESKTOP_VIEWPORT_QA.md](docs/DESKTOP_VIEWPORT_QA.md) - manualna checklista QA dla desktop shell + Unity viewport.
- [docs/DESKTOP_CLEAN_UI_QA.md](docs/DESKTOP_CLEAN_UI_QA.md) - manualna checklista QA dla czystego widoku desktop shell.
- [docs/MATERIAL_SOLID_3D_QA.md](docs/MATERIAL_SOLID_3D_QA.md) - manualna checklista QA dla plyt materialowych 3D.
- [docs/MATERIAL_UNFOLDING_QA.md](docs/MATERIAL_UNFOLDING_QA.md) - manualna checklista QA dla rozwiniecia 3D do czesci 2D.
- [docs/MATERIAL_NESTING_QA.md](docs/MATERIAL_NESTING_QA.md) - manualna checklista QA dla arkusza materialu i nestingu.
- [docs/NESTED_DXF_EXPORT_QA.md](docs/NESTED_DXF_EXPORT_QA.md) - manualna checklista QA eksportu DXF z ulozonych arkuszy.
- [docs/SLOPED_LASER_WORKFLOW_QA.md](docs/SLOPED_LASER_WORKFLOW_QA.md) - manualna checklista QA szybkiego workflow do lasera.
- [docs/RELEASE_QA_CHECKLIST.md](docs/RELEASE_QA_CHECKLIST.md) - checklista QA przed wydaniem.
- [docs/MVP_WORKFLOW.md](docs/MVP_WORKFLOW.md) - minimalny workflow MVP: material, parametry, generowanie i eksport SVG.
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
