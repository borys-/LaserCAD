# Standardy kodowania

## Namespace

Namespace powinien zaczynac sie od nazwy solution `LaserCad`.

Konwencja:

- projekt `LaserCad.Core` uzywa namespace `LaserCad.Core`,
- projekt `LaserCad.Geometry` uzywa namespace `LaserCad.Geometry`,
- projekt `LaserCad.Export.Svg` uzywa namespace `LaserCad.Export.Svg`,
- projekt `LaserCad.Export.Dxf` uzywa namespace `LaserCad.Export.Dxf`,
- projekt `LaserCad.Tests` uzywa namespace `LaserCad.Tests`.

Dla podkatalogow nalezy dopisywac kolejne segmenty namespace zgodnie ze struktura domeny, np. `LaserCad.Geometry.Primitives`.

## Nazewnictwo

Konwencje nazewnictwa:

- klasy, rekordy, struktury, enumy i interfejsy: `PascalCase`,
- metody i properties: `PascalCase`,
- parametry metod i zmienne lokalne: `camelCase`,
- pola prywatne: `_camelCase`,
- stale: `PascalCase`,
- interfejsy: prefiks `I`, np. `IExporter`.

Nazwy powinny opisywac intencje domenowa, np. `MaterialThickness`, `Kerf`, `FingerWidth`, zamiast skrotow technicznych.
