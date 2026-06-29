using System;
using LaserCad.Core.Documents;
using LaserCad.Geometry;
using UnityEngine;

namespace LaserCad.Unity
{
    /// <summary>
    /// Glowny kontroler aplikacji Unity.
    /// </summary>
    public sealed class LaserCadApplicationController : MonoBehaviour
    {
        [SerializeField]
        private DocumentInfoView documentInfoView;

        [SerializeField]
        private bool loadDemoDocument = true;

        /// <summary>
        /// Aktualnie zaladowany dokument CAD.
        /// </summary>
        public CadDocument CurrentDocument { get; private set; }

        private void Awake()
        {
            Initialize();
        }

        /// <summary>
        /// Inicjalizuje stan aplikacji.
        /// </summary>
        public void Initialize()
        {
            CurrentDocument = loadDemoDocument ? CreateDemoDocument() : new CadDocument();
            if (documentInfoView != null)
            {
                documentInfoView.Show(CurrentDocument);
            }
        }

        private static CadDocument CreateDemoDocument()
        {
            var sketch = new Sketch(name: "Demo");

            sketch = sketch.AddEntity(new RectangleEntity(new Point2D(-90.0, -45.0), 80.0, 50.0, layerName: "Cut"));
            sketch = sketch.AddEntity(new LineEntity(new LineSegment2D(new Point2D(10.0, -45.0), new Point2D(90.0, 5.0)), layerName: "Cut"));
            sketch = sketch.AddEntity(new CircleEntity(new Circle2D(new Point2D(45.0, 40.0), 18.0), layerName: "Engrave"));
            sketch = sketch.AddEntity(new ArcEntity(new Arc2D(new Point2D(-45.0, 45.0), 24.0, 0.0, Math.PI, ArcDirection.Counterclockwise), layerName: "Score"));
            sketch = sketch.AddEntity(
                new PolylineEntity(
                    new Polyline2D(
                        new[]
                        {
                            new Point2D(-20.0, 20.0),
                            new Point2D(0.0, 34.0),
                            new Point2D(22.0, 20.0),
                            new Point2D(0.0, 8.0),
                        },
                        true),
                    layerName: "Cut"));
            sketch = sketch.AddEntity(new TextEntity("Laser CAD", new Point2D(-90.0, 25.0), 10.0, layerName: "Engrave"));

            return new CadDocument(name: "Demo dokument").WithMaterialProfile(DefaultMaterialProfiles.Plywood3Mm).AddSketch(sketch);
        }
    }
}
