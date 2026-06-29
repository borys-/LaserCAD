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

    private Vector3 lastPanMousePosition;

    private void Awake()
    {
        workspaceCamera ??= GetComponent<Camera>();
        ConfigureOrthographicCamera();
    }

    private void Update()
    {
        HandleZoom();
        HandlePan();
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

    private void HandlePan()
    {
        if (workspaceCamera is null)
        {
            return;
        }

        if (Input.GetMouseButtonDown(2) || Input.GetMouseButtonDown(1))
        {
            lastPanMousePosition = Input.mousePosition;
        }

        if (!Input.GetMouseButton(2) && !Input.GetMouseButton(1))
        {
            return;
        }

        var previousWorldPoint = workspaceCamera.ScreenToWorldPoint(lastPanMousePosition);
        var currentWorldPoint = workspaceCamera.ScreenToWorldPoint(Input.mousePosition);
        var worldDelta = previousWorldPoint - currentWorldPoint;

        transform.position += new Vector3(worldDelta.x, worldDelta.y, 0f);
        lastPanMousePosition = Input.mousePosition;
    }
}
