using LaserCad.Core.Documents;
using UnityEngine;

namespace LaserCad.Unity;

/// <summary>
/// Glowny kontroler aplikacji Unity.
/// </summary>
public sealed class LaserCadApplicationController : MonoBehaviour
{
    /// <summary>
    /// Aktualnie zaladowany dokument CAD.
    /// </summary>
    public CadDocument? CurrentDocument { get; private set; }

    private void Awake()
    {
        Initialize();
    }

    /// <summary>
    /// Inicjalizuje stan aplikacji.
    /// </summary>
    public void Initialize()
    {
        CurrentDocument = null;
    }
}
