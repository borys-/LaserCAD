# LaserCad.Domain

Ten katalog jest miejscem na DLL bibliotek domenowych uzywanych przez Unity.

Wymagane biblioteki po zbudowaniu solution:

- `LaserCad.Core.dll`
- `LaserCad.Geometry.dll`

Biblioteki powinny pochodzic z tego samego commita co projekt Unity. Unity jest adapterem UI i nie powinno kopiowac logiki z tych bibliotek.

Projekty `LaserCad.Core` i `LaserCad.Geometry` buduja dodatkowy target `netstandard2.1` dla Unity. Po buildzie `LaserCad.Core` kopiuje wymagane DLL do tego katalogu automatycznie.

Komenda budujaca domenowe DLL dla Unity:

```powershell
dotnet build ..\src\LaserCad.Core\LaserCad.Core.csproj -f netstandard2.1
```
