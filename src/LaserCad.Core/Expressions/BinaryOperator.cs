namespace LaserCad.Core.Expressions;

/// <summary>
/// Operator arytmetyczny uzywany przez BinaryExpression.
/// Wybierz wartosc zgodna z operacja, ktora ma zostac wykonana podczas ewaluacji.
/// </summary>
public enum BinaryOperator
{
    /// <summary>
    /// Dodawanie lewej i prawej strony.
    /// </summary>
    Add,

    /// <summary>
    /// Odejmowanie prawej strony od lewej.
    /// </summary>
    Subtract,

    /// <summary>
    /// Mnozenie lewej i prawej strony.
    /// </summary>
    Multiply,

    /// <summary>
    /// Dzielenie lewej strony przez prawa.
    /// </summary>
    Divide
}
