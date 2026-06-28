namespace LaserCad.Core.Parameters;

/// <summary>
/// Okresla rodzaj wartosci przechowywanej przez parametr.
/// Uzywaj tego enumu przy tworzeniu Parameter, aby walidacja wiedziala, jakiego typu wartosci oczekiwac.
/// </summary>
public enum ParameterType
{
    /// <summary>
    /// Parametr dlugosci przechowywany jako Length i ewaluowany w milimetrach.
    /// </summary>
    Length,

    /// <summary>
    /// Parametr liczbowy przechowywany jako double.
    /// </summary>
    Number,

    /// <summary>
    /// Parametr logiczny przechowywany jako bool.
    /// </summary>
    Boolean,

    /// <summary>
    /// Parametr tekstowy przechowywany jako string.
    /// </summary>
    Text,

    /// <summary>
    /// Parametr wyboru przechowywany jako string reprezentujacy wybrana opcje.
    /// </summary>
    Choice
}
