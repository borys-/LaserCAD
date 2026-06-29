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

    [SerializeField]
    private float zoomSensitivity = 12f;

    private void Awake()
    {
        workspaceCamera ??= GetComponent<Camera>();
        ConfigureOrthographicCamera();
    }

    private void Update()
    {
        HandleZoom();
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

    private void HandleZoom()
    {
        if (workspaceCamera is null)
        {
            return;
        }

        var scrollDelta = Input.mouseScrollDelta.y;
        if (Mathf.Approximately(scrollDelta, 0f))
        {
            return;
        }

        workspaceCamera.orthographicSize -= scrollDelta * zoomSensitivity;
    }
}
