# Format pliku projektu Laser CAD

Formatem pliku projektu w MVP jest JSON oparty o `System.Text.Json`.
Plik zapisuje stabilny kontrakt DTO, a nie bezposrednia serializacje klas domenowych.

## Zasady formatu

- Wersja formatu jest zapisana w polu `formatVersion`.
- Biezaca obslugiwana wersja formatu to `1`.
- Wymiary typu `Length` sa zapisywane jako liczby w milimetrach.
- Kolory warstw sa zapisywane jako tekst `#RRGGBB`.
- Parametry zapisuja `id`, `name`, `type`, `value`, opcjonalne `displayUnit`, `minimumValue` i `maximumValue`.
- Profil materialu zapisuje nazwe oraz wszystkie wymiary produkcyjne w milimetrach.
- Szkice i generatory pozostaja poza zakresem pierwszego formatu zapisu, dopoki ich encje nie maja stabilnego kontraktu plikowego.

## Przykladowy dokument

```json
{
  "formatVersion": 1,
  "id": "9f0f0c35-6824-4a23-9f30-c9a21b30c5e1",
  "name": "Untitled",
  "parameters": [],
  "layers": [
    {
      "name": "Cut",
      "color": "#FF0000",
      "role": "Cut"
    }
  ],
  "materialProfile": null
}
```
