# LaserCad.Domain

Ten katalog jest miejscem na DLL bibliotek domenowych uzywanych przez Unity.

Wymagane biblioteki po zbudowaniu solution:

- `LaserCad.Core.dll`
- `LaserCad.Geometry.dll`

Biblioteki powinny pochodzic z tego samego commita co projekt Unity. Unity jest adapterem UI i nie powinno kopiowac logiki z tych bibliotek.

Przykladowa komenda budujaca domenowe DLL:

```powershell
dotnet build ..\LaserCad.sln
```

Po buildzie skopiuj DLL z katalogow `src/*/bin` do tego folderu, jezeli Unity nie widzi jeszcze referencji.

