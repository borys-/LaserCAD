using System;
using System.IO;
using LaserCad.Export.Svg;
using UnityEngine;

namespace LaserCad.Unity
{
    /// <summary>
    /// Panel eksportu aktualnego dokumentu do SVG.
    /// </summary>
    public sealed class SvgExportPanel : MonoBehaviour
    {
        [SerializeField]
        private LaserCadApplicationController applicationController;

        [SerializeField]
        private Rect panelRect = new Rect(324f, 12f, 280f, 126f);

        private string fileName = "laser-cad-export.svg";
        private string statusText = "Gotowe";

        private void OnGUI()
        {
            if (ViewportProcessMode.IsViewportProcess())
            {
                return;
            }

            if (applicationController == null)
            {
                return;
            }

            panelRect = GUI.Window(GetInstanceID(), panelRect, DrawWindow, "Eksport SVG");
        }

        private void DrawWindow(int windowId)
        {
            GUILayout.Label("Plik");
            fileName = GUILayout.TextField(fileName);

            GUILayout.Space(8f);
            if (GUILayout.Button("Eksportuj SVG"))
            {
                ExportSvg();
            }

            GUILayout.Label(statusText);
            GUI.DragWindow(new Rect(0f, 0f, panelRect.width, 22f));
        }

        private void ExportSvg()
        {
            try
            {
                if (applicationController.CurrentDocument == null)
                {
                    statusText = "Brak dokumentu.";
                    return;
                }

                string path = GetExportPath(fileName, ".svg");
                string svg = new SvgExporter().Export(applicationController.CurrentDocument);
                File.WriteAllText(path, svg);
                statusText = "Zapisano: " + path;
            }
            catch (Exception exception)
            {
                statusText = exception.Message;
            }
        }

        private static string GetExportPath(string requestedFileName, string extension)
        {
            string safeFileName = string.IsNullOrWhiteSpace(requestedFileName)
                ? "laser-cad-export" + extension
                : Path.GetFileName(requestedFileName.Trim());

            if (!safeFileName.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
            {
                safeFileName += extension;
            }

            return Path.Combine(Application.persistentDataPath, safeFileName);
        }
    }
}
