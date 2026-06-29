using UnityEngine;

namespace LaserCad.Unity;

/// <summary>
/// Kontroluje widok roboczy kamery 2D.
/// </summary>
[RequireComponent(typeof(Camera))]
public sealed class WorkspaceCameraController : MonoBehaviour
{
    [SerializeField]
    private Camera? workspaceCamera;

    [SerializeField]
    private float initialOrthographicSize = 100f;

    private void Awake()
    {
        workspaceCamera ??= GetComponent<Camera>();
        ConfigureOrthographicCamera();
    }

    private void ConfigureOrthographicCamera()
    {
        if (workspaceCamera is null)
        {
            return;
        }

        workspaceCamera.orthographic = true;
        workspaceCamera.orthographicSize = initialOrthographicSize;
    }
}
