namespace LaserCad.Core.FingerJoints;

/// <summary>
/// Tryb dopasowania polaczenia palcowego uzywany przy kompensacji luzu montazowego.
/// </summary>
public enum FingerJointFitMode
{
    /// <summary>
    /// Dopasowanie ciasne, preferowane dla polaczen wciskanych.
    /// </summary>
    Tight,

    /// <summary>
    /// Dopasowanie neutralne bez dodatkowej intencji zacisku lub luzu.
    /// </summary>
    Neutral,

    /// <summary>
    /// Dopasowanie luzne, preferowane tam, gdzie czesci maja skladac sie latwiej.
    /// </summary>
    Loose
}
