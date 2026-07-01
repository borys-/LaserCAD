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
- Szkice sa zapisywane w polu `sketches` jako lista obiektow z `id`, `name` i `entities`.
- Encje szkicu maja wspolne pola `type`, `id`, `layerName` oraz opcjonalne `dimensionBindings`.
- Geometria encji jest zapisywana jawnie: punkty jako `{ "x": 0, "y": 0 }`, okregi i luki przez srodek, promien i katy w radianach.
- Encja `Text` zapisuje `text`, `position`, `height`, opcjonalne `fontFamily`, opcjonalne `fontFilePath` oraz opcjonalne `alignment`.
- Powiazania wymiarow zapisuja `dimension` oraz `parameterId`, np. szerokosc prostokata -> `Width`.
- Elementy materialowe 3D sa zapisywane w polu `materialSolids` jako lista obiektow z `id`, `name`, `materialProfile`, `mesh` i `orientation`.
- Mesh elementu materialowego zapisuje wierzcholki 3D `{ "x": 0, "y": 0, "z": 0 }` oraz `triangleIndices`.
- Orientacja elementu materialowego zapisuje `position`, `rotationRadians` oraz `surfaceNormal`.
- Generatory pozostaja poza zakresem pierwszego formatu zapisu, dopoki ich kontrakt plikowy nie bedzie stabilny.

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
  "sketches": [
    {
      "id": "76fe8508-2290-4f05-b2d0-e28dd42ef1c5",
      "name": "Front panel",
      "entities": [
        {
          "type": "Rectangle",
          "id": "4184a82d-e894-4de9-b36b-25a68d94384e",
          "layerName": "Cut",
          "corners": [
            { "x": 0, "y": 0 },
            { "x": 120, "y": 0 },
            { "x": 120, "y": 80 },
            { "x": 0, "y": 80 }
          ],
          "dimensionBindings": [
            {
              "dimension": "Width",
              "parameterId": "Width"
            }
          ]
        }
      ]
    }
  ],
  "materialProfile": null
}
```
