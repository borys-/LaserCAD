using UnityEngine;

namespace LaserCad.Unity
{
    /// <summary>
    /// Kontroluje widok roboczy kamery 2D.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public sealed class WorkspaceCameraController : MonoBehaviour
    {
        [SerializeField]
        private Camera workspaceCamera;

        [SerializeField]
        private float initialOrthographicSize = 100f;

        [SerializeField]
        private float zoomSensitivity = 12f;

        [SerializeField]
        private float minimumOrthographicSize = 5f;

        [SerializeField]
        private float maximumOrthographicSize = 500f;

        [SerializeField]
        private KeyCode resetViewKey = KeyCode.Home;

        private Vector3 initialPosition;
        private Vector3 lastPanMousePosition;

        private void Awake()
        {
            if (workspaceCamera == null)
            {
                workspaceCamera = GetComponent<Camera>();
            }

            initialPosition = transform.position;
            ConfigureOrthographicCamera();
        }

        private void Update()
        {
            HandleZoom();
            HandlePan();
            HandleResetView();
        }

        /// <summary>
        /// Przywraca poczatkowe polozenie i skale widoku roboczego.
        /// </summary>
        public void ResetView()
        {
            if (workspaceCamera == null)
            {
                return;
            }

            transform.position = initialPosition;
            workspaceCamera.orthographicSize = ClampOrthographicSize(initialOrthographicSize);
        }

        private void ConfigureOrthographicCamera()
        {
            if (workspaceCamera == null)
            {
                return;
            }

            workspaceCamera.orthographic = true;
            workspaceCamera.orthographicSize = ClampOrthographicSize(initialOrthographicSize);
        }

        private void HandleZoom()
        {
            if (workspaceCamera == null)
            {
                return;
            }

            var scrollDelta = Input.mouseScrollDelta.y;
            if (Mathf.Approximately(scrollDelta, 0f))
            {
                return;
            }

            var requestedSize = workspaceCamera.orthographicSize - scrollDelta * zoomSensitivity;
            workspaceCamera.orthographicSize = ClampOrthographicSize(requestedSize);
        }

        private void HandlePan()
        {
            if (workspaceCamera == null)
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

        private void HandleResetView()
        {
            if (Input.GetKeyDown(resetViewKey))
            {
                ResetView();
            }
        }

        private float ClampOrthographicSize(float requestedSize)
        {
            return Mathf.Clamp(requestedSize, minimumOrthographicSize, maximumOrthographicSize);
        }
    }
}
