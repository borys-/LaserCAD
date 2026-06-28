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
