namespace LaserCad.Core.FeatureTree;

/// <summary>
/// Okresla rodzaj wpisu w drzewie historii modelu.
/// </summary>
public enum FeatureTreeItemKind
{
    /// <summary>
    /// Wpis reprezentuje instancje generatora parametrycznego.
    /// </summary>
    Generator,

    /// <summary>
    /// Wpis reprezentuje operacje edycyjna wykonana na dokumencie.
    /// </summary>
    EditOperation
}
