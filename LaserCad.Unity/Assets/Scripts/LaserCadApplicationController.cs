using System;
using LaserCad.Core.BoxGenerators;
using LaserCad.Core.Commands;
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
        private readonly BoxGenerator boxGenerator = new BoxGenerator();

        [SerializeField]
        private DocumentInfoView documentInfoView;

        [SerializeField]
        private bool loadDemoDocument = true;

        /// <summary>
        /// Aktualnie zaladowany dokument CAD.
        /// </summary>
        public CadDocument CurrentDocument { get; private set; }

        /// <summary>
        /// Historia komend edycyjnych dla aktualnego dokumentu.
        /// </summary>
        public UndoRedoStack CurrentHistory { get; private set; }

        /// <summary>
        /// Aktualne opcje generatora pudelka prezentowane w UI.
        /// </summary>
        public BoxGeneratorOptions CurrentBoxOptions { get; private set; }

        private void Awake()
        {
            Initialize();
        }

        /// <summary>
        /// Inicjalizuje stan aplikacji.
        /// </summary>
        public void Initialize()
        {
            CurrentBoxOptions = new BoxGeneratorOptions();
            var isViewportProcess = ViewportProcessMode.IsViewportProcess();
            LoadDocument(loadDemoDocument && !isViewportProcess ? CreateDemoDocument() : new CadDocument(name: "Viewport"));
        }

        /// <summary>
        /// Aktualizuje opcje generatora pudelka po walidacji domenowej.
        /// </summary>
        public void SetBoxOptions(BoxGeneratorOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            CurrentBoxOptions = options;
            LoadDocument(CreateBoxPreviewDocument(options));
        }

        /// <summary>
        /// Aktualizuje profil materialu aktualnego dokumentu.
        /// </summary>
        public void SetMaterialProfile(MaterialProfile materialProfile)
        {
            if (materialProfile == null)
            {
                throw new ArgumentNullException(nameof(materialProfile));
            }

            LoadDocument((CurrentDocument ?? new CadDocument()).WithMaterialProfile(materialProfile));
        }

        /// <summary>
        /// Cofa ostatnia komende edycyjna, jesli jest dostepna.
        /// </summary>
        public void Undo()
        {
            if (CurrentHistory == null || !CurrentHistory.CanUndo)
            {
                return;
            }

            CurrentDocument = CurrentHistory.Undo();
            if (documentInfoView != null)
            {
                documentInfoView.Show(CurrentDocument);
            }
        }

        /// <summary>
        /// Ponawia ostatnio cofnieta komende edycyjna, jesli jest dostepna.
        /// </summary>
        public void Redo()
        {
            if (CurrentHistory == null || !CurrentHistory.CanRedo)
            {
                return;
            }

            CurrentDocument = CurrentHistory.Redo();
            if (documentInfoView != null)
            {
                documentInfoView.Show(CurrentDocument);
            }
        }

        /// <summary>
        /// Laduje nowy dokument CAD do viewportu.
        /// </summary>
        public void LoadDocument(CadDocument document)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            CurrentDocument = document;
            CurrentHistory = new UndoRedoStack(document);
            if (documentInfoView != null)
            {
                documentInfoView.Show(CurrentDocument);
            }
        }

        private CadDocument CreateBoxPreviewDocument(BoxGeneratorOptions options)
        {
            var materialProfile = CurrentDocument != null && CurrentDocument.MaterialProfile != null
                ? CurrentDocument.MaterialProfile
                : DefaultMaterialProfiles.Plywood3Mm;

            return new CadDocument(name: "Podglad pudelka")
                .WithMaterialProfile(materialProfile)
                .AddSketch(boxGenerator.GenerateSketch(options));
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
