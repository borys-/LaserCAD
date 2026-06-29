using System.Collections.Generic;
using LaserCad.Core.Documents;
using LaserCad.Geometry;
using UnityEngine;

namespace LaserCad.Unity
{
    /// <summary>
    /// Wyswietla podstawowe wlasciwosci aktualnego zaznaczenia.
    /// </summary>
    public sealed class SelectionPropertiesPanel : MonoBehaviour
    {
        [SerializeField]
        private LaserCadApplicationController applicationController;

        [SerializeField]
        private SelectionService selectionService;

        [SerializeField]
        private Rect panelRect = new Rect(12f, 72f, 260f, 116f);

        private void OnGUI()
        {
            if (selectionService == null)
            {
                return;
            }

            GUI.Box(panelRect, "Selection");

            GUILayout.BeginArea(new Rect(panelRect.x + 10f, panelRect.y + 24f, panelRect.width - 20f, panelRect.height - 32f));
            GUILayout.Label("Selected: " + selectionService.SelectionCount);

            var firstEntity = GetFirstSelectedEntity();
            if (firstEntity != null)
            {
                GUILayout.Label("Type: " + firstEntity.GetType().Name);
                GUILayout.Label("Layer: " + firstEntity.LayerName);
                GUILayout.Label("Bounds: " + FormatBounds(firstEntity.Bounds));
            }
            else
            {
                GUILayout.Label("Type: none");
                GUILayout.Label("Layer: -");
                GUILayout.Label("Bounds: -");
            }

            GUILayout.EndArea();
        }

        private ISketchEntity GetFirstSelectedEntity()
        {
            foreach (var entity in GetEntities())
            {
                if (selectionService.IsSelected(entity))
                {
                    return entity;
                }
            }

            return null;
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

        private static string FormatBounds(BoundingBox bounds)
        {
            return string.Format(
                "{0:0.##},{1:0.##} - {2:0.##},{3:0.##}",
                bounds.MinX,
                bounds.MinY,
                bounds.MaxX,
                bounds.MaxY);
        }
    }
}
