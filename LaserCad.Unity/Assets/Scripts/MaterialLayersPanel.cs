using System;
using System.Globalization;
using System.Linq;
using LaserCad.Core.Documents;
using UnityEngine;

namespace LaserCad.Unity
{
    /// <summary>
    /// Panel profilu materialu i warstw dokumentu.
    /// </summary>
    public sealed class MaterialLayersPanel : MonoBehaviour
    {
        [SerializeField]
        private LaserCadApplicationController applicationController;

        [SerializeField]
        private Rect panelRect = new Rect(616f, 12f, 300f, 254f);

        private readonly MaterialProfile[] materialProfiles = DefaultMaterialProfiles.All.ToArray();
        private int selectedMaterialIndex;
        private bool initialized;

        private void OnGUI()
        {
            if (applicationController == null)
            {
                return;
            }

            EnsureInitialized();
            panelRect = GUI.Window(GetInstanceID(), panelRect, DrawWindow, "Material i warstwy");
        }

        private void DrawWindow(int windowId)
        {
            GUILayout.Label("Profil materialu");
            selectedMaterialIndex = GUILayout.SelectionGrid(selectedMaterialIndex, GetMaterialLabels(), 1);

            if (GUILayout.Button("Zastosuj material"))
            {
                applicationController.SetMaterialProfile(materialProfiles[selectedMaterialIndex]);
            }

            GUILayout.Space(8f);
            DrawMaterialDetails();
            GUILayout.Space(8f);
            DrawLayers();

            GUI.DragWindow(new Rect(0f, 0f, panelRect.width, 22f));
        }

        private void DrawMaterialDetails()
        {
            MaterialProfile profile = GetCurrentMaterialProfile();
            GUILayout.Label("Grubosc: " + Format(profile.Thickness.Millimeters) + " mm");
            GUILayout.Label("Kerf: " + Format(profile.DefaultKerf.Millimeters) + " mm");
            GUILayout.Label("Clearance: " + Format(profile.DefaultClearance.Millimeters) + " mm");
            GUILayout.Label("Min. palec: " + Format(profile.MinimumFingerWidth.Millimeters) + " mm");
        }

        private void DrawLayers()
        {
            GUILayout.Label("Warstwy");
            var document = applicationController.CurrentDocument;
            if (document == null || document.Layers.Count == 0)
            {
                GUILayout.Label("Brak warstw");
                return;
            }

            foreach (Layer layer in document.Layers)
            {
                GUILayout.Label(layer.Name + " | " + layer.Role + " | " + layer.Color.Hex);
            }
        }

        private void EnsureInitialized()
        {
            if (initialized)
            {
                return;
            }

            MaterialProfile profile = GetCurrentMaterialProfile();
            for (int index = 0; index < materialProfiles.Length; index++)
            {
                if (string.Equals(materialProfiles[index].Name, profile.Name, StringComparison.Ordinal))
                {
                    selectedMaterialIndex = index;
                    break;
                }
            }

            initialized = true;
        }

        private MaterialProfile GetCurrentMaterialProfile()
        {
            return applicationController.CurrentDocument != null && applicationController.CurrentDocument.MaterialProfile != null
                ? applicationController.CurrentDocument.MaterialProfile
                : DefaultMaterialProfiles.Plywood3Mm;
        }

        private string[] GetMaterialLabels()
        {
            string[] labels = new string[materialProfiles.Length];
            for (int index = 0; index < materialProfiles.Length; index++)
            {
                labels[index] = materialProfiles[index].Name;
            }

            return labels;
        }

        private static string Format(double value)
        {
            return value.ToString("0.###", CultureInfo.InvariantCulture);
        }
    }
}
