using System;
using System.Collections.Generic;
using LaserCad.Core.Documents;
using LaserCad.Geometry;
using UnityEngine;

namespace LaserCad.Unity
{
    /// <summary>
    /// Przechowuje aktualne zaznaczenie encji szkicu w widoku Unity.
    /// </summary>
    public sealed class SelectionService : MonoBehaviour
    {
        [SerializeField]
        private Camera workspaceCamera;

        [SerializeField]
        private LaserCadApplicationController applicationController;

        [SerializeField]
        private float clickToleranceMillimeters = 2f;

        [SerializeField]
        private float dragThresholdPixels = 4f;

        private readonly HashSet<Guid> selectedEntityIds = new HashSet<Guid>();
        private Vector2 mouseDownScreenPosition;
        private Vector2 currentMouseScreenPosition;
        private bool isPointerDown;
        private bool isInputEnabled = true;

        /// <summary>
        /// Identyfikatory aktualnie zaznaczonych encji.
        /// </summary>
        public IReadOnlyCollection<Guid> SelectedEntityIds
        {
            get { return selectedEntityIds; }
        }

        /// <summary>
        /// Liczba zaznaczonych encji.
        /// </summary>
        public int SelectionCount
        {
            get { return selectedEntityIds.Count; }
        }

        /// <summary>
        /// Informuje, czy uzytkownik przeciaga aktualnie prostokat zaznaczania.
        /// </summary>
        public bool IsDraggingSelection
        {
            get
            {
                return isInputEnabled
                    && isPointerDown
                    && Vector2.Distance(mouseDownScreenPosition, currentMouseScreenPosition) >= dragThresholdPixels;
            }
        }

        /// <summary>
        /// Pozycja ekranu, w ktorej zaczelo sie aktualne przeciaganie.
        /// </summary>
        public Vector2 DragStartScreenPosition
        {
            get { return mouseDownScreenPosition; }
        }

        /// <summary>
        /// Aktualna pozycja ekranu podczas przeciagania.
        /// </summary>
        public Vector2 DragCurrentScreenPosition
        {
            get { return currentMouseScreenPosition; }
        }

        private void Update()
        {
            if (!isInputEnabled)
            {
                isPointerDown = false;
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                isPointerDown = true;
                mouseDownScreenPosition = Input.mousePosition;
                currentMouseScreenPosition = mouseDownScreenPosition;
            }

            if (isPointerDown)
            {
                currentMouseScreenPosition = Input.mousePosition;
            }

            if (isPointerDown && Input.GetMouseButtonUp(0))
            {
                FinishPointerSelection();
                isPointerDown = false;
            }
        }

        /// <summary>
        /// Czysci zaznaczenie.
        /// </summary>
        public void Clear()
        {
            selectedEntityIds.Clear();
        }

        /// <summary>
        /// Wlacza albo wylacza obsluge inputu zaznaczania, np. gdy aktywne jest narzedzie rysowania.
        /// </summary>
        public void SetInputEnabled(bool enabled)
        {
            isInputEnabled = enabled;
            if (!isInputEnabled)
            {
                isPointerDown = false;
            }
        }

        /// <summary>
        /// Zaznacza tylko jedna encje.
        /// </summary>
        public void SelectOnly(ISketchEntity entity)
        {
            selectedEntityIds.Clear();

            if (entity != null)
            {
                selectedEntityIds.Add(entity.Id);
            }
        }

        /// <summary>
        /// Przelacza zaznaczenie encji bez zmiany pozostalych elementow.
        /// </summary>
        public void Toggle(ISketchEntity entity)
        {
            if (entity == null)
            {
                return;
            }

            if (!selectedEntityIds.Remove(entity.Id))
            {
                selectedEntityIds.Add(entity.Id);
            }
        }

        /// <summary>
        /// Sprawdza, czy encja jest zaznaczona.
        /// </summary>
        public bool IsSelected(ISketchEntity entity)
        {
            return entity != null && selectedEntityIds.Contains(entity.Id);
        }

        private void FinishPointerSelection()
        {
            var currentScreenPosition = (Vector2)Input.mousePosition;
            if (Vector2.Distance(mouseDownScreenPosition, currentScreenPosition) < dragThresholdPixels)
            {
                SelectAtMousePosition();
                return;
            }

            SelectByRectangle(mouseDownScreenPosition, currentScreenPosition);
        }

        private void SelectAtMousePosition()
        {
            var entity = FindEntityAtMousePosition();
            var isMultiSelect = IsMultiSelectModifierPressed();

            if (entity != null)
            {
                if (isMultiSelect)
                {
                    Toggle(entity);
                }
                else
                {
                    SelectOnly(entity);
                }

                return;
            }

            if (!isMultiSelect)
            {
                Clear();
            }
        }

        private void SelectByRectangle(Vector2 startScreenPosition, Vector2 endScreenPosition)
        {
            if (workspaceCamera == null)
            {
                return;
            }

            var startWorld = workspaceCamera.ScreenToWorldPoint(startScreenPosition);
            var endWorld = workspaceCamera.ScreenToWorldPoint(endScreenPosition);
            var minX = Math.Min(startWorld.x, endWorld.x);
            var maxX = Math.Max(startWorld.x, endWorld.x);
            var minY = Math.Min(startWorld.y, endWorld.y);
            var maxY = Math.Max(startWorld.y, endWorld.y);
            var isMultiSelect = IsMultiSelectModifierPressed();

            if (!isMultiSelect)
            {
                Clear();
            }

            foreach (var entity in GetEntities())
            {
                if (IntersectsBounds(entity.Bounds, minX, maxX, minY, maxY))
                {
                    selectedEntityIds.Add(entity.Id);
                }
            }
        }

        private ISketchEntity FindEntityAtMousePosition()
        {
            if (workspaceCamera == null)
            {
                return null;
            }

            var world = workspaceCamera.ScreenToWorldPoint(Input.mousePosition);
            return FindNearestEntity(new Vector2(world.x, world.y));
        }

        private ISketchEntity FindNearestEntity(Vector2 position)
        {
            ISketchEntity bestEntity = null;
            var bestDistance = float.PositiveInfinity;

            foreach (var entity in GetEntities())
            {
                var distance = GetDistanceToEntity(position, entity);
                if (distance <= clickToleranceMillimeters && distance < bestDistance)
                {
                    bestEntity = entity;
                    bestDistance = distance;
                }
            }

            return bestEntity;
        }

        private static float GetDistanceToEntity(Vector2 position, ISketchEntity entity)
        {
            var line = entity as LineEntity;
            if (line != null)
            {
                return GetDistanceToSegment(position, ToVector2(line.Segment.Start), ToVector2(line.Segment.End));
            }

            var rectangle = entity as RectangleEntity;
            if (rectangle != null)
            {
                return GetDistanceToClosedPoints(position, rectangle.Corners);
            }

            var polyline = entity as PolylineEntity;
            if (polyline != null)
            {
                return GetDistanceToPolyline(position, polyline.Polyline);
            }

            var circle = entity as CircleEntity;
            if (circle != null)
            {
                return Mathf.Abs(Vector2.Distance(position, ToVector2(circle.Circle.Center)) - (float)circle.Circle.Radius);
            }

            var arc = entity as ArcEntity;
            if (arc != null)
            {
                return GetDistanceToArc(position, arc.Arc);
            }

            return GetDistanceToBounds(position, entity.Bounds);
        }

        private static float GetDistanceToPolyline(Vector2 position, Polyline2D polyline)
        {
            var bestDistance = float.PositiveInfinity;

            for (var index = 1; index < polyline.Points.Count; index++)
            {
                bestDistance = Mathf.Min(
                    bestDistance,
                    GetDistanceToSegment(position, ToVector2(polyline.Points[index - 1]), ToVector2(polyline.Points[index])));
            }

            if (polyline.IsClosed && polyline.Points.Count > 1)
            {
                bestDistance = Mathf.Min(
                    bestDistance,
                    GetDistanceToSegment(
                        position,
                        ToVector2(polyline.Points[polyline.Points.Count - 1]),
                        ToVector2(polyline.Points[0])));
            }

            return bestDistance;
        }

        private static float GetDistanceToClosedPoints(Vector2 position, IReadOnlyList<Point2D> points)
        {
            var bestDistance = float.PositiveInfinity;

            for (var index = 1; index < points.Count; index++)
            {
                bestDistance = Mathf.Min(
                    bestDistance,
                    GetDistanceToSegment(position, ToVector2(points[index - 1]), ToVector2(points[index])));
            }

            if (points.Count > 1)
            {
                bestDistance = Mathf.Min(
                    bestDistance,
                    GetDistanceToSegment(position, ToVector2(points[points.Count - 1]), ToVector2(points[0])));
            }

            return bestDistance;
        }

        private static float GetDistanceToArc(Vector2 position, Arc2D arc)
        {
            const int SegmentCount = 48;
            var bestDistance = float.PositiveInfinity;
            var previous = ToVector2(arc.PointAt(0.0));

            for (var index = 1; index <= SegmentCount; index++)
            {
                var current = ToVector2(arc.PointAt(index / (double)SegmentCount));
                bestDistance = Mathf.Min(bestDistance, GetDistanceToSegment(position, previous, current));
                previous = current;
            }

            return bestDistance;
        }

        private static float GetDistanceToSegment(Vector2 position, Vector2 start, Vector2 end)
        {
            var segment = end - start;
            var lengthSquared = segment.sqrMagnitude;

            if (lengthSquared <= Mathf.Epsilon)
            {
                return Vector2.Distance(position, start);
            }

            var t = Mathf.Clamp01(Vector2.Dot(position - start, segment) / lengthSquared);
            var nearest = start + (segment * t);
            return Vector2.Distance(position, nearest);
        }

        private static Vector2 ToVector2(Point2D point)
        {
            return new Vector2((float)point.X, (float)point.Y);
        }

        private IEnumerable<ISketchEntity> GetEntities()
        {
            if (applicationController == null || applicationController.CurrentDocument == null)
            {
                yield break;
            }

            foreach (var sketch in applicationController.CurrentDocument.Sketches)
            {
                foreach (var entity in sketch.Entities)
                {
                    yield return entity;
                }
            }
        }

        private static float GetDistanceToBounds(Vector2 position, BoundingBox bounds)
        {
            var dx = Mathf.Max(
                Mathf.Max((float)bounds.MinX - position.x, 0f),
                position.x - (float)bounds.MaxX);
            var dy = Mathf.Max(
                Mathf.Max((float)bounds.MinY - position.y, 0f),
                position.y - (float)bounds.MaxY);

            return Mathf.Sqrt(dx * dx + dy * dy);
        }

        private static bool IntersectsBounds(BoundingBox bounds, float minX, float maxX, float minY, float maxY)
        {
            return bounds.MaxX >= minX
                && bounds.MinX <= maxX
                && bounds.MaxY >= minY
                && bounds.MinY <= maxY;
        }

        private static bool IsMultiSelectModifierPressed()
        {
            return Input.GetKey(KeyCode.LeftControl)
                || Input.GetKey(KeyCode.RightControl)
                || Input.GetKey(KeyCode.LeftShift)
                || Input.GetKey(KeyCode.RightShift);
        }
    }
}
