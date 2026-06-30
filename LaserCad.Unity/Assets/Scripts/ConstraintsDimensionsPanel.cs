using System.Linq;
using LaserCad.Core.Documents;
using UnityEngine;

namespace LaserCad.Unity
{
    /// <summary>
    /// Panel informacji o ograniczeniach i wymiarach MVP.
    /// </summary>
    public sealed class ConstraintsDimensionsPanel : MonoBehaviour
    {
        [SerializeField]
        private LaserCadApplicationController applicationController;

        [SerializeField]
        private Rect panelRect = new Rect(616f, 404f, 300f, 150f);

        private void OnGUI()
        {
            if (applicationController == null)
            {
                return;
            }

            panelRect = GUI.Window(GetInstanceID(), panelRect, DrawWindow, "Constraints / Dimensions");
        }

        private void DrawWindow(int windowId)
        {
            var document = applicationController.CurrentDocument;
            int sketchesCount = document != null ? document.Sketches.Count : 0;
            int bindingsCount = CountDimensionBindings(document);

            GUILayout.Label("Szkice: " + sketchesCount);
            GUILayout.Label("Powiazania wymiarow: " + bindingsCount);
            GUILayout.Label("Dimensions: Width, Height, Diameter");
            GUILayout.Label("Constraints: H, V, Parallel, Perp., Coincident, Equal");

            GUI.DragWindow(new Rect(0f, 0f, panelRect.width, 22f));
        }

        private static int CountDimensionBindings(CadDocument document)
        {
            if (document == null)
            {
                return 0;
            }

            return document.Sketches
                .SelectMany(sketch => sketch.Entities)
                .OfType<Entity>()
                .Sum(entity => entity.DimensionBindings.Count);
        }
    }
}
