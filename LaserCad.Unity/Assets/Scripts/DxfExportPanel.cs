using System;
using System.IO;
using LaserCad.Export.Dxf;
using UnityEngine;

namespace LaserCad.Unity
{
    /// <summary>
    /// Panel eksportu aktualnego dokumentu do DXF.
    /// </summary>
    public sealed class DxfExportPanel : MonoBehaviour
    {
        [SerializeField]
        private LaserCadApplicationController applicationController;

        [SerializeField]
        private Rect panelRect = new Rect(324f, 146f, 280f, 126f);

        private string fileName = "laser-cad-export.dxf";
        private string statusText = "Gotowe";

        private void OnGUI()
        {
            if (applicationController == null)
            {
                return;
            }

            panelRect = GUI.Window(GetInstanceID(), panelRect, DrawWindow, "Eksport DXF");
        }

        private void DrawWindow(int windowId)
        {
            GUILayout.Label("Plik");
            fileName = GUILayout.TextField(fileName);

            GUILayout.Space(8f);
            if (GUILayout.Button("Eksportuj DXF"))
            {
                ExportDxf();
            }

            GUILayout.Label(statusText);
            GUI.DragWindow(new Rect(0f, 0f, panelRect.width, 22f));
        }

        private void ExportDxf()
        {
            try
            {
                if (applicationController.CurrentDocument == null)
                {
                    statusText = "Brak dokumentu.";
                    return;
                }

                string path = GetExportPath(fileName, ".dxf");
                string dxf = new DxfExporter().Export(applicationController.CurrentDocument);
                File.WriteAllText(path, dxf);
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
